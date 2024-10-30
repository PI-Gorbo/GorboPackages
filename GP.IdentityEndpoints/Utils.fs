namespace GP.IdentityEndpoints

open System.Threading.Tasks
open FluentValidation.Results
open FsToolkit.ErrorHandling
open FsToolkit.ErrorHandling.Operator.TaskResult
open Microsoft.AspNetCore.Identity

module Utils =

    let check<'T, 'E> (check: 'T -> TaskResult<unit, 'E>) (taskResult: TaskResult<'T, 'E>) =
        taskResult
        >>= fun value -> check value |> TaskResult.map (fun _ -> value)


    let ensure<'T, 'E> (predicate: 'T -> bool) (errorMap: 'T -> 'E) (taskResult: TaskResult<'T, 'E>) =
        taskResult
        >>= fun res ->
                if predicate (res) then
                    taskResult
                else
                    TaskResult.error (errorMap (res))


    let mapIdentityResult (res: Task<IdentityResult>) : Task<Result<unit, string>> =
        res
        |> TaskResult.ofTask
        |> ensure
            (fun res -> res.Succeeded)
            (fun res ->
                res.Errors
                |> Seq.map (fun x -> x.Description)
                |> String.concat ", ")
        |> TaskResult.ignore

    let mapValidationResult (taskRes: Task<ValidationResult>) =
        taskRes
        |> TaskResult.ofTask
        |> TaskResult.bind (fun res ->
            if res.IsValid then
                TaskResult.ok ()
            else
                TaskResult.error (
                    res.Errors
                    |> Seq.map (fun err -> err.ErrorMessage)
                    |> String.concat ", "
                ))
