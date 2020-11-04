using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace WedSite.Pages
{
    public class PageNotFoundModel : PageModel
    {
        private readonly ILogger<PageNotFoundModel> _logger;

        public PageNotFoundModel(ILogger<PageNotFoundModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            _logger.LogError("Page was not found in this session!");
        }
    }
}
