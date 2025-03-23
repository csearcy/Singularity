namespace Singularity.Models.Race
 {
    public class BossViewModel
    {
        public string Name { get; set; } = default!;
        public string NameSlug { get; set; } = default!;
        public string ImageUrl { get; set; } = default!;
        public List<RankingViewModel> RankingViewModels { get; set; } = default!;
    }
}