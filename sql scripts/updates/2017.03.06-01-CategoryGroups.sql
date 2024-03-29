use [fi.retorch.com]
/*
   Monday, March 6, 20179:41:52 PM
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
ALTER TABLE dbo.CategoryGroups
	DROP CONSTRAINT DF_CategoryGroups_IsIncome
GO
CREATE TABLE dbo.Tmp_CategoryGroups
	(
	Id int NOT NULL IDENTITY (1, 1),
	UserId nvarchar(36) NOT NULL,
	Name nvarchar(100) NOT NULL,
	DateCreated datetime NOT NULL,
	TransferType int NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_CategoryGroups SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.Tmp_CategoryGroups ADD CONSTRAINT
	DF_CategoryGroups_IsIncome DEFAULT ((0)) FOR TransferType
GO
SET IDENTITY_INSERT dbo.Tmp_CategoryGroups ON
GO
IF EXISTS(SELECT * FROM dbo.CategoryGroups)
	 EXEC('INSERT INTO dbo.Tmp_CategoryGroups (Id, UserId, Name, DateCreated, TransferType)
		SELECT Id, UserId, Name, DateCreated, CONVERT(int, IsIncome) FROM dbo.CategoryGroups WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_CategoryGroups OFF
GO
DROP TABLE dbo.CategoryGroups
GO
EXECUTE sp_rename N'dbo.Tmp_CategoryGroups', N'CategoryGroups', 'OBJECT' 
GO
ALTER TABLE dbo.CategoryGroups ADD CONSTRAINT
	PK_CategoryGroups PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT
select Has_Perms_By_Name(N'dbo.CategoryGroups', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.CategoryGroups', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.CategoryGroups', 'Object', 'CONTROL') as Contr_Per 