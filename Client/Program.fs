module Main
    open Client
    open System.ServiceModel

    [<EntryPoint>]
    let main argv = 

        let ralphId = 
            try
                AccountService.AddAccount("Ralph", 0M)
            with 
            | :? FaultException<ExceptionDetail> as ex->
                printfn "%A" ex.Detail.InnerException
                -1

        let bobId =
            let result =
                async { return! AccountServiceAsync.AddAccountAsync("Bob", 0M) |> Async.AwaitTask }
                |> Async.Catch
                |> Async.RunSynchronously

            match result with
            | Choice1Of2 result -> result
            | Choice2Of2 exn ->
                printfn "%A" exn
                -1

        printfn "Ralph %d" ralphId
        printfn "Bob %d" bobId

        0
