using System.Security;
using Microsoft.AspNetCore.Identity;

namespace GP.MartenIdentity;
public class DocumentIdentityUser : IdentityUser<Guid>
{
    public List<string> RoleClaims { get; set; } = [];
}
