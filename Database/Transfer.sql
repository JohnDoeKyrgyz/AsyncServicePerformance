CREATE TABLE [dbo].[Transfer]
(
	[Id] INT IDENTITY NOT NULL PRIMARY KEY,
	[FromCustomerId] INT NOT NULL,
	[ToCustomerId] INT NOT NULL,
	Amount DECIMAL(18, 2) NOT NULL, 
    [Time] DATETIME2 NOT NULL, 
    CONSTRAINT [FK_Transfer_FromCustomer] FOREIGN KEY ([FromCustomerId]) REFERENCES [Customer]([Id]),
	CONSTRAINT [FK_Transfer_ToCustomer] FOREIGN KEY ([ToCustomerId]) REFERENCES [Customer]([Id])
)
