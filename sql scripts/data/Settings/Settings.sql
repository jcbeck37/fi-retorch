use [fi.retorch.com]

SET IDENTITY_INSERT [dbo].[Settings] ON

INSERT INTO [dbo].[Settings] (Id, [Name]) VALUES(1, 'Dashboard Start Date')
INSERT INTO [dbo].[Settings] (Id, [Name]) VALUES(2, 'Dashboard End Date')

SET IDENTITY_INSERT [dbo].[Settings] OFF