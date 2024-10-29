namespace GP.IdentityEndpoints.Operations

open System.Threading.Tasks
open FsToolkit.ErrorHandling
open FsToolkit.ErrorHandling.Operator.TaskResult
open Microsoft.AspNetCore.Identity

module EmailPasswordLogin =
    type LoginRequest = { Email: string; Password: string }
    type LoginFailure = InvalidCredentiails

    let login<'TUser when 'TUser: (new: unit -> 'TUser) and 'TUser: not struct and 'TUser: null>
        (dto: LoginRequest)
        (userManager: UserManager<'TUser>)
        (signInManager: SignInManager<'TUser>) : Task<Result<'TUser,LoginFailure>>
        =

        userManager.FindByEmailAsync(dto.Email)
        |> TaskResult.ofTask
        |> TaskResult.bindRequireNotNull InvalidCredentiails
        >>= fun user ->
            signInManager.CheckPasswordSignInAsync(user, dto.Password, false)
            |> TaskResult.ofTask
            |> TaskResult.bind (fun result -> if result.Succeeded then TaskResult.ok user else TaskResult.error InvalidCredentiails)
