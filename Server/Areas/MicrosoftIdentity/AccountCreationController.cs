using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;

namespace Server.Areas.MicrosoftIdentity;

//[NonController]
[AllowAnonymous]
[Area("MicrosoftIdentity")]
[Route("[area]/[controller]/[action]")]
public class AccountController : Controller //: Microsoft.Identity.Web.UI.Areas.MicrosoftIdentity.Controllers.AccountController
{
    // public IdentityController(IOptionsMonitor<MicrosoftIdentityOptions> microsoftIdentityOptionsMonitor) : base(microsoftIdentityOptionsMonitor)
    // {
    // }
    
    [HttpGet("{scheme?}")]
    public IActionResult SignUp([FromRoute] string? scheme)
    {
        scheme ??= OpenIdConnectDefaults.AuthenticationScheme;
        var parameters = new Dictionary<string, object?>
        {
            { "prompt", "create" },
        };
        OAuthChallengeProperties oAuthChallengeProperties = new OAuthChallengeProperties(new Dictionary<string, string?>(), parameters);
        oAuthChallengeProperties.RedirectUri = Url.Content("~/");

        return Challenge(
            oAuthChallengeProperties,
            scheme);
    }
}