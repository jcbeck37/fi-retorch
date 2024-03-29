use [fi.retorch.com]
/*
   Sunday, January 29, 20171:51:45 PM
   User: 
   Server: PERCHERON\SQLEXPRESS
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
CREATE TABLE dbo.AccountCategories
	(
	AccountId int NOT NULL,
	CategoryId int NOT NULL,
	IsActive bit NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.AccountCategories ADD CONSTRAINT
	DF_AccountCategories_IsActive DEFAULT 0 FOR IsActive
GO
ALTER TABLE dbo.AccountCategories ADD CONSTRAINT
	PK_AccountCategories PRIMARY KEY CLUSTERED 
	(
	AccountId,
	CategoryId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.AccountCategories SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.AccountCategories', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.AccountCategories', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.AccountCategories', 'Object', 'CONTROL') as Contr_Per 