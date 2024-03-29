use [fi.retorch.com]
/*
   Sunday, January 29, 20171:06:01 PM
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
CREATE TABLE dbo.CategoryGroups
	(
	Id int NOT NULL IDENTITY (1, 1),
	UserId nvarchar(36) NOT NULL,
	Name nvarchar(100) NOT NULL,
	DateCreated datetime NOT NULL,
	IsIncome bit NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.CategoryGroups ADD CONSTRAINT
	DF_CategoryGroups_IsIncome DEFAULT 0 FOR IsIncome
GO
ALTER TABLE dbo.CategoryGroups ADD CONSTRAINT
	PK_CategoryGroups PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.CategoryGroups SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.CategoryGroups', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.CategoryGroups', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.CategoryGroups', 'Object', 'CONTROL') as Contr_Per 