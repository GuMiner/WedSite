using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using WedSite.Data;
using WedSite.Database;

namespace WedSite.Pages
{
    [Authorize(Roles = "Admin,AdminReadOnly")]
    public class AdminModel : PageModel
    {
        private readonly ILogger<AdminModel> _logger;
        private readonly IDatabase database;

        public AdminModel(IDatabase database, ILogger<AdminModel> logger)
        {
            this.database = database;
            _logger = logger;
        }

        public IEnumerable<Guest> Guests => database.GetGuests();

        [BindProperty]
        public InputModel Input { get; set; }

        [BindProperty]
        public Guest Update { get; set; }

        public class InputModel
        {
            public string PartyName { get; set; }
        }

        public void OnGet() { }

        public void OnPostAdd()
        {
            database.AddGuest(Guest.GenerateCode(), Input.PartyName);
        }

        public void OnPostUpdate()
        {
            database.UpdateGuest(Update);
        }
    }
}
