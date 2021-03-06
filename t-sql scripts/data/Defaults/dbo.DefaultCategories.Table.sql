USE [fi.retorch.com]
GO
--SET IDENTITY_INSERT [dbo].[DefaultCategories] ON

/* Active Income */
INSERT [dbo].[DefaultCategories] ([GroupName], [Name], [DateCreated]) VALUES(N'Working Income', N'Paychecks', getDate())
INSERT [dbo].[DefaultCategories] ([GroupName], [Name], [DateCreated]) VALUES(N'Working Income', N'Retirement Contribution', getDate())

/* Passive Income */
INSERT [dbo].[DefaultCategories] ([GroupName], [Name], [DateCreated]) VALUES(N'Passive Income', N'Dividends', getDate())
INSERT [dbo].[DefaultCategories] ([GroupName], [Name], [DateCreated]) VALUES(N'Passive Income', N'Investment Gains', getDate())

/* Transfers */
INSERT [dbo].[DefaultCategories] ([GroupName], [Name], [DateCreated]) VALUES(N'Transfers', N'Credit Card Payment', getDate())
INSERT [dbo].[DefaultCategories] ([GroupName], [Name], [DateCreated]) VALUES(N'Transfers', N'Payment / Credit', getDate())

/* Household Expenses */
INSERT [dbo].[DefaultCategories] ([GroupName], [Name], [DateCreated]) VALUES(N'Household Expenses', N'Car Insurance', getDate())
INSERT [dbo].[DefaultCategories] ([GroupName], [Name], [DateCreated]) VALUES(N'Household Expenses', N'Education', getDate())
INSERT [dbo].[DefaultCategories] ([GroupName], [Name], [DateCreated]) VALUES(N'Household Expenses', N'Gas', getDate())
INSERT [dbo].[DefaultCategories] ([GroupName], [Name], [DateCreated]) VALUES(N'Household Expenses', N'Groceries', getDate())
INSERT [dbo].[DefaultCategories] ([GroupName], [Name], [DateCreated]) VALUES(N'Household Expenses', N'Insurance', getDate())
INSERT [dbo].[DefaultCategories] ([GroupName], [Name], [DateCreated]) VALUES(N'Household Expenses', N'Home Maintenance', getDate())
INSERT [dbo].[DefaultCategories] ([GroupName], [Name], [DateCreated]) VALUES(N'Household Expenses', N'Household', getDate())
INSERT [dbo].[DefaultCategories] ([GroupName], [Name], [DateCreated]) VALUES(N'Household Expenses', N'Mortgage / Rent', getDate())
INSERT [dbo].[DefaultCategories] ([GroupName], [Name], [DateCreated]) VALUES(N'Household Expenses', N'Taxes', getDate())
INSERT [dbo].[DefaultCategories] ([GroupName], [Name], [DateCreated]) VALUES(N'Household Expenses', N'Transportation', getDate())
INSERT [dbo].[DefaultCategories] ([GroupName], [Name], [DateCreated]) VALUES(N'Household Expenses', N'Utilities', getDate())

/* Optional Expenses */
INSERT [dbo].[DefaultCategories] ([GroupName], [Name], [DateCreated]) VALUES(N'Optional Expenses', N'Apparel', getDate())
INSERT [dbo].[DefaultCategories] ([GroupName], [Name], [DateCreated]) VALUES(N'Optional Expenses', N'ATM/Cash', getDate())
INSERT [dbo].[DefaultCategories] ([GroupName], [Name], [DateCreated]) VALUES(N'Optional Expenses', N'Cable TV / Internet', getDate())
INSERT [dbo].[DefaultCategories] ([GroupName], [Name], [DateCreated]) VALUES(N'Optional Expenses', N'Car Payments', getDate())
INSERT [dbo].[DefaultCategories] ([GroupName], [Name], [DateCreated]) VALUES(N'Optional Expenses', N'Cell Phone', getDate())
INSERT [dbo].[DefaultCategories] ([GroupName], [Name], [DateCreated]) VALUES(N'Optional Expenses', N'Credit Card Interest', getDate())
INSERT [dbo].[DefaultCategories] ([GroupName], [Name], [DateCreated]) VALUES(N'Optional Expenses', N'Gifts', getDate())
INSERT [dbo].[DefaultCategories] ([GroupName], [Name], [DateCreated]) VALUES(N'Optional Expenses', N'Eating Out', getDate())
INSERT [dbo].[DefaultCategories] ([GroupName], [Name], [DateCreated]) VALUES(N'Optional Expenses', N'Electronics', getDate())
INSERT [dbo].[DefaultCategories] ([GroupName], [Name], [DateCreated]) VALUES(N'Optional Expenses', N'Entertainment', getDate())
INSERT [dbo].[DefaultCategories] ([GroupName], [Name], [DateCreated]) VALUES(N'Optional Expenses', N'Loan Interest', getDate())
INSERT [dbo].[DefaultCategories] ([GroupName], [Name], [DateCreated]) VALUES(N'Optional Expenses', N'Monthly Entertainment', getDate())
INSERT [dbo].[DefaultCategories] ([GroupName], [Name], [DateCreated]) VALUES(N'Optional Expenses', N'Pets', getDate())
INSERT [dbo].[DefaultCategories] ([GroupName], [Name], [DateCreated]) VALUES(N'Optional Expenses', N'Travel', getDate())

--SET IDENTITY_INSERT [dbo].[DefaultCategories] OFF
