USE [fi.retorch.com]
GO
--SET IDENTITY_INSERT [dbo].[DefaultAccountTypes] ON 

INSERT [dbo].[DefaultAccountTypes] ( [Name], [IsDebt], [DateCreated], [PositiveText], [NegativeText]) VALUES (N'Checking', 0, getDate(), N'Deposit', N'Withdraw')
INSERT [dbo].[DefaultAccountTypes] ( [Name], [IsDebt], [DateCreated], [PositiveText], [NegativeText]) VALUES (N'Savings', 0, getDate(), N'Deposit', N'Withdraw')
INSERT [dbo].[DefaultAccountTypes] ( [Name], [IsDebt], [DateCreated], [PositiveText], [NegativeText]) VALUES (N'Credit Card', 1, getDate(), N'Charge', N'Payment/Credit')
INSERT [dbo].[DefaultAccountTypes] ( [Name], [IsDebt], [DateCreated], [PositiveText], [NegativeText]) VALUES (N'Loan', 1, getDate(), N'Charge', N'Payment')
INSERT [dbo].[DefaultAccountTypes] ( [Name], [IsDebt], [DateCreated], [PositiveText], [NegativeText]) VALUES (N'Real Estate', 0, getDate(), N'Appreciation', N'Depreciation')
INSERT [dbo].[DefaultAccountTypes] ( [Name], [IsDebt], [DateCreated], [PositiveText], [NegativeText]) VALUES (N'Other Assets', 0, getDate(), N'Appreciation', N'Depreciation')
INSERT [dbo].[DefaultAccountTypes] ( [Name], [IsDebt], [DateCreated], [PositiveText], [NegativeText]) VALUES (N'Retirement', 0, getDate(), N'Deposit', N'Distribution')
INSERT [dbo].[DefaultAccountTypes] ( [Name], [IsDebt], [DateCreated], [PositiveText], [NegativeText]) VALUES (N'Investment', 0, getDate(), N'Deposit', N'Withdraw')

--SET IDENTITY_INSERT [dbo].[DefaultAccountTypes] OFF
