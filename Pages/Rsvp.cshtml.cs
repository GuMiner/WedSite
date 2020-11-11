using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Claims;
using WedSite.Data;
using WedSite.Database;

namespace WedSite.Pages
{
    [Authorize]
    public class RsvpModel : PageModel
    {
        private readonly ILogger<RsvpModel> _logger;
        private readonly IDatabase database;

        public RsvpModel(IDatabase database, ILogger<RsvpModel> logger)
        {
            _logger = logger;
            this.database = database;
        }

        [BindProperty]
        public Guest Guest { get; set; }

        [BindProperty]
        public string SongRequests { get; set; }

        [BindProperty]
        public string AdultCount { get; set; }

        [BindProperty]
        public string KidCount { get; set; }

        public void OnGet()
        {
            var guestCode = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            this.Guest = database.GetGuest(guestCode);
        }

        public void OnPostRsvpYes()
        {
            var guestCode = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            this.Guest = database.GetGuest(guestCode);

            this.Guest.ReservationState = GuestStates.RsvpCanCome;
            this.Guest.ReservationNotes = AdultCount + " (" + KidCount + " kids)";
            this.Guest.SongRequests = SongRequests;
            database.UpdateGuest(this.Guest);
        }

        public void OnPostRsvpNo()
        {
            var guestCode = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            this.Guest = database.GetGuest(guestCode);

            this.Guest.ReservationState = GuestStates.RsvpCannotCome;
            database.UpdateGuest(this.Guest);
        }

        public void OnPostRsvpReset()
        {
            var guestCode = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            this.Guest = database.GetGuest(guestCode);

            this.Guest.ReservationState = GuestStates.LoginNoRsvp;
            database.UpdateGuest(this.Guest);
        }
    }
}
