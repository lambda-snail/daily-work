using Microsoft.AspNetCore.Identity;

namespace Server.Features.User;

public class ApplicationUser
{
    public Guid Id { get; set; }
    
    /// <summary>
    /// The ObjectId in entra, not to be confused with Id which is the id in the application database.
    /// </summary>
    public Guid ObjectId { get; set; }
    public string Email { get; set; }
    public string DisplayName { get; set; }
}