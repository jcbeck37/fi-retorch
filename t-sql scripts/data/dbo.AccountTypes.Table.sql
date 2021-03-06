USE [fi.retorch.com]
GO
SET IDENTITY_INSERT [dbo].[AccountTypes] ON 

INSERT [dbo].[AccountTypes] ([Id], [UserId], [Name], [IsDebt], [DateCreated], [PositiveText], [NegativeText]) VALUES (1, N'ac822f82-26fa-4e8c-bb66-429007a7de82', N'Checking', 0, CAST(N'2017-01-29 18:17:26.543' AS DateTime), N'Deposit', N'Withdraw')
INSERT [dbo].[AccountTypes] ([Id], [UserId], [Name], [IsDebt], [DateCreated], [PositiveText], [NegativeText]) VALUES (2, N'ac822f82-26fa-4e8c-bb66-429007a7de82', N'Savings', 0, CAST(N'2017-01-29 20:40:56.227' AS DateTime), N'Deposit', N'Withdraw')
INSERT [dbo].[AccountTypes] ([Id], [UserId], [Name], [IsDebt], [DateCreated], [PositiveText], [NegativeText]) VALUES (3, N'ac822f82-26fa-4e8c-bb66-429007a7de82', N'Credit Card', 1, CAST(N'2017-01-29 20:46:14.820' AS DateTime), N'Charge', N'Payment/Credit')
INSERT [dbo].[AccountTypes] ([Id], [UserId], [Name], [IsDebt], [DateCreated], [PositiveText], [NegativeText]) VALUES (4, N'ac822f82-26fa-4e8c-bb66-429007a7de82', N'Retirement', 0, CAST(N'2017-01-29 21:42:39.403' AS DateTime), N'Deposit', N'Distribution')
INSERT [dbo].[AccountTypes] ([Id], [UserId], [Name], [IsDebt], [DateCreated], [PositiveText], [NegativeText]) VALUES (5, N'ac822f82-26fa-4e8c-bb66-429007a7de82', N'Investment', 0, CAST(N'2017-02-03 21:43:55.923' AS DateTime), N'Deposit', N'Withdraw')
INSERT [dbo].[AccountTypes] ([Id], [UserId], [Name], [IsDebt], [DateCreated], [PositiveText], [NegativeText]) VALUES (6, N'ac822f82-26fa-4e8c-bb66-429007a7de82', N'Real Estate', 0, CAST(N'2017-02-03 21:44:22.597' AS DateTime), N'Appreciation', N'Depreciation')
INSERT [dbo].[AccountTypes] ([Id], [UserId], [Name], [IsDebt], [DateCreated], [PositiveText], [NegativeText]) VALUES (7, N'ac822f82-26fa-4e8c-bb66-429007a7de82', N'Loan', 1, CAST(N'2017-02-03 21:45:19.300' AS DateTime), N'Charge', N'Payment')
SET IDENTITY_INSERT [dbo].[AccountTypes] OFF
