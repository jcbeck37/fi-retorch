use [fi.retorch.com]
/*
   Wednesday, February 1, 20176:54:31 PM
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
CREATE TABLE dbo.ReminderTypes
	(
	Id int NOT NULL IDENTITY (1, 1),
	Name nvarchar(100) NOT NULL,
	IsActive bit NOT NULL,
	DateCreated datetime NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.ReminderTypes ADD CONSTRAINT
	DF_ReminderTypes_IsActive DEFAULT 0 FOR IsActive
GO
ALTER TABLE dbo.ReminderTypes ADD CONSTRAINT
	PK_ReminderTypes PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.ReminderTypes SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.ReminderTypes', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.ReminderTypes', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.ReminderTypes', 'Object', 'CONTROL') as Contr_Per 