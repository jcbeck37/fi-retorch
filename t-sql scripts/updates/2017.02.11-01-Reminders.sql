/*
   Saturday, February 11, 20179:20:32 PM
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
ALTER TABLE dbo.Reminders ADD
	Sequence int NOT NULL CONSTRAINT DF_Reminders_Sequence DEFAULT 0
GO
ALTER TABLE dbo.Reminders SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.Reminders', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.Reminders', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.Reminders', 'Object', 'CONTROL') as Contr_Per 