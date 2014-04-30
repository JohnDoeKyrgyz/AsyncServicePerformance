module Data
    open System
    open System.Collections.Generic
    open System.ComponentModel.DataAnnotations
    open System.ComponentModel.DataAnnotations.Schema
    open System.Data.Entity
    open System.Data.Entity.ModelConfiguration.Conventions
    open System.Linq

    [<CLIMutable>]
    type Customer = {
        [<Key>]
        [<DatabaseGenerated(DatabaseGeneratedOption.Identity)>]
        Id: int
        Name: string
        Balance: decimal
        LastUpdated: DateTime
        CreatedOn: DateTime
        [<Timestamp>]
        RowVersion: byte[] }

    [<CLIMutable>]
    type Transfer = {
        [<Key>]
        [<DatabaseGenerated(DatabaseGeneratedOption.Identity)>]
        Id: int
        FromCustomerId: int
        ToCustomerId: int        
        Time: DateTime
        Amount: decimal }

    type BankContext() =
        inherit DbContext("BankContext")

        [<DefaultValue>]
        val mutable customers : IDbSet<Customer>
        [<DefaultValue>]
        val mutable transfers : IDbSet<Transfer>

        member this.Customers with get() = this.customers and set v = this.customers <- v
        
        member this.Transfers with get() = this.transfers and set v = this.transfers <- v

        override this.OnModelCreating(modelBuilder : DbModelBuilder) =
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>()

    let Add<'T when 'T : not struct> entity entitySetGetter =
        use context = new BankContext()
        let dbSet : IDbSet<'T> = entitySetGetter context
        let attachedEntity = dbSet.Add(entity)
        let saveCount = context.SaveChanges()
        saveCount, attachedEntity

    let AddAsync<'T when 'T : not struct> entity entitySetGetter =
        use context = new BankContext()
        let dbSet : IDbSet<'T> = entitySetGetter context
        let attachedEntity = dbSet.Add(entity)
        let saveCount = context.SaveChangesAsync() |> Async.AwaitTask
        saveCount, attachedEntity

    let Query<'T when 'T : not struct> entitySetGetter =
        use context = new BankContext()
        let dbSet : IDbSet<'T> = entitySetGetter context        
        dbSet :> IQueryable<'T>

    let EntityAction entityBuilder entitySetGetter action =
        let entity = entityBuilder()
        action entity entitySetGetter