namespace GP.IdentityEndpoints.Operations

open System
open System.Security.Claims
open Microsoft.AspNetCore.Authentication
open Microsoft.AspNetCore.Authentication.Cookies
open Microsoft.AspNetCore.Http
open FsToolkit.ErrorHandling

module CookieOperations =

    let AttachCookieToContext (httpContext: HttpContext) (userId: Guid) (additionalClaims : Claim seq) =
        taskResult {
            return!
                httpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    ClaimsPrincipal(
                        ClaimsIdentity(
                            (additionalClaims |> Seq.append [ Claim("USERID", userId.ToString()) ]),
                            CookieAuthenticationDefaults.AuthenticationScheme
                        )
                    )
                )
        }
        
    let RemoveCookieFromContext (httpContext : HttpContext) = httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme)
