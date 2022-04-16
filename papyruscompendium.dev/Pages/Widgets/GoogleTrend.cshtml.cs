
using System.Threading.Tasks;

using GoogleTrends;
using GoogleTrends.Models.Explore;
using GoogleTrends.Models.GeoData;
using GoogleTrends.Models.Widgets;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace papyruscompendium.dev.Pages.Widgets {
    public class GoogleTrendModel : PageModel {
        [BindProperty]
        public string SearchQuery { get; set; }

        public ExploreResponse ExploreResponse { get; set; }
        public RankedList[] RelatedQueries { get; set; }
        public RankedList[] RelatedTopics { get; set; }
        public GeoMapData[] GeoMapData { get; set; }
        public TimelineData[] TimeLineData { get; set; }

        private readonly IGoogleTrendsClient _googleTrendsClient;

        public GoogleTrendModel(IGoogleTrendsClient googleTrendsClient) {
            _googleTrendsClient = googleTrendsClient;
        }

        public async Task<IActionResult> OnPostAsync() {
            ExploreResponse = await _googleTrendsClient.Explore.ExploreQuery(SearchQuery, SearchType.WebSearch,
                queryTime: QueryTimes.PastYear, geoSearch: GeoIds.WorldWide);

            RelatedQueries = await ExploreResponse.GetRelatedQueries();
            RelatedTopics = await ExploreResponse.GetRelatedTopics();
            GeoMapData = await ExploreResponse.GetGeoMapData();
            TimeLineData = await ExploreResponse.GetTimeLineData();

            return Page();
        }

        public IActionResult OnGet() {
            return Redirect("/Widgets/GoogleTrendSearch");
        }
    }
}
