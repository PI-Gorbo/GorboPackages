namespace GP.IdentityEndpoints.Operations

open System
open System.Threading.Tasks
open FsToolkit.ErrorHandling
open Microsoft.AspNetCore.Identity
open FsToolkit.ErrorHandling.Operator.TaskResult
open GP.IdentityEndpoints.Utils
module RegisterEndpoint =

    type RegisterDto =
        { Username: string
          Email: string
          Password: string }

    type RegisterFailure =
        | EmailAlreadyRegistered
        | UsernameAlreadyRegistered
        | PasswordInvalid of reason: string
        | GeneralFailure of error: string

    let Register<'TUser when 'TUser: (new: unit -> 'TUser) and 'TUser: not struct and 'TUser: null>
        (dto: RegisterDto)
        (userManager: UserManager<'TUser>)
        (modifyUser: 'TUser -> 'TUser)
        : TaskResult<'TUser, RegisterFailure> =
        let user = new 'TUser()

        let findByEmail () =
            dto.Email
            |> userManager.FindByEmailAsync
            |> TaskResult.ofTask
            |> TaskResult.bindRequireNotNull EmailAlreadyRegistered
            |> TaskResult.ignore

        let findByName () =
            dto.Username
            |> userManager.FindByNameAsync
            |> TaskResult.ofTask
            |> TaskResult.bindRequireNotNull UsernameAlreadyRegistered
            |> TaskResult.ignore

        let verifyPassword (user: 'TUser) (password: string) : TaskResult<unit, RegisterFailure> =
            userManager.PasswordValidators
            |> Seq.fold
                (fun result passwordValidator ->
                    result
                    >>= (fun _ ->
                        passwordValidator.ValidateAsync(userManager, user, password)
                        |> mapIdentityResult
                        |> TaskResult.mapError (fun err -> PasswordInvalid(err))))
                (TaskResult.ok ())
        
        let modifiedUser = modifyUser(user)
            
        findByEmail ()
        >>= findByName
        >>= (fun _ -> verifyPassword modifiedUser dto.Password)
        >>= (fun _ ->
            userManager.SetEmailAsync(modifiedUser, dto.Email)
            |> TaskResult.ofTask
            |> (TaskResult.mapError (fun err -> GeneralFailure(err)))
            |> TaskResult.ignore)
        >>= (fun _ ->
            userManager.SetUserNameAsync(modifiedUser, dto.Username)
            |> TaskResult.ofTask
            |> TaskResult.mapError (fun err -> GeneralFailure(err))
            |> TaskResult.ignore)
        >>= (fun _ ->
            userManager.CreateAsync(modifiedUser, dto.Password)
            |> mapIdentityResult
            |> TaskResult.mapError (fun err -> GeneralFailure(err)))
        >>= (fun _ -> TaskResult.ok user)
