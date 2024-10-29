namespace GP.IdentityEndpoints.Operations

open System
open System.Text
open FsToolkit.ErrorHandling
open FsToolkit.ErrorHandling.Operator.TaskResult
open Microsoft.AspNetCore.Identity
open Microsoft.AspNetCore.WebUtilities
open GP.IdentityEndpoints.Utils

module ConfirmEmail =

    type ConfirmEmailRequest =
        { userId: Guid
          ConfirmEmailToken: string }

    type ConfirmEmailError =
        | UserNotFound
        | FailedToConfirm of error: string

    let confirm<'TUser when 'TUser: not struct and 'TUser: null and 'TUser :> IdentityUser<'TUser>>
        (userManager: UserManager<'TUser>)
        (request: ConfirmEmailRequest)
        =
        userManager.FindByIdAsync(request.userId.ToString())
        |> TaskResult.ofTask
        |> TaskResult.bindRequireNotNull UserNotFound
        >>= fun user ->
                if not user.EmailConfirmed then
                    userManager.ConfirmEmailAsync(
                        user,
                        Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.ConfirmEmailToken))
                    )
                    |> mapIdentityResult
                    |> TaskResult.mapError FailedToConfirm
                    |> TaskResult.map (fun _  -> user)
                else
                    TaskResult.ok user
