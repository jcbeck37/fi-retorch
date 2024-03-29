USE [fi.retorch.com]
GO
/*
   Friday, February 24, 20178:22:23 PM
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
CREATE TABLE dbo.UserRoles
	(
	UserId nvarchar(36) NOT NULL,
	RoleId nvarchar(36) NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.UserRoles ADD CONSTRAINT
	PK_UserRoles PRIMARY KEY CLUSTERED 
	(
	UserId,
	RoleId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.UserRoles SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.UserRoles', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.UserRoles', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.UserRoles', 'Object', 'CONTROL') as Contr_Per 