namespace GP.IdentityEndpoints

open FsToolkit.ErrorHandling

module Email =

    type IIdentityEmailSender<'DTO> =
        abstract member SendEmail: 'DTO -> TaskResult<unit, string>

    type VerifyEmailDto<'TUser> = { user: 'TUser; token: string }
    type IVerifyEmailSender<'TUser> =
        inherit IIdentityEmailSender<VerifyEmailDto<'TUser>>

    type ResetPasswordDto<'TUser> = { user: 'TUser; token: string }
    type IResetPasswordSender<'TUser> =
        inherit IIdentityEmailSender<ResetPasswordDto<'TUser>>