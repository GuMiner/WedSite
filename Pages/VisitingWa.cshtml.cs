using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace WedSite.Pages
{
    public class VisitingWaModel : PageModel
    {
        private readonly ILogger<VisitingWaModel> _logger;

        public VisitingWaModel(ILogger<VisitingWaModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
    }
}
