using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace WedSite.Pages
{
    public class HotelTravelModel : PageModel
    {
        private readonly ILogger<ScheduleModel> _logger;

        public HotelTravelModel(ILogger<ScheduleModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
    }
}
