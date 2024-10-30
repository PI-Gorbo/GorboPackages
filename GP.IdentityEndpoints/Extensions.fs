namespace GP.IdentityEndpoints

open GP.IdentityEndpoints.Email
open Microsoft.Extensions.DependencyInjection

module Extensions =
    
    type IServiceCollection with
        member this.RegisterIdentityEmailSenders<'TUser,'TConfirmEmailSender,'TResetPasswordSender
            when 'TConfirmEmailSender :> IConfirmEmailSender<'TUser>
            and 'TConfirmEmailSender : not struct
            and 'TResetPasswordSender :> IResetPasswordSender<'TUser>
            and 'TResetPasswordSender : not struct>
            () =
            this.AddScoped<IConfirmEmailSender<'TUser>,'TConfirmEmailSender>() |> ignore
            this.AddScoped<IResetPasswordSender<'TUser>, 'TResetPasswordSender>() |> ignore
        

