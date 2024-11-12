using Microsoft.Extensions.Caching.Memory;
using Singularity.Models;
using Singularity.Models.BlizzardApiModels;
using IdentityModel.Client;
using Singularity.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Net;

namespace Singularity.Services
{
    public class BlizzardDataService : IBlizzardDataService
    {
        private readonly IMemoryCache _cache;
        private readonly SemaphoreSlim _cacheLock = new(1, 1);
        private readonly IBlizzardApi _blizzardApi;

        private readonly string RealmSlug;
        private readonly string GuildNameSlug;
        private readonly string? TokenEndpoint;
        private readonly string? ClientId;
        private readonly string? ClientSecret;
        private string? _accessToken;

        public BlizzardDataService(IMemoryCache cache, IOptions<BlizzardApiOptions> options, IBlizzardApi blizzardApi)
        {
            _cache = cache;
            _blizzardApi = blizzardApi;

            var blizzardOptions = options.Value;
            RealmSlug = blizzardOptions.Guild.Realm;
            GuildNameSlug = blizzardOptions.Guild.Name;
            TokenEndpoint = blizzardOptions.TokenEndpoint;
            ClientId = blizzardOptions.ClientId;
            ClientSecret = blizzardOptions.ClientSecret;
        }

        public async Task<GuildViewModel> GetAllApiData()
        {
            await GetAccessTokenAsync();

            var rosterTask = GetRosterDataAsync();
            var mythicKeystoneSeasonIndexTask = GetMythicKeystoneSeasonsIndexDataAsync();

            await Task.WhenAll(rosterTask, mythicKeystoneSeasonIndexTask);

            var guildSummary = new GuildViewModel
            {
                Roster = await rosterTask,
                MythicKeystoneSeasonIndex = await mythicKeystoneSeasonIndexTask
            };

            if (guildSummary?.Roster?.Members == null)
            {
                return guildSummary;
            }

            var fetchTasks = guildSummary.Roster.Members.Select(async member =>
            {
                member.Media = await FetchCharacterMediaData(member);
                member.MythicKeystoneSeason = await FetchCharacterMythicKeystoneSeason(member, guildSummary.MythicKeystoneSeasonIndex.CurrentSeason.Id);
                member.Character = await FetchCharacterProfileSummaries(member);
            });

            await Task.WhenAll(fetchTasks);

            return guildSummary;
        }

        private async Task<CharacterMedia?> FetchCharacterMediaData(Member member)
        {
            return await FetchCharacterData(member, async characterName =>
            {
                if (member == null || !GuildViewModel.AcceptableRoles.Contains(member.Rank))
                {
                    return new CharacterMedia();
                }

                var (data, statusCode) = await GetCachedDataAsync($"CharacterMediaData_{characterName}",
                    () => _blizzardApi.GetCharacterMedia(RealmSlug, characterName, $"Bearer {_accessToken}"));

                return statusCode == HttpStatusCode.OK ? data : member.Media;
            });
        }

        private async Task<MythicKeystoneSeason?> FetchCharacterMythicKeystoneSeason(Member member, int seasonId)
        {
            return await FetchCharacterData(member, async characterName =>
            {
                if (member == null || !GuildViewModel.AcceptableRoles.Contains(member.Rank))
                {
                    return new MythicKeystoneSeason();
                }

                var (data, statusCode) = await GetCachedDataAsync($"CharacterMythicKeystoneSeasonData_{characterName}",
                    () => _blizzardApi.GetMythicKeystoneSeasonData(RealmSlug, characterName, seasonId, $"Bearer {_accessToken}"));

                return statusCode == HttpStatusCode.OK ? data : new MythicKeystoneSeason();
            });
        }

        private async Task<Character?> FetchCharacterProfileSummaries(Member member)
        {
            return await FetchCharacterData(member, async characterName =>
            {
                if (member == null)
                {
                    return new Character();
                }

                if (!GuildViewModel.AcceptableRoles.Contains(member.Rank))
                {
                    return member.Character;
                }

                var (data, statusCode) = await GetCachedDataAsync($"CharacterProfileSummaryData_{characterName}",
                    () => _blizzardApi.GetCharacterProfileSummary(RealmSlug, characterName, $"Bearer {_accessToken}"));

                if (statusCode == HttpStatusCode.OK && data != null)
                {
                    data.UpdateProperties(member.Character);
                }

                return statusCode == HttpStatusCode.OK ? data : new Character();
            });
        }

        private async Task<T?> FetchCharacterData<T>(Member member, Func<string, Task<T>> fetchDataFunc)
        {
            if (member?.Character?.Name == null)
            {
                return default;
            }

            var characterName = member.Character.Name.ToLower();
            try
            {
                var data = await fetchDataFunc(characterName);
                return data;
            }
            catch (Refit.ApiException apiEx) when (apiEx.StatusCode == HttpStatusCode.NotFound)
            {
                return default;
            }
            catch (Exception)
            {
                return default;
            }
        }

        private async Task GetAccessTokenAsync()
        {
            if (!string.IsNullOrEmpty(_accessToken))
            {
                return;
            }

            var client = new HttpClient();
            var tokenRequest = new ClientCredentialsTokenRequest
            {
                Address = TokenEndpoint,
                ClientId = ClientId,
                ClientSecret = ClientSecret,
            };

            var tokenResponse = await client.RequestClientCredentialsTokenAsync(tokenRequest);
            if (tokenResponse.IsError) throw new Exception("Failed to retrieve access token");

            _accessToken = tokenResponse.AccessToken;
        }

        private async Task<Roster> GetRosterDataAsync()
        {
            var (data, statusCode) = await GetCachedDataAsync("RostersData",
                () => _blizzardApi.GetRoster(RealmSlug, GuildNameSlug, $"Bearer {_accessToken}"));

            if (statusCode == HttpStatusCode.OK && data != null)
            {
                data.Members = data.Members?.Where(member => GuildViewModel.AcceptableRoles.Contains(member.Rank)).ToList();
            }

            return statusCode == HttpStatusCode.OK ? data : new Roster();
        }

        private async Task<MythicKeystoneSeasonIndex> GetMythicKeystoneSeasonsIndexDataAsync()
        {
            var (data, statusCode) = await GetCachedDataAsync("MythicKeystoneSeasonsIndexData",
                () => _blizzardApi.GetMythicKeystoneSeasonIndex($"Bearer {_accessToken}"));
            return statusCode == HttpStatusCode.OK ? data : new MythicKeystoneSeasonIndex();
        }

        private async Task<(T Data, HttpStatusCode StatusCode)> GetCachedDataAsync<T>(string endpointKey, Func<Task<T>> apiCall) where T : new()
        {
            if (_cache.TryGetValue(endpointKey, out var cachedData))
            {
                return ((T)cachedData, HttpStatusCode.OK);
            }

            await _cacheLock.WaitAsync();
            try
            {
                var response = await apiCall();
                HttpStatusCode statusCode = HttpStatusCode.OK;

                if (response == null)
                {
                    statusCode = HttpStatusCode.NotFound;
                    return (new T(), statusCode);
                }

                cachedData = response;

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                };
                _cache.Set(endpointKey, cachedData, cacheEntryOptions);

                return ((T)cachedData, statusCode);
            }
            catch (Refit.ApiException apiEx) when (apiEx.StatusCode == HttpStatusCode.NotFound)
            {
                return (new T(), HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                return (new T(), HttpStatusCode.InternalServerError);
            }
            finally
            {
                _cacheLock.Release();
            }
        }
    }
}