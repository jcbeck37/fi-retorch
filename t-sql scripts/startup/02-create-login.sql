USE [master]
GO
CREATE LOGIN [dbLogin] WITH PASSWORD=N'PASSWORD', DEFAULT_DATABASE=[fi.retorch.com], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
USE [fi.retorch.com]
GO
CREATE USER [dbUser] FOR LOGIN [dbLogin]
GO
USE [fi.retorch.com]
GO
EXEC sp_addrolemember N'db_datareader', N'dbUser'
GO
USE [fi.retorch.com]
GO
EXEC sp_addrolemember N'db_datawriter', N'dbUser'
GO
