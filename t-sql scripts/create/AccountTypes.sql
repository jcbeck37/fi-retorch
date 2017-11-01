USE [fi.retorch.com]
GO

/****** Object:  Table [dbo].[AccountTypes]    Script Date: 2/9/2017 7:18:10 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AccountTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](36) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[IsDebt] [bit] NOT NULL CONSTRAINT [DF_AccountTypes_IsDebt]  DEFAULT ((0)),
	[PositiveText] [nvarchar](100) NULL,
	[NegativeText] [nvarchar](100) NULL,
	[DateCreated] [datetime] NOT NULL,
 CONSTRAINT [PK_AccountTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

