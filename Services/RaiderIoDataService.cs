using Microsoft.Extensions.Caching.Memory;
using Singularity.Models;
using Singularity.Models.RaiderIoApiModels;
using IdentityModel.Client;
using Singularity.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Net;

namespace Singularity.Services
{
    public class RaiderIoDataService : IRaiderIoDataService
    {
        private readonly IMemoryCache _cache;
        private readonly SemaphoreSlim _cacheLock = new(1, 1);
        private readonly IRaiderIoApi _raiderIoApi;
        private readonly string RealmSlug;
        private readonly string RaidDifficulty;
        public readonly List<Raid> Raids;
        private static bool _dataIsReady = false;
        
        public RaiderIoDataService(IMemoryCache cache, IOptions<RaiderIoApiOptions> options, IRaiderIoApi raiderIoApi)
        {
            _cache = cache;
            _raiderIoApi = raiderIoApi;

            var raiderIoOptions = options.Value;
            RealmSlug = raiderIoOptions.Realm;
            RaidDifficulty = raiderIoOptions.RaidDifficulty;
            Raids = raiderIoOptions.Raids;
        }

        public Task<bool> GetDataStatusAsync()
        {
            return Task.FromResult(_dataIsReady);
        }

        public async Task<RaceViewModel> GetAllApiData()
        {
            var cacheKey = "RaceSummary";
            if (_cache.TryGetValue(cacheKey, out RaceViewModel cachedData))
            {
                return cachedData;
            }

            var raidRankingsTask = GetRaidRankingsAsync();

            var raceSummary = new RaceViewModel {
                RaidRankings = await raidRankingsTask
            };

            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            };
            _cache.Set(cacheKey, raceSummary, cacheEntryOptions);

            _dataIsReady = true;
            return raceSummary;
        }        

        public async Task<List<Ranking>> GetRaidRankingsAsync()
        {
            try
            {
                var (data, statusCode) = await GetCachedDataAsync("RaceRankingData",
                    () => _raiderIoApi.GetRaidRankings(Raids.First(s => s.IsCurrent).RaiderIoApiName, RaidDifficulty, RealmSlug));

                if (statusCode == HttpStatusCode.OK && data != null)
                {
                    return data;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }            

            return new List<Ranking>();
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