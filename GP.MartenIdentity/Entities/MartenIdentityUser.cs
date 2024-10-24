using System.Security;
using Microsoft.AspNetCore.Identity;
using Polly.Retry;

namespace GP.MartenIdentity;

public class MartenIdentityUser : IdentityUser<Guid>
{
    public List<MartenIdentityClaim> Claims { get; set; } = [];
    public List<MartenIdentityUserLogin> Logins { get; set; } = [];
    public List<MartenIdentityRole> Roles { get; set; } = [];
    public List<MartenIdentityRecoveryCode> RecoveryCodes { get; set; } = [];

    public List<MartenIdentityUserToken> Tokens { get; set; } = [];
}
