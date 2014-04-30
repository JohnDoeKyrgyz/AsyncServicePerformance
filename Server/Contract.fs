module Contract

    open System
    open System.Collections.Generic
    open System.Runtime.Serialization
    open System.ServiceModel
    open System.Threading.Tasks

    [<CLIMutable>]
    type Account = {
        Name: string
        Id: int
        CreatedOn: DateTime
        LastUpdatedOn: DateTime
        Balance: decimal }

    [<CLIMutable>]
    type Customer = {
        Id: int
        Name: string }

    [<CLIMutable>]
    type Transaction = {
        Id: int
        From: Customer
        To: Customer
        Amount: decimal
        Time: DateTime }

    type ITransactionService = 
        [<OperationContract>]
        abstract member Transfer : fromCustomerId:int * toCustomerId:int * amount:decimal -> Transaction
        [<OperationContract>]
        abstract member ReconcileTransactions : unit -> Task

    [<ServiceContract>]
    type IAccountService =
        [<OperationContract>]
        abstract member AddAccount : name:string * startingBalance:decimal -> int
        [<OperationContract>]
        abstract member ListAccounts : unit -> IEnumerable<Account>
        [<OperationContract>]
        abstract member ListTransactions : customerId:int -> IEnumerable<Transaction>
        
    type ITransactionServiceAsync = 
        [<OperationContract>]
        abstract member TransferAsync : fromCustomerId:int * toCustomerId:int * amount:decimal -> Task<Transaction>
        [<OperationContract>]
        abstract member ReconcileTransactionsAsync : unit -> Task

    [<ServiceContract>]
    type IAccountServiceAsync =
        [<OperationContract>]
        abstract member AddAccountAsync : name:string * startingBalance:decimal -> Task<int>
        [<OperationContract>]
        abstract member ListAccountsAsync : unit -> Task<IEnumerable<Account>>
        [<OperationContract>]
        abstract member ListTransactionsAsync : customerId:int -> Task<IEnumerable<Transaction>>