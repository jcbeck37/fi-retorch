use [fi.retorch.com]
/*
   Tuesday, February 28, 20177:39:27 PM
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
CREATE TABLE dbo.Settings
	(
	Id int NOT NULL IDENTITY (1, 1),
	Name varchar(100) NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Settings ADD CONSTRAINT
	PK_Settings PRIMARY KEY CLUSTERED 
	(
	Id,
	Name
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.Settings SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.Settings', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.Settings', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.Settings', 'Object', 'CONTROL') as Contr_Per 