using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using DotVVM.Framework.Hosting;
using DotVVM.Framework.ViewModel.Validation;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using WebGallery.BL.Services;

namespace WebGallery.ViewModels.Authentication
{
    public class RegisterViewModel : NonAuthenticatedMasterPage, IValidatableObject
    {
        private readonly UserService userService;


        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }

        public RegisterViewModel(UserService userService)
        {
            this.userService = userService;
        }


        public async Task Register()
        {

            var identityResult = await userService.RegisterAsync(UserName, Password);
            if (identityResult.Succeeded)
            {
                await SignIn();
            }
            else
            {
                var modelErrors = ConvertIdentityErrorsToModelErrors(identityResult);
                Context.ModelState.Errors.AddRange(modelErrors);
                Context.FailOnInvalidModelState();
            }

            Context.RedirectToRoute("Default", allowSpaRedirect: false);
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Password != ConfirmPassword)
            {
                yield return new ValidationResult("Passwords do not match.", new[] { nameof(ConfirmPassword) });
            }
        }

        private async Task SignIn()
        {
            var claimsIdentity = await userService.SignInAsync(UserName, Password);
            await Context.GetAuthentication().SignInAsync(IdentityConstants.ApplicationScheme, new ClaimsPrincipal(claimsIdentity));
        }

        private IEnumerable<ViewModelValidationError> ConvertIdentityErrorsToModelErrors(IdentityResult identityResult)
        {
            return identityResult.Errors.Select(identityError => new ViewModelValidationError
            {
                ErrorMessage = identityError.Description,
                PropertyPath = nameof(UserName)
            });
        }
    }
}
