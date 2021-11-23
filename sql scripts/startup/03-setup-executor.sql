USE [fi.retorch.com]
GO
CREATE role [db_executor]
GO
GRANT EXECUTE TO db_executor
GO
EXEC sp_addrolemember 'db_executor','dbUser'
GO