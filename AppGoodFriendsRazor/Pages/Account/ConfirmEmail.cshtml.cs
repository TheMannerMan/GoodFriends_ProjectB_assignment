using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Models;
using AppMusicRazor.SeidoHelpers;
using System.Text.Encodings.Web;

namespace AppMusicRazor.Pages.Account
{
	public class ConfirmEmailModel : PageModel
    {
        private readonly ILogger<RegisterModel> _logger;

        #region Injected by ASP.NET Core Identity
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender _emailSender;
        #endregion

        //For Validation and Identity Errors
        public ModelValidationResult ValidationResult { get; set; } = new ModelValidationResult(false, null, null);


        public ConfirmEmailModel(
             ILogger<RegisterModel> logger,
             UserManager<User> userManager,
             IEmailSender emailSender)
        {
            _logger = logger;
            _userManager = userManager;
            _emailSender = emailSender;
        }
        public async Task<IActionResult> OnGet(Guid userId, string code, Uri returnUrl)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                return Page();
            }
            else
            { 
                //Here I simple use Validation Error to show error from Identity.
                //Could be a seperate modal or separate page
                var identityErrors = result.Errors.Select(e => e.Description).ToList();
                ValidationResult = new ModelValidationResult(true, identityErrors, null);


                //Send another email
                var userIdNew = await _userManager.GetUserIdAsync(user);
                var codeNew = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                codeNew = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { userId = userId, code = code, returnUrl = returnUrl },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                return Page();
            }
        }
    }
}
