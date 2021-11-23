USE [fi.retorch.com]
GO

/****** Object:  Index [PK_AccountTypes]    Script Date: 3/2/2017 8:21:57 PM ******/
ALTER TABLE [dbo].[AccountTypes]

DROP CONSTRAINT [PK_AccountTypes]
GO

ALTER TABLE [dbo].[AccountTypes] ADD  CONSTRAINT [PK_AccountTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


