module Service
    open System.ServiceModel
    open Contract

    [<Sealed>]
    type AccountService =
        interface IAccountService

    [<Sealed>]
    type AccountServiceAsync =
        interface IAccountServiceAsync

    val StartService<'TService> : string -> ServiceHost