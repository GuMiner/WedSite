using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace WedSite.Pages
{
    public class WeddingPartyModel : PageModel
    {
        private readonly ILogger<WeddingPartyModel> _logger;

        public WeddingPartyModel(ILogger<WeddingPartyModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
    }
}
