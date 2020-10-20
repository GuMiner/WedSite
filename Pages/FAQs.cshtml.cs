using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace WedSite.Pages
{
    public class FAQsModel : PageModel
    {
        private readonly ILogger<FAQsModel> _logger;

        public FAQsModel(ILogger<FAQsModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
    }
}
