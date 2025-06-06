using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SpellingTest.Web.Pages
{
    public class LoginModel : PageModel
    {
        public async Task<IActionResult> OnGetAsync(string redirectUri)
        {
            if (string.IsNullOrEmpty(redirectUri))
            {
                redirectUri = Url.Content("~/");
            }

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                Response.Redirect(redirectUri);
            }


            return Challenge(new AuthenticationProperties
            {
                RedirectUri = redirectUri
            }, "oidc");
        }
    }
}
