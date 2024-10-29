namespace GP.IdentityEndpoints

open GP.IdentityEndpoints.Email
open Microsoft.Extensions.DependencyInjection

module Extensions =
    
    type IServiceCollection with
        member this.RegisterIdentityEmailSenders<'TUser,'TVerifyEmailSender,'TResetPasswordSender
            when 'TVerifyEmailSender :> IVerifyEmailSender<'TUser>
            and 'TVerifyEmailSender : not struct
            and 'TResetPasswordSender :> IResetPasswordSender<'TUser>
            and 'TResetPasswordSender : not struct>
            () =
            this.AddScoped<IVerifyEmailSender<'TUser>,'TVerifyEmailSender>() |> ignore
            this.AddScoped<IResetPasswordSender<'TUser>, 'TResetPasswordSender>() |> ignore
        

