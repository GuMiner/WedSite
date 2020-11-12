using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace WedSite.Pages
{
    [Authorize(Roles = "Admin")]
    public class AdminVisitorsModel : PageModel
    {
        private readonly ILogger<AdminVisitorsModel> _logger;

        public AdminVisitorsModel(ILogger<AdminVisitorsModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
    }
}
