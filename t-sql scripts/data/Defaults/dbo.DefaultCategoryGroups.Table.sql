USE [fi.retorch.com]
GO
--SET IDENTITY_INSERT [dbo].[CategoryGroups] ON 

INSERT [dbo].[DefaultCategoryGroups] ([Name], [DateCreated], [TransferType]) VALUES (N'Working Income', getDate(), 1)
INSERT [dbo].[DefaultCategoryGroups] ([Name], [DateCreated], [TransferType]) VALUES (N'Passive Income', getDate(), 1)
INSERT [dbo].[DefaultCategoryGroups] ([Name], [DateCreated], [TransferType]) VALUES (N'Household Expenses', getDate(), 0)
INSERT [dbo].[DefaultCategoryGroups] ([Name], [DateCreated], [TransferType]) VALUES (N'Optional Expenses', getDate(), 0)
--INSERT [dbo].[DefaultCategoryGroups] ([Name], [DateCreated], [TransferType]) VALUES (N'Rental Expenses', getDate(), 0)

--SET IDENTITY_INSERT [dbo].[CategoryGroups] OFF
