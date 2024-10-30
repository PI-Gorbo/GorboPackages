namespace GP.IdentityEndpoints

open FsToolkit.ErrorHandling

module Email =

    type IIdentityEmailSender<'DTO> =
        abstract member SendEmail: 'DTO -> TaskResult<unit, string>

    type ConfirmEmailDto<'TUser> = { user: 'TUser; token: string }
    type IConfirmEmailSender<'TUser> =
        inherit IIdentityEmailSender<ConfirmEmailDto<'TUser>>

    type ResetPasswordDto<'TUser> = { user: 'TUser; token: string }
    type IResetPasswordSender<'TUser> =
        inherit IIdentityEmailSender<ResetPasswordDto<'TUser>>