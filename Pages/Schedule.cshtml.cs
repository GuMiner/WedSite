using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace WedSite.Pages
{
    public class ScheduleModel : PageModel
    {
        private readonly ILogger<ScheduleModel> _logger;

        public ScheduleModel(ILogger<ScheduleModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
    }
}
