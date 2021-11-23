USE [fi.retorch.com]
GO

/****** Object:  Table [dbo].[Categories]    Script Date: 2/3/2017 10:34:18 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Categories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](36) NOT NULL,
	[GroupId] [int] NULL,
	[Name] [nvarchar](100) NOT NULL,
	[IsActive] [bit] NOT NULL CONSTRAINT [DF_Categories_IsActive]  DEFAULT ((0)),
	[DateCreated] [datetime] NOT NULL,
 CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

