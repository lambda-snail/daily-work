using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace Server.Features.User;

public class UserManager
{
    private readonly UserRepository _userRepository;
    //private readonly AuthenticationStateProvider _authenticationStateProvider;

    private ApplicationUser? _user = null;
    
    public UserManager(UserRepository userRepository, AuthenticationStateProvider authenticationStateProvider)
    {
        _userRepository = userRepository;
        //_authenticationStateProvider = authenticationStateProvider;
    }

    /// <summary>
    /// Gets the user data for the current user, or creates it if the user just signed up.
    /// The result is cached so we only hit the database at most two times per session.
    /// </summary>
    public async Task<ApplicationUser> GetOrCreateCurrentUser(ClaimsPrincipal principal)
    {
        if (_user is not null)
        {
            return _user;
        }
        
        var userId = principal.Claims.FirstOrDefault(c => string.Equals(c.Type, "http://schemas.microsoft.com/identity/claims/objectidentifier", StringComparison.InvariantCultureIgnoreCase));
        ArgumentNullException.ThrowIfNull(userId); // Not logged in TODO: Add custom exception and handle gracefully
        
        ApplicationUser? user = await _userRepository.GetByObjectId(Guid.Parse(userId.Value));
        if (user is null)
        {
            var name = principal.Claims.FirstOrDefault(c => string.Equals(c.Type, "name", StringComparison.InvariantCultureIgnoreCase));
            var email = principal.Claims.FirstOrDefault(c => string.Equals(c.Type, "preferred_username", StringComparison.InvariantCultureIgnoreCase));
            
            ArgumentNullException.ThrowIfNull(name);
            ArgumentNullException.ThrowIfNull(email);

            user = new ApplicationUser
            {
                ObjectId = Guid.Parse(userId.Value),
                DisplayName = name.Value,
                Email = email.Value,
            };
            
            await _userRepository.Create(user);
        }

        _user = user;
        return user;
    }
}