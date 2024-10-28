using System.Net.Http.Headers;
using Microsoft.Extensions.Caching.Memory;
using Singularity.Models;
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
        private readonly BlizzardApiOptions _options;
        private string? _accessToken;

        public BlizzardDataService(HttpClient httpClient, IMemoryCache cache, IOptions<BlizzardApiOptions> options)
        {
            _httpClient = httpClient;
            _cache = cache;
            _options = options.Value;
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
                Address = _options.TokenEndpoint,
                ClientId = _options.ClientId,
                ClientSecret = _options.ClientSecret,
            };

            var tokenResponse = await client.RequestClientCredentialsTokenAsync(tokenRequest);
            if (tokenResponse.IsError) throw new Exception("Failed to retrieve access token");

            _accessToken = tokenResponse.AccessToken;
            return _accessToken;
        }

        public async Task<string> GetMembersDataAsync()
        {
            var realmSlug = _options.Guild.Realm;
            var guildNameSlug = _options.Guild.Name;
            var baseUrl = _options.BaseUrl;

            var endpoint = $"{baseUrl}/data/wow/guild/{realmSlug}/{guildNameSlug}/roster?namespace=profile-us&locale=en_US";
            return await GetCachedDataAsync($"MembersData", endpoint);
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
            if (_cache.TryGetValue(endpointKey, out string cachedData))
            {
                return cachedData;
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