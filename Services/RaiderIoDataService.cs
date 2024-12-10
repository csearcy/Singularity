using Microsoft.Extensions.Caching.Memory;
using Singularity.Models;
using Singularity.Models.Race;
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
        private readonly int TopXRealmRaceRanks;
        private readonly int TopXBossRanks;
        private readonly string TeamNameToIgnoreForRace;
        public readonly List<Raid> Raids;
        private static bool _dataIsReady = false;

        public RaiderIoDataService(IMemoryCache cache, IOptions<RaiderIoApiOptions> options, IRaiderIoApi raiderIoApi)
        {
            _cache = cache;
            _raiderIoApi = raiderIoApi;

            var raiderIoOptions = options.Value;
            RealmSlug = raiderIoOptions.Realm;
            RaidDifficulty = raiderIoOptions.RaidDifficulty;
            TopXBossRanks = raiderIoOptions.TopXBossRanks;
            TopXRealmRaceRanks = raiderIoOptions.TopXRealmRaceRanks;
            TeamNameToIgnoreForRace = raiderIoOptions.TeamNameToIgnoreForRace;
            Raids = raiderIoOptions.Raids;
        }

        public Task<bool> GetDataStatusAsync()
        {
            return Task.FromResult(_dataIsReady);
        }

        public async Task<RaceModel> GetAllApiData(List<Boss> bosses)
        {
            var cacheKey = "RaceSummary";
            if (_cache.TryGetValue(cacheKey, out RaceModel cachedData))
            {
                return cachedData;
            }

            var raidName = Raids.First(s => s.IsCurrent).RaidSlugName;
            var raceModel = new RaceModel
            {
                SelectedExpansion = raidName,
                Raids = Raids,
                BossCount = bosses.Count
            };
            await GetRaidRankingsAsync(raceModel, raidName);
            await GetTopXBossRankingsAsync(raceModel, bosses, raidName);

            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            };
            _cache.Set(cacheKey, raceModel, cacheEntryOptions);

            _dataIsReady = true;
            return raceModel;
        }

        private async Task<RaidRanking> GetRaidRankingsAsync(RaceModel raceModel, string raidName)
        {
            var (data, statusCode) = await GetCachedDataAsync($"RaceRankingData_{raidName}_{RaidDifficulty}",
                    () => _raiderIoApi.GetRaidRankings(raidName, RaidDifficulty, RealmSlug));

            if (statusCode == HttpStatusCode.OK && data != null)
            {
                raceModel.RaidRankingParent = data;
                int guildToIgnoreRank = data.RaidRankings?.Where(s => s.Guild.Name.Equals(TeamNameToIgnoreForRace, StringComparison.OrdinalIgnoreCase)).Select(s => s.Rank).FirstOrDefault() ?? 0;
                raceModel.GuildToIgnoreRank = guildToIgnoreRank;
                raceModel.TopXRealmRaceRanks = GetTopRealmRaceRankings(data.RaidRankings);
                return data;
            }

            return new RaidRanking();
        }

        private async Task<List<BossRanking>> GetTopXBossRankingsAsync(RaceModel raceModel, IEnumerable<Boss> bosses, string raidName)
        {
            var bossRankings = new List<BossRanking>();
            foreach (var boss in bosses)
            {
                if (boss?.Slug == null)
                {
                    continue;
                }

                var bossSlug = GetBossSlug(boss.Slug);
                var (data, statusCode) = await GetCachedDataAsync($"BossRankingData_{raidName}_{RaidDifficulty}_{bossSlug}",
                    () => _raiderIoApi.GetBossRankings(raidName, RaidDifficulty, bossSlug, RealmSlug));

                if (statusCode != HttpStatusCode.OK || data == null)
                {
                    continue;
                }

                bossRankings.Add(new BossRanking
                {
                    BossName = boss?.Name ?? "N/A",
                    BossSlugName = boss?.Slug ?? "N/A",
                    BossImageUrl = boss?.ImageUrl ?? "N/A",
                    BossRankings = GetTopBossRankings(data.BossRankings)
                });
            }

            raceModel.BossRankings = bossRankings;
            raceModel.TopXBossRaceRanks = bossRankings;
            return raceModel.BossRankings;
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

        public List<Ranking> GetTopRealmRaceRankings(List<Ranking> realmRankings)
        {
            var filteredRankings = realmRankings
                .Where(r => !r.Guild.Name.Equals(TeamNameToIgnoreForRace, StringComparison.OrdinalIgnoreCase))
                .OrderBy(r => r.Rank)
                .Take(TopXRealmRaceRanks)
                .ToList();

            for (int i = 0; i < filteredRankings.Count; i++)
            {
                filteredRankings[i].Rank = i + 1;
            }

            while (filteredRankings.Count < TopXRealmRaceRanks)
            {
                filteredRankings.Add(new Ranking
                {
                    Rank = filteredRankings.Count + 1,
                    Guild = new Guild 
                    { 
                        Name = "TBD" 
                    }
                });
            }

            return filteredRankings;
        }

        public List<Ranking> GetTopBossRankings(List<Ranking> bossRankings)
        {
            var filteredRankings = bossRankings
                .Where(r => !r.Guild.Name.Equals(TeamNameToIgnoreForRace, StringComparison.OrdinalIgnoreCase))
                .OrderBy(r => r.Rank)
                .Take(TopXBossRanks)
                .ToList();

            for (int i = 0; i < filteredRankings.Count; i++)
            {
                filteredRankings[i].Rank = i + 1;
            }

            while (filteredRankings.Count < TopXBossRanks)
            {
                filteredRankings.Add(new Ranking
                {
                    Rank = filteredRankings.Count + 1,
                    Guild = new Guild 
                    { 
                        Name = "TBD" 
                    }
                });
            }

            return filteredRankings;
        }

        public string GetBossSlug(string bossName)
        {
            var specialSlugs = new Dictionary<string, string>
            {
                { "nexusprincess-kyveza", "nexus-princess-kyveza" },
                { "sikran-captain-of-the-sureki", "sikran" }
            };

            if (specialSlugs.TryGetValue(bossName, out var slug))
            {
                return slug;
            }

            return bossName;
        }
    }
}