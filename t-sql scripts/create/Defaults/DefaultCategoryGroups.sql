/*
   Thursday, February 16, 20178:58:06 PM
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
CREATE TABLE dbo.DefaultCategoryGroups
	(
	Name nvarchar(100) NOT NULL,
	IsIncome bit NOT NULL,
	DateCreated datetime NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.DefaultCategoryGroups ADD CONSTRAINT
	DF_DefaultCategoryGroups_IsIncome DEFAULT (0) FOR IsIncome
GO
ALTER TABLE dbo.DefaultCategoryGroups ADD CONSTRAINT
	DF_DefaultCategoryGroups_DateCreated DEFAULT (getDate()) FOR DateCreated
GO
ALTER TABLE dbo.DefaultCategoryGroups ADD CONSTRAINT
	PK_DefaultCategoryGroups PRIMARY KEY CLUSTERED 
	(
	Name
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.DefaultCategoryGroups SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.DefaultCategoryGroups', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.DefaultCategoryGroups', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.DefaultCategoryGroups', 'Object', 'CONTROL') as Contr_Per 