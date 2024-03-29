use [fi.retorch.com]
/*
   Tuesday, February 28, 20177:40:46 PM
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
CREATE TABLE dbo.UserSettings
	(
	UserId nvarchar(36) NOT NULL,
	SettingId int NOT NULL,
	Setting nvarchar(100) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.UserSettings ADD CONSTRAINT
	PK_UserSettings PRIMARY KEY CLUSTERED 
	(
	UserId,
	SettingId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.UserSettings SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.UserSettings', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.UserSettings', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.UserSettings', 'Object', 'CONTROL') as Contr_Per 