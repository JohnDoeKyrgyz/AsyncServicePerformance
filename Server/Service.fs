module Service
    open Contract
    open Data

    open System
    open System.Data.Entity
    open System.Collections.Generic
    open System.Linq    
    open System.Threading.Tasks
    open System.ServiceModel
    open System.ServiceModel.Description
    open System.Runtime.Serialization
    
    let loadAccountDto (customer : Customer) = {
        Id = customer.Id; 
        Name = customer.Name; 
        LastUpdatedOn = customer.LastUpdated; 
        CreatedOn = customer.CreatedOn; 
        Balance = customer.Balance}

    let loadTransactionDto (transfer : Transfer) = {
        Id = transfer.Id
        From = {Id = transfer.FromCustomerId; Name = "Bob"}
        To = {Id = transfer.FromCustomerId; Name = "Bob"}
        Amount = transfer.Amount
        Time = transfer.Time }
        
    let createCustomer name initialBalance = 
        let date = DateTime.Now
        {Id = 0; Name = name; Balance = initialBalance; CreatedOn = date; RowVersion = Array.empty; LastUpdated = date}    

    let customers (context : BankContext) = context.Customers
    let transfers (context : BankContext) = context.Transfers

    let listCustomersQuery() = query { for customer in Query customers do yield loadAccountDto customer }    
    let listTransactionsQuery customerId = 
        query {
            for transfer in Query transfers do
                if transfer.FromCustomerId = customerId || transfer.ToCustomerId = customerId then
                    yield loadTransactionDto transfer }

    let addCustomerAction entityAction name initialBalance =
        let customerBuilder() = createCustomer name initialBalance        
        EntityAction customerBuilder customers entityAction    
        
    let addCustomer = addCustomerAction Add
    let addCustomerAsync = addCustomerAction AddAsync

    [<Sealed>]
    [<ServiceBehavior(IncludeExceptionDetailInFaults=true)>]
    type AccountService() =
        interface IAccountService with            
            member this.AddAccount(name, startingBalance) = 
                let saveCount, customer = addCustomer name startingBalance
                customer.Id        
            member this.ListAccounts() = listCustomersQuery() :> IEnumerable<Account>
            member this.ListTransactions customerId = listTransactionsQuery customerId :> IEnumerable<Transaction>

    [<Sealed>]
    [<ServiceBehavior(IncludeExceptionDetailInFaults=true)>]
    type AccountServiceAsync() =
        interface IAccountServiceAsync with
            member this.AddAccountAsync(name, startingBalance) = 
                async {
                    let saveCounter, customer = addCustomerAsync name startingBalance
                    let! saveCount = saveCounter
                    return customer.Id}
                |> Async.StartAsTask
            member this.ListAccountsAsync() = listCustomersQuery().ToListAsync().ContinueWith( fun (t : Task<List<Account>>) -> t.Result :> IEnumerable<Account> )
            member this.ListTransactionsAsync customerId = 
                (listTransactionsQuery customerId).ToListAsync().ContinueWith( fun (t : Task<List<Transaction>>) -> t.Result :> IEnumerable<Transaction> )

    let StartService<'TService> endpointName =
        let uri = new Uri(sprintf "http://localhost:8080/%s" endpointName)
        let serviceHost = new ServiceHost(typeof<'TService>, uri)

        //allow meta data publishing
        let metadataBehavior = new ServiceMetadataBehavior(HttpGetEnabled = true)
        metadataBehavior.MetadataExporter.PolicyVersion <- PolicyVersion.Policy15
        serviceHost.Description.Behaviors.Add(metadataBehavior)

        serviceHost.Open()

        serviceHost