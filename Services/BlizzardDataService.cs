using Microsoft.Extensions.Caching.Memory;
using Singularity.Models;
using Singularity.Models.BlizzardApiModels;
using IdentityModel.Client;
using Singularity.Services.Interfaces;
using Microsoft.Extensions.Options;

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

            guildSummary.CharacterMedias = await FetchCharacterMediaData(guildSummary);
            return guildSummary;
        }

        private async Task<List<CharacterMedia?>> FetchCharacterMediaData(GuildViewModel guildSummary)
        {
            if(guildSummary?.Roster?.Members == null) {
                return null;
            }

            var characterMediaTasks = guildSummary.Roster.Members.Select(async member =>
            {
                var characterName = member?.Character?.Name.ToLower();
                if (string.IsNullOrEmpty(characterName))
                {
                    return null;
                }
                try
                {
                    member.Media = await GetCharacterMediaDataAsync(characterName);
                    return member.Media;
                }
                catch (Refit.ApiException apiEx) when (apiEx.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Console.WriteLine($"Character media for {characterName} not found (404).");
                    return null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving character media for {characterName}: {ex.Message}");
                    return null;
                }
            });

            var characterMediaResults = (await Task.WhenAll(characterMediaTasks)).Where(result => result != null).ToList();
            return characterMediaResults;
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
            return await GetCachedDataAsync("RostersData", 
                () => _blizzardApi.GetRoster(RealmSlug, GuildNameSlug, $"Bearer {_accessToken}"));
        }
        
        public async Task<CharacterMedia> GetCharacterMediaDataAsync(string characterName)
        {
            return await GetCachedDataAsync("CharacterMediaData", 
                () => _blizzardApi.GetCharacterMedia(RealmSlug, characterName, $"Bearer {_accessToken}"));
        }

        public async Task<MythicKeystoneSeasonIndex> GetMythicKeystoneSeasonsIndexDataAsync() {
            return await GetCachedDataAsync("MythicKeystoneSeasonsIndexData", 
                () => _blizzardApi.GetMythicKeystoneSeasonIndex($"Bearer {_accessToken}"));
        }

        public async Task<T> GetCachedDataAsync<T>(string endpointKey, Func<Task<T>> apiCall)
        {
            if (_cache.TryGetValue(endpointKey, out var cachedData))
            {
                return (T)cachedData;
            }

            await _cacheLock.WaitAsync();
            try
            {
                var response = await apiCall();
                if (response is HttpResponseMessage httpResponse)
                {
                    httpResponse.EnsureSuccessStatusCode();
                    cachedData = await httpResponse.Content.ReadAsStringAsync();
                }
                else
                {
                    cachedData = response;
                }

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                };
                _cache.Set(endpointKey, cachedData, cacheEntryOptions);
            }
            finally
            {
                _cacheLock.Release();
            }

            return (T)cachedData;
        }
    }
}