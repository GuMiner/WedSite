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

namespace WedSite.Pages
{
    [AllowAnonymous]
    public class AdminLoginModel : PageModel
    {
        private readonly ILogger<AdminLoginModel> logger;
        private readonly IDatabase database;

        public AdminLoginModel(IDatabase database, ILogger<AdminLoginModel> logger)
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
            public string AdminCode { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("/Utility/Admin");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("/Utility/Admin");

            if (ModelState.IsValid)
            {
                var IP = Utilities.GetIp(Request);

                var adminID = -42;
                var nameRole = "Admin";
                var email = "gus.gran@outlook.com";
                if (Input.AdminCode.Equals("AngieReadOnlyAdmin"))
                {
                    adminID = -43;
                    nameRole = "AdminReadOnly";
                    email = "a.k.brazier@comcast.net";
                }
                else if (!Input.AdminCode.Equals("superSecureAdmin42"))
                {
                    ModelState.AddModelError(string.Empty, "That's not the admin password!");
                    Console.WriteLine($"ADMIN Login failure at IP {IP}.");
                    return Page();
                }

                long loginId = database.AddGuestLogin(new GuestLogin(adminID, IP));

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Name, nameRole),
                    new Claim(ClaimTypes.Role, nameRole),
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

                logger.LogInformation($"Admin {nameRole} logged in.");
                return LocalRedirect(returnUrl);
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
