using CSharpFunctionalExtensions;
using Marten;
using Microsoft.AspNetCore.Identity;

namespace GP.MartenIdentity;
public class MartenUserStore<TUser>(IDocumentSession session) :
                                    IUserStore<TUser>
                                    // ,
                                    // IUserClaimStore<TUser>
                                    // IUserLoginStore<TUser>,
                                    // IUserRoleStore<TUser>,
                                    // IUserPasswordStore<TUser>,
                                    // IUserSecurityStampStore<TUser>,
                                    // IUserTwoFactorStore<TUser>,
                                    // IUserPhoneNumberStore<TUser>,
                                    // IUserEmailStore<TUser>,
                                    // IUserLockoutStore<TUser>,
                                    // IUserTwoFactorRecoveryCodeStore<TUser>,
                                    // IUserAuthenticatorKeyStore<TUser>,
                                    // IQueryableUserStore<TUser>,
                                    where TUser : DocumentIdentityUser
{
    public void Dispose() { }
    public Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
    {
        return Result.Try(async () =>
            {
                session.Store(user);
                await session.SaveChangesAsync(cancellationToken);
            }).Finally(outcome =>
            {
                if (outcome.IsSuccess)
                {
                    return IdentityResult.Success;
                }
                return IdentityResult.Failed(new IdentityError() { Code = "CREATE USER ERROR", Description = outcome.Error });
            });
    }

    public Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
    {
        return Result.Try(async () =>
        {
            session.Delete(user.Id);
            await session.SaveChangesAsync(cancellationToken);
        }).Finally(outcome =>
        {
            if (outcome.IsSuccess)
            {
                return IdentityResult.Success;
            }
            return IdentityResult.Failed(new IdentityError() { Code = "CREATE USER ERROR", Description = outcome.Error });
        });
    }


    public Task<TUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        return Result.Try(async () =>
       {
           var parsedSuccessfully = Guid.TryParse(userId, out Guid id);
           if (!parsedSuccessfully)
           {
               return null;
           }
           return await session.LoadAsync<TUser>(id);
       }).Finally(outcome => outcome.IsSuccess ? outcome.Value : null);
    }

    public Task<TUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        return Result.Try(async () =>
        {
            return await session
                .Query<TUser>()
                .SingleOrDefaultAsync(x => x.NormalizedUserName == normalizedUserName, cancellationToken);
        }).Finally(x => x.IsSuccess ? x.Value : null);
    }

    public Task<string?> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user?.NormalizedUserName);
    }

    public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.Id.ToString());
    }

    public Task<string?> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.UserName);
    }

    public Task SetNormalizedUserNameAsync(TUser user, string? normalizedName, CancellationToken cancellationToken)
    {
        user.NormalizedUserName = normalizedName;
        return Task.CompletedTask;
    }

    public Task SetUserNameAsync(TUser user, string? userName, CancellationToken cancellationToken)
    {
        user.UserName = userName;
        return Task.CompletedTask;
    }

    public Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
    {
        return Result.Try(async () =>
        {
            session.Store(user);
            await session.SaveChangesAsync(cancellationToken);
        }).Finally(outcome =>
            outcome.IsSuccess ?
                IdentityResult.Success
                :
                IdentityResult.Failed(new IdentityError() { Code = "UPDATE USER ERROR", Description = outcome.Error })
        );
    }
}