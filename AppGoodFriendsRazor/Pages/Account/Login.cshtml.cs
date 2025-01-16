using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using AppMusicRazor.SeidoHelpers;
using static AppMusicRazor.Pages.RegisterModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace AppMusicRazor.Pages.Account
{
	public class LoginModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<User> signInManager, ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public csLoginIM LoginIM { get; set; }

        //For Validation and Identity Errors
        public ModelValidationResult ValidationResult { get; set; } = new ModelValidationResult(false, null, null);

        //public string ReturnUrl { get; set; }

        public class csLoginIM
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            public bool RememberMe { get; set; }
        }

        public void OnGetAsync()
        {
            LoginIM = new csLoginIM();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValidPartially(out ModelValidationResult validationResult))
            {
                ValidationResult = validationResult;
                return Page();
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result = await _signInManager.PasswordSignInAsync(LoginIM.Email, LoginIM.Password, LoginIM.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                _logger.LogInformation("Login succeeded");
                return LocalRedirect("/");
            }

            //Failed Login
            ValidationResult = new ModelValidationResult(true,
                    new List<string>() { "Invalid login attempt." }, null) ;
            return Page();
        }
    }
}
