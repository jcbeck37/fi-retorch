/*
   Saturday, March 11, 201711:30:28 AM
   User: 
   Server: .\SQLEXPRESS
   Database: fi.retorch.com
   Application: 
*/

/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Transactions
	DROP CONSTRAINT DF_Transactions_IsCredit
GO
ALTER TABLE dbo.Transactions
	DROP CONSTRAINT DF_Transactions_DisplayOrder
GO
CREATE TABLE dbo.Tmp_Transactions
	(
	Id int NOT NULL IDENTITY (1, 1),
	AccountId int NOT NULL,
	CategoryId int NULL,
	Name nvarchar(200) NOT NULL,
	DisplayDate datetime NOT NULL,
	Amount decimal(18, 2) NOT NULL,
	IsCredit bit NOT NULL,
	DateCreated datetime NOT NULL,
	DatePosted datetime NULL,
	Sequence int NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_Transactions SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.Tmp_Transactions ADD CONSTRAINT
	DF_Transactions_IsCredit DEFAULT ((0)) FOR IsCredit
GO
ALTER TABLE dbo.Tmp_Transactions ADD CONSTRAINT
	DF_Transactions_DisplayOrder DEFAULT ((0)) FOR Sequence
GO
SET IDENTITY_INSERT dbo.Tmp_Transactions ON
GO
IF EXISTS(SELECT * FROM dbo.Transactions)
	 EXEC('INSERT INTO dbo.Tmp_Transactions (Id, AccountId, CategoryId, Name, DisplayDate, Amount, IsCredit, DateCreated, DatePosted, Sequence)
		SELECT Id, AccountId, CategoryId, Name, DisplayDate, Amount, IsCredit, DateCreated, DatePosted, Sequence FROM dbo.Transactions WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_Transactions OFF
GO
DROP TABLE dbo.Transactions
GO
EXECUTE sp_rename N'dbo.Tmp_Transactions', N'Transactions', 'OBJECT' 
GO
ALTER TABLE dbo.Transactions ADD CONSTRAINT
	PK_Transactions PRIMARY KEY CLUSTERED 
	(
	Id,
	AccountId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT
select Has_Perms_By_Name(N'dbo.Transactions', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.Transactions', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.Transactions', 'Object', 'CONTROL') as Contr_Per 