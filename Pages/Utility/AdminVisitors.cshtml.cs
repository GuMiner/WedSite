﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace WedSite.Pages
{
    public class AdminVisitorsModel : PageModel
    {
        private readonly ILogger<ScheduleModel> _logger;

        public AdminVisitorsModel(ILogger<ScheduleModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
    }
}
