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
        private static bool _dataIsReady = false;
        private readonly string RealmSlug;
        private readonly string GuildNameSlug;
        private readonly string? TokenEndpoint;
        private readonly string? ClientId;
        private readonly string? ClientSecret;
        private readonly string? CurrentExpansion;
        private readonly string? RaidDifficulty;
        public readonly List<Raid> Raids;
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
            CurrentExpansion = blizzardOptions.CurrentExpansion;
            Raids = blizzardOptions.Raids;
            RaidDifficulty = blizzardOptions.RaidDifficulty;
        }

        public Task<bool> GetDataStatusAsync()
        {
            return Task.FromResult(_dataIsReady);
        }

        public async Task<GuildViewModel> GetAllApiData()
        {
            var cacheKey = "GuildSummary";
            if (_cache.TryGetValue(cacheKey, out GuildViewModel cachedData))
            {
                return cachedData;
            }

            await GetAccessTokenAsync();

            var rosterTask = GetRosterDataAsync();
            var mythicKeystoneSeasonIndexTask = GetMythicKeystoneSeasonsIndexDataAsync();

            await Task.WhenAll(rosterTask, mythicKeystoneSeasonIndexTask);
            var guildSummary = new GuildViewModel
            {
                Roster = await rosterTask,
                MythicKeystoneSeasonIndex = await mythicKeystoneSeasonIndexTask,
            };

            if (guildSummary?.Roster?.Members == null)
            {
                return guildSummary;
            }

            await GetJournalExpansionDataAsync(guildSummary);
            await FetchCharacterMediaData(guildSummary);
            await FetchCharacterMythicKeystoneSeason(guildSummary);
            await FetchCharacterProfileSummaries(guildSummary);
            await FetchCharacterRaidData(guildSummary);

            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            };
            _cache.Set(cacheKey, guildSummary, cacheEntryOptions);

            _dataIsReady = true;
            return guildSummary;
        }

        private async Task<List<CharacterMedia?>> FetchCharacterMediaData(GuildViewModel guildSummary)
        {
            return await FetchCharacterData(guildSummary, async characterName =>
            {
                var member = guildSummary.Roster.Members.FirstOrDefault(m => m.Character.Name.ToLower() == characterName);
                if (member == null || !GuildViewModel.AcceptableRoles.Contains(member.Rank))
                {
                    return new CharacterMedia();
                }

                var realmSlug = member?.Character?.Realm?.Slug ?? RealmSlug;
                var (data, statusCode) = await GetCachedDataAsync($"CharacterMediaData_{characterName}",
                    () => _blizzardApi.GetCharacterMedia(realmSlug, characterName, $"Bearer {_accessToken}"));

                member.Media = statusCode == HttpStatusCode.OK ? data : new CharacterMedia();
                return member.Media;
            });
        }

        private async Task<List<MythicKeystoneSeason?>> FetchCharacterMythicKeystoneSeason(GuildViewModel guildSummary)
        {
            return await FetchCharacterData(guildSummary, async characterName =>
            {
                var member = guildSummary.Roster.Members.FirstOrDefault(m => m.Character.Name.ToLower() == characterName);
                if (member == null || !GuildViewModel.AcceptableRoles.Contains(member.Rank))
                {
                    return new MythicKeystoneSeason();
                }

                var seasonId = guildSummary.MythicKeystoneSeasonIndex.CurrentSeason.Id;
                var realmSlug = member?.Character?.Realm?.Slug ?? RealmSlug;
                var (data, statusCode) = await GetCachedDataAsync($"CharacterMythicKeystoneSeasonData_{characterName}",
                    () => _blizzardApi.GetMythicKeystoneSeasonData(realmSlug, characterName, seasonId, $"Bearer {_accessToken}"));

                if(statusCode == HttpStatusCode.OK && data != null)
                {
                    member.MythicKeystoneSeason = data;
                    member.MythicRating = member.MythicKeystoneSeason?.MythicRating?.Rating == null ? "N/A" : Math.Round(member.MythicKeystoneSeason.MythicRating.Rating).ToString();
                    return member.MythicKeystoneSeason;
                }

                member.MythicKeystoneSeason = new MythicKeystoneSeason();
                member.MythicRating = "N/A";
                return member.MythicKeystoneSeason;
            });
        }

        private async Task<List<Character?>> FetchCharacterProfileSummaries(GuildViewModel guildSummary)
        {
            return await FetchCharacterData(guildSummary, async characterName =>
            {
                var member = guildSummary.Roster.Members.FirstOrDefault(m => m.Character.Name.ToLower() == characterName);
                if (member == null)
                {
                    return new Character();
                }

                if (!GuildViewModel.AcceptableRoles.Contains(member.Rank))
                {
                    return member.Character;
                }
                
                var realmSlug = member?.Character?.Realm?.Slug ?? RealmSlug;
                var (data, statusCode) = await GetCachedDataAsync($"CharacterProfileSummaryData_{characterName}",
                    () => _blizzardApi.GetCharacterProfileSummary(realmSlug, characterName, $"Bearer {_accessToken}"));

                if (statusCode == HttpStatusCode.OK && data != null)
                {
                    member.Character.UpdateProperties(data);
                    member.EquippedItemLevel = data.EquippedItemLevel;
                }

                return member.Character;
            });
        }

        private async Task<List<CharacterRaid?>> FetchCharacterRaidData(GuildViewModel guildSummary)
        {
            return await FetchCharacterData(guildSummary, async characterName =>
            {
                var member = guildSummary.Roster.Members.FirstOrDefault(m => m.Character.Name.ToLower() == characterName);
                if (member == null)
                {
                    return new CharacterRaid();
                }

                if (!GuildViewModel.AcceptableRoles.Contains(member.Rank))
                {
                    return member.Raid;
                }

                var realmSlug = member?.Character?.Realm?.Slug ?? RealmSlug;
                var (data, statusCode) = await GetCachedDataAsync($"CharacterRaidData_{characterName}",
                    () => _blizzardApi.GetCharacterRaids(realmSlug, characterName, $"Bearer {_accessToken}"));

                if(statusCode == HttpStatusCode.OK && data != null)
                {
                    var expansions = data.Expansions.FirstOrDefault(s => s.Expansion.Id == guildSummary.CurrentExpansionId);
                    var currentRaidName = Raids.FirstOrDefault(r => r.IsCurrent)?.BlizzardApiName;
                    var instance = expansions?.Instances.FirstOrDefault(i => i.Instance?.Name == currentRaidName)?.Modes
                        .FirstOrDefault(m => m.Difficulty.Name == RaidDifficulty);

                    var raidProgress = "N/A";
                    if (instance != null && instance.Progress != null)
                    {
                        var completed = instance.Progress.CompletedCount;
                        var total = instance.Progress.TotalCount;
                        raidProgress = $"{completed}/{total} {RaidDifficulty[..1].ToUpper()}";
                    }

                    member.Raid = data;
                    member.RaidProgress = raidProgress;
                    return member.Raid;
                }

                member.Raid = new CharacterRaid();
                member.RaidProgress = "N/A";
                return member.Raid;
            });
        }

        private async Task<List<T?>> FetchCharacterData<T>(GuildViewModel guildSummary, Func<string, Task<T>> fetchDataFunc)
        {
            var characterDataTasks = guildSummary.Roster.Members.Select(async member =>
            {
                var characterName = member?.Character?.Name.ToLower();
                if (string.IsNullOrEmpty(characterName))
                {
                    return default;
                }
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
            });

            return (await Task.WhenAll(characterDataTasks)).Where(result => result != null).ToList();
        }

        public async Task GetAccessTokenAsync()
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

        public async Task<Roster> GetRosterDataAsync()
        {
            var (data, statusCode) = await GetCachedDataAsync("RostersData",
                () => _blizzardApi.GetRoster(RealmSlug, GuildNameSlug, $"Bearer {_accessToken}"));

            if (statusCode == HttpStatusCode.OK && data != null)
            {
                data.Members = data.Members?.Where(member => GuildViewModel.AcceptableRoles.Contains(member.Rank)).ToList();
            }

            return statusCode == HttpStatusCode.OK ? data : new Roster();
        }

        public async Task<MythicKeystoneSeasonIndex> GetMythicKeystoneSeasonsIndexDataAsync()
        {
            var (data, statusCode) = await GetCachedDataAsync("MythicKeystoneSeasonsIndexData",
                () => _blizzardApi.GetMythicKeystoneSeasonIndex($"Bearer {_accessToken}"));
            return statusCode == HttpStatusCode.OK ? data : new MythicKeystoneSeasonIndex();
        }

        public async Task<JournalExpansion> GetJournalExpansionDataAsync(GuildViewModel guildSummary)
        {
            var (data, statusCode) = await GetCachedDataAsync("JournalExpansionData",
                () => _blizzardApi.GetJournalExpansionData($"Bearer {_accessToken}"));

            if (statusCode == HttpStatusCode.OK && data != null) {
                guildSummary.CurrentExpansionId = data.Tiers.FirstOrDefault(e => e.Name == CurrentExpansion)?.Id ?? 0;
                return data;
            }
            return new JournalExpansion();
        }

        public async Task<(T Data, HttpStatusCode StatusCode)> GetCachedDataAsync<T>(string endpointKey, Func<Task<T>> apiCall) where T : new()
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