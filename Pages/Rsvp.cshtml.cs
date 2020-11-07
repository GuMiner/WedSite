using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace WedSite.Pages
{
    public class RsvpModel : PageModel
    {
        private readonly ILogger<RsvpModel> _logger;

        public RsvpModel(ILogger<RsvpModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
    }
}
