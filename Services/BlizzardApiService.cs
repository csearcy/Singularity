using IdentityModel.Client;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using Singularity.Models;

namespace Singularity.Services
{
    public class BlizzardApiService
    {
        private readonly HttpClient _httpClient;
        private readonly BlizzardApiOptions _options;
        private string? _accessToken;

        public BlizzardApiService(HttpClient httpClient, IOptions<BlizzardApiOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        private async Task<string> GetAccessTokenAsync()
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

        public async Task<string> GetWowDataAsync(string endpoint)
        {
            var token = await GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"https://us.api.blizzard.com{endpoint}");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
