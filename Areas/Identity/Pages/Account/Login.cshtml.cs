using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System;
using System.Collections.Generic;
using WedSite.Database;
using WedSite.Data;
using WedSite.Tracker;

namespace WedSite.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly ILogger<LoginModel> logger;
        private readonly IDatabase database;

        public LoginModel(IDatabase database, ILogger<LoginModel> logger)
        {
            this.database = database;
            this.logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [RegularExpression("[0-9]{3}\\-?[0-9]{3}\\-?[0-9]{1}",
                ErrorMessage = "Wedding invitation code doesn't look right. Valid codes will look like '000-000-0'. Dashes are optional.")]
            public string RsvpCode { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("/Index");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("/Rsvp");

            if (ModelState.IsValid)
            {
                Input.RsvpCode = Input.RsvpCode.Replace("-", string.Empty);

                string IP = Utilities.GetIp(Request);
                
                Guest guest = database.GetGuest(Input.RsvpCode);
                if (guest is null)
                {
                    ModelState.AddModelError(string.Empty, "Couldn't find a wedding invitation using that code.");
                    Console.WriteLine($"Login failure at IP {IP} with code {Input.RsvpCode}");
                    return Page();
                }

                long loginId = database.AddGuestLogin(new GuestLogin(guest.Id, IP));

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, "gus.gran@outlook.com"),
                    new Claim(ClaimTypes.Name, guest.PartyName),
                    new Claim(ClaimTypes.Role, "Guest"),
                    new Claim(ClaimTypes.SerialNumber, loginId.ToString()),
                };
                
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1),
                    IsPersistent = true,
                    IssuedUtc = DateTimeOffset.UtcNow,
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                logger.LogInformation($"{guest.PartyName} logged in.");
                return LocalRedirect(returnUrl);
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
