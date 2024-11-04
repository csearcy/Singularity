using System.Net.Http.Headers;
using Microsoft.Extensions.Caching.Memory;
using Singularity.Models;
using Singularity.Models.BlizzardApiModels;
using IdentityModel.Client;
using Singularity.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Singularity.Services
{
    public class BlizzardDataService : IBlizzardDataService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly SemaphoreSlim _cacheLock = new(1, 1);
        private readonly string BaseUrl;
        private readonly string RealmSlug;
        private readonly string GuildNameSlug;
        private readonly string? TokenEndpoint;
        private readonly string? ClientId;

        private readonly string? ClientSecret;

        private string? _accessToken;


        public BlizzardDataService(HttpClient httpClient, IMemoryCache cache, IOptions<BlizzardApiOptions> options)
        {
            _httpClient = httpClient;
            _cache = cache;

            var blizzardOptions = options.Value;
            BaseUrl = blizzardOptions.BaseUrl;
            RealmSlug = blizzardOptions.Guild.Realm;
            GuildNameSlug = blizzardOptions.Guild.Name;
            TokenEndpoint = blizzardOptions.TokenEndpoint;
            ClientId = blizzardOptions.ClientId;
            ClientSecret = blizzardOptions.ClientSecret;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            if (!string.IsNullOrEmpty(_accessToken))
            {
                return _accessToken;
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
            return _accessToken;
        }

        public async Task<Roster> GetRosterDataAsync()
        {
            var endpoint = $"{BaseUrl}/data/wow/guild/{RealmSlug}/{GuildNameSlug}/roster?namespace=profile-us&locale=en_US";
            var json = await GetCachedDataAsync($"RostersData", endpoint);

            return JsonSerializer.Deserialize<Roster>(json);
        }

        public async Task<string> GetMythicKeystoneSeasonsIndexDataAsync() {
            var endpoint = $"{BaseUrl}/data/wow/mythic-keystone/season/index?namespace=dynamic-us&locale=en_US";
            return await GetCachedDataAsync($"MythicKeystoneSeasonsIndexData", endpoint);
        }



        public async Task<string> GetWowDataAsync(string endpoint)
        {
            var token = await GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"https://us.api.blizzard.com{endpoint}");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetCachedDataAsync(string endpointKey, string apiEndpoint)
        {
            if (_cache.TryGetValue(endpointKey, out string? cachedData))
            {
                return cachedData ?? string.Empty;
            }

            await _cacheLock.WaitAsync();
            try
            {
                if (!_cache.TryGetValue(endpointKey, out cachedData))
                {
                    var token = await GetAccessTokenAsync();
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var response = await _httpClient.GetAsync(apiEndpoint);
                    response.EnsureSuccessStatusCode();
                    cachedData = await response.Content.ReadAsStringAsync();

                    var cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                    };
                    _cache.Set(endpointKey, cachedData, cacheEntryOptions);
                }
            }
            finally
            {
                _cacheLock.Release();
            }

            return cachedData;
        }
    }
}