USE [fi.retorch.com]
GO

TRUNCATE TABLE ReminderSchedules

SET IDENTITY_INSERT [dbo].[ReminderSchedules] ON 

INSERT [dbo].[ReminderSchedules] ([Id], [Name], [IsActive], [DateCreated]) VALUES (1, N'Daily', 1, CAST(N'2017-02-01 20:18:29.960' AS DateTime))
INSERT [dbo].[ReminderSchedules] ([Id], [Name], [IsActive], [DateCreated]) VALUES (2, N'Weekly', 1, CAST(N'2017-02-01 20:18:36.147' AS DateTime))
INSERT [dbo].[ReminderSchedules] ([Id], [Name], [IsActive], [DateCreated]) VALUES (3, N'Bi-weekly', 1, CAST(N'2017-02-01 20:18:42.567' AS DateTime))
INSERT [dbo].[ReminderSchedules] ([Id], [Name], [IsActive], [DateCreated]) VALUES (4, N'Semi-monthly', 1, CAST(N'2017-02-01 20:19:00.593' AS DateTime))
INSERT [dbo].[ReminderSchedules] ([Id], [Name], [IsActive], [DateCreated]) VALUES (5, N'Monthly', 1, CAST(N'2017-02-01 20:19:08.217' AS DateTime))
INSERT [dbo].[ReminderSchedules] ([Id], [Name], [IsActive], [DateCreated]) VALUES (6, N'Quarterly', 1, CAST(N'2017-02-01 20:19:08.217' AS DateTime))
INSERT [dbo].[ReminderSchedules] ([Id], [Name], [IsActive], [DateCreated]) VALUES (7, N'Semi-annual', 1, CAST(N'2017-02-01 20:19:13.837' AS DateTime))
INSERT [dbo].[ReminderSchedules] ([Id], [Name], [IsActive], [DateCreated]) VALUES (8, N'Annual', 1, CAST(N'2017-02-01 20:19:13.837' AS DateTime))

SET IDENTITY_INSERT [dbo].[ReminderSchedules] OFF
