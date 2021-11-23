USE [fi.retorch.com]
GO
/****** Object:  Index [PK_Transactions]    Script Date: 3/2/2017 8:16:20 PM ******/
ALTER TABLE [dbo].[Transactions]

DROP CONSTRAINT [PK_Transactions]
GO

ALTER TABLE [dbo].[Transactions]
ADD  CONSTRAINT [PK_Transactions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC,
	[AccountId] ASC--,
	--[DisplayDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


