using System.Net.Http.Headers;
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
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly SemaphoreSlim _cacheLock = new(1, 1);
        private readonly IBlizzardApi _blizzardApi;

        private readonly string BaseUrl;
        private readonly string RealmSlug;
        private readonly string GuildNameSlug;
        private readonly string? TokenEndpoint;
        private readonly string? ClientId;
        private readonly string? ClientSecret;
        private string? _accessToken;


        public BlizzardDataService(HttpClient httpClient, IMemoryCache cache, IOptions<BlizzardApiOptions> options, IBlizzardApi blizzardApi)
        {
            _httpClient = httpClient;
            _cache = cache;
            _blizzardApi = blizzardApi;

            var blizzardOptions = options.Value;
            BaseUrl = blizzardOptions.BaseUrl;
            RealmSlug = blizzardOptions.Guild.Realm;
            GuildNameSlug = blizzardOptions.Guild.Name;
            TokenEndpoint = blizzardOptions.TokenEndpoint;
            ClientId = blizzardOptions.ClientId;
            ClientSecret = blizzardOptions.ClientSecret;
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
            await GetAccessTokenAsync();
            return await GetCachedDataAsync("RostersData", 
                () => _blizzardApi.GetRoster(RealmSlug, GuildNameSlug, $"Bearer {_accessToken}"));
        }

        public async Task<MythicKeystoneSeasonIndex> GetMythicKeystoneSeasonsIndexDataAsync() {
            await GetAccessTokenAsync();
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
                if (_cache.TryGetValue(endpointKey, out cachedData))
                {
                    return (T)cachedData;
                }
                
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