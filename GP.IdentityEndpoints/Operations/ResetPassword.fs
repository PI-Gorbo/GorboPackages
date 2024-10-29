namespace GP.IdentityEndpoints.Operations

open System.Text
open FsToolkit.ErrorHandling
open FsToolkit.ErrorHandling.Operator.TaskResult
open Microsoft.AspNetCore.Identity
open Microsoft.AspNetCore.WebUtilities

module ResetPassword =

    type ResetPasswordRequest =
        { email: string
          password: string
          token: string }

    type ResetPasswordError =
        | UserNotFound
        | FailedToSetPassword of error: string

    let reset<'TUser when 'TUser: not struct and 'TUser: null>
        (userManager: UserManager<'TUser>)
        (request: ResetPasswordRequest)
        =
        userManager.FindByEmailAsync(request.email)
        |> TaskResult.ofTask
        |> TaskResult.bindRequireNotNull UserNotFound
        >>= fun user -> // Reset the user's password.
                userManager.ResetPasswordAsync(
                    user,
                    Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.token)),
                    request.password
                )
                |> TaskResult.ofTask
        >>= fun identityResult -> // Map Identity Error if the reset failed.
                if identityResult.Succeeded then
                    TaskResult.ok ()
                else
                    TaskResult.error (
                        FailedToSetPassword(
                            identityResult.Errors
                            |> Seq.map (fun err -> err.Description)
                            |> Seq.fold (fun item agg -> agg + ", " + item) ""
                        )
                    )
