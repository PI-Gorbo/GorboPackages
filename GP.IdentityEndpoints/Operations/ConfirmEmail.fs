﻿namespace GP.IdentityEndpoints.Operations

open System
open System.Text
open FsToolkit.ErrorHandling
open FsToolkit.ErrorHandling.Operator.TaskResult
open GP.IdentityEndpoints.Email
open Microsoft.AspNetCore.Http.HttpResults
open Microsoft.AspNetCore.Identity
open Microsoft.AspNetCore.WebUtilities
open GP.IdentityEndpoints.Utils

module ConfirmEmail =

    let send<'TUser when 'TUser: not struct and 'TUser: null>
        (userManager: UserManager<'TUser>)
        (user: 'TUser)
        (emailSender: IConfirmEmailSender<'TUser>)
        =
        userManager.GenerateEmailConfirmationTokenAsync(user)
        |> TaskResult.ofTask
        >>= fun token -> emailSender.SendEmail { user = user; token = token }
 
    type ConfirmEmailRequest =
        { userId: Guid
          ConfirmEmailToken: string }

    type ConfirmEmailError =
        | UserNotFound
        | FailedToConfirm of error: string

    let confirm<'TUser when 'TUser: not struct and 'TUser: null>
        (userManager: UserManager<'TUser>)
        (request: ConfirmEmailRequest)
        =
        userManager.FindByIdAsync(request.userId.ToString())
        |> TaskResult.ofTask
        |> TaskResult.bindRequireNotNull UserNotFound
        >>= fun user ->
            taskResult {
                let! emailIsConfirmed = userManager.IsEmailConfirmedAsync(user)
                if not emailIsConfirmed then
                     return!
                        userManager.ConfirmEmailAsync(
                            user,
                            Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.ConfirmEmailToken))
                        )
                        |> mapIdentityResult
                        |> TaskResult.mapError FailedToConfirm
                        |> TaskResult.map (fun _ -> user)
                else
                    return user
            }