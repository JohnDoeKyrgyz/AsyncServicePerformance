module Client

    open System.ServiceModel
    open Contract

    let createServiceAddress endpointName = new EndpointAddress( sprintf "http://localhost:8080/%s" endpointName )    

    let binding = new BasicHttpBinding()    

    type AccountServiceClient() =
        inherit ClientBase<IAccountService>( binding, createServiceAddress "Accounts" )

        interface IAccountService
            with
                override this.AddAccount(name, initialBalance) = base.Channel.AddAccount(name, initialBalance)
                override this.ListAccounts() = base.Channel.ListAccounts()
                override this.ListTransactions(customerId) = base.Channel.ListTransactions(customerId)

    let AccountService = new AccountServiceClient() :> IAccountService

    type AccountServiceAsyncClient() =
        inherit ClientBase<IAccountServiceAsync>( binding, createServiceAddress "AccountsAsync" )

        interface IAccountServiceAsync
            with
                override this.AddAccountAsync(name, initialBalance) = base.Channel.AddAccountAsync(name, initialBalance)
                override this.ListAccountsAsync() = base.Channel.ListAccountsAsync()
                override this.ListTransactionsAsync(customerId) = base.Channel.ListTransactionsAsync(customerId)

    let AccountServiceAsync = new AccountServiceAsyncClient() :> IAccountServiceAsync

