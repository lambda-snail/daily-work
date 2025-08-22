using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Server.Features.User;

public partial class SigninRedirect : ComponentBase
{
    [Inject]
    public AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;

    [Inject] public UserRepository UserRepository { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        ClaimsPrincipal principal = authState.User;
 
        var userId = principal.Claims.FirstOrDefault(c => string.Equals(c.Type, "uid", StringComparison.InvariantCultureIgnoreCase));
        ArgumentNullException.ThrowIfNull(userId);
        
        ApplicationUser? user = await UserRepository.GetByObjectId(Guid.Parse(userId.Value));
        if (user is null)
        {
            var name = principal.Claims.FirstOrDefault(c => string.Equals(c.Type, "name", StringComparison.InvariantCultureIgnoreCase));
            var email = principal.Claims.FirstOrDefault(c => string.Equals(c.Type, "preferred_username", StringComparison.InvariantCultureIgnoreCase));
            
            ArgumentNullException.ThrowIfNull(name);
            ArgumentNullException.ThrowIfNull(email);

            user = new ApplicationUser
            {
                Id = Guid.Parse(userId.Value),
                DisplayName = name.Value,
                Email = email.Value,
            };
            
            await UserRepository.Create(user);
        }

    }
}