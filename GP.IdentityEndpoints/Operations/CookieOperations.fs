namespace GP.IdentityEndpoints.Operations

open System
open System.Security.Claims
open Microsoft.AspNetCore.Authentication
open Microsoft.AspNetCore.Authentication.Cookies
open Microsoft.AspNetCore.Http
open FsToolkit.ErrorHandling

module CookieOperations =

    let AttachCookieToContext (httpContext: HttpContext) (userId: Guid) (additionalClaims : Claim seq) =
        httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            ClaimsPrincipal(
                ClaimsIdentity(
                    (additionalClaims |> Seq.append [ Claim("USERID", userId.ToString()) ]),
                    CookieAuthenticationDefaults.AuthenticationScheme
                )
            )
        )
        
    let RemoveCookieFromContext (httpContext : HttpContext) = httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme)
    
    type ClaimsPrincipal with
        member this.GetUserId () =
            this.Claims
            |> Seq.tryFind (fun c -> c.Type = "USERID")
            |> function
                | Some foundClaim ->
                    try
                        Ok(Guid.Parse(foundClaim.Value))
                    with
                    | ex -> Error()
                | None -> Error()
