open Service
open System
open System.ServiceModel

let onServiceFaulted eventArgs =
    printfn "%A" eventArgs

let onUnknownMessageReceived (args : UnknownMessageReceivedEventArgs) =
    printfn "%A" args.Message

[<EntryPoint>]
let main argv = 

    let services = [
        StartService<AccountService>("Accounts")
        StartService<AccountServiceAsync>("AccountsAsync")]

    for service in services do
        service.Faulted |> Event.add onServiceFaulted
        service.UnknownMessageReceived |> Event.add onUnknownMessageReceived
    
    printfn "Services are running"
    printfn "Press any key to exit..."

    Console.ReadKey() |> ignore

    for service in services do
        service.Close()

    0 // return an integer exit code