USE [fi.retorch.com]
GO
SET IDENTITY_INSERT [dbo].[ReminderTypes] ON 

INSERT [dbo].[ReminderTypes] ([Id], [Name], [IsActive], [DateCreated]) VALUES (1, N'Recurring Income', 1, CAST(N'2017-02-01 19:59:37.927' AS DateTime))
INSERT [dbo].[ReminderTypes] ([Id], [Name], [IsActive], [DateCreated]) VALUES (2, N'Recurring Expense', 1, CAST(N'2017-02-01 19:59:46.950' AS DateTime))
INSERT [dbo].[ReminderTypes] ([Id], [Name], [IsActive], [DateCreated]) VALUES (3, N'Payment', 1, CAST(N'2017-02-01 19:59:54.273' AS DateTime))
INSERT [dbo].[ReminderTypes] ([Id], [Name], [IsActive], [DateCreated]) VALUES (4, N'Interest', 1, CAST(N'2017-02-01 20:00:02.450' AS DateTime))
SET IDENTITY_INSERT [dbo].[ReminderTypes] OFF
