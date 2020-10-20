using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace WedSite.Pages
{
    public class RegistryModel : PageModel
    {
        private readonly ILogger<RegistryModel> _logger;

        public RegistryModel(ILogger<RegistryModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
    }
}
