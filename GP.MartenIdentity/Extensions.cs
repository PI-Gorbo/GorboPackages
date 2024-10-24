using Microsoft.AspNetCore.Identity;
using Marten;
using JasperFx.Core;
namespace GP.MartenIdentity;
public static class Extensions
{
    public static Marten.StoreOptions RegisterIdentityModels<TUser, TRole>(this Marten.StoreOptions opts)
        where TUser : MartenIdentityUser
        where TRole : MartenIdentityRole
    {
        opts.Schema.For<TUser>().Index(x => x.NormalizedEmail, x => { x.IsUnique = true; });
        opts.Schema.For<TRole>();
        return opts;
    }
}