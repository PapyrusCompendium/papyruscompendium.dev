using System.Linq;
using System.Threading.Tasks;

using GoogleTrends;

using Microsoft.AspNetCore.Mvc;

namespace papyruscompendium.dev.Controller {
    [Route("api/[controller]")]
    public class AutoCompleteQuery : ControllerBase {
        private readonly IGoogleTrendsClient _googleTrendsClient;

        public AutoCompleteQuery(IGoogleTrendsClient googleTrendsClient) {
            _googleTrendsClient = googleTrendsClient;
        }

        [ValidateAntiForgeryToken]
        [HttpGet]
        public async Task<IActionResult> GetRecommendedQueries(string searchQuery) {
            var autoCompleteResults = await _googleTrendsClient.AutoComplete.GetAutoCompleteSuggestions(searchQuery);
            return Ok(autoCompleteResults.Select(i => $"({i.Type}) {i.Title}"));
        }
    }
}
