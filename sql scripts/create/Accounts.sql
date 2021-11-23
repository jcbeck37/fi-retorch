USE [fi.retorch.com]
GO

/****** Object:  Table [dbo].[Accounts]    Script Date: 2/6/2017 10:05:04 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Accounts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](36) NOT NULL,
	[TypeId] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[DateOpened] [datetime] NOT NULL,
	[OpeningBalance] [decimal](18, 2) NOT NULL,
	[CurrentBalance] [decimal](18, 2) NOT NULL,
	[IsClosed] [bit] NOT NULL CONSTRAINT [DF_Accounts_IsClosed]  DEFAULT ((0)),
	[DateClosed] [datetime] NULL,
	[IsDisplayed] [bit] NOT NULL CONSTRAINT [DF_Accounts_IsDisplayed]  DEFAULT ((0)),
	[DisplayOrder] [int] NOT NULL CONSTRAINT [DF_Accounts_DisplayOrder]  DEFAULT ((0)),
 CONSTRAINT [PK_Accounts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

