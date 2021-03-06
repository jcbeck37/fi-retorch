/*
   Monday, February 13, 201711:05:56 PM
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
ALTER TABLE dbo.Reminders
	DROP CONSTRAINT DF_Reminders_IsCredit
GO
ALTER TABLE dbo.Reminders
	DROP CONSTRAINT DF_Reminders_Sequence
GO
CREATE TABLE dbo.Tmp_Reminders
	(
	Id int NOT NULL IDENTITY (1, 1),
	AccountId int NOT NULL,
	CategoryId int NULL,
	ScheduleId int NOT NULL,
	TypeId int NULL,
	Name nvarchar(100) NOT NULL,
	Amount decimal(18, 2) NULL,
	Rate decimal(18, 4) NULL,
	IsCredit bit NOT NULL,
	NextDate datetime NOT NULL,
	LastDate datetime NULL,
	Sequence int NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_Reminders SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.Tmp_Reminders ADD CONSTRAINT
	DF_Reminders_IsCredit DEFAULT ((0)) FOR IsCredit
GO
ALTER TABLE dbo.Tmp_Reminders ADD CONSTRAINT
	DF_Reminders_Sequence DEFAULT ((0)) FOR Sequence
GO
SET IDENTITY_INSERT dbo.Tmp_Reminders ON
GO
IF EXISTS(SELECT * FROM dbo.Reminders)
	 EXEC('INSERT INTO dbo.Tmp_Reminders (Id, AccountId, CategoryId, ScheduleId, TypeId, Name, Amount, Rate, IsCredit, NextDate, LastDate, Sequence)
		SELECT Id, AccountId, CategoryId, ScheduleId, TypeId, Name, Amount, Rate, IsCredit, NextDate, LastDate, Sequence FROM dbo.Reminders WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_Reminders OFF
GO
DROP TABLE dbo.Reminders
GO
EXECUTE sp_rename N'dbo.Tmp_Reminders', N'Reminders', 'OBJECT' 
GO
ALTER TABLE dbo.Reminders ADD CONSTRAINT
	PK_Reminders PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT
select Has_Perms_By_Name(N'dbo.Reminders', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.Reminders', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.Reminders', 'Object', 'CONTROL') as Contr_Per 