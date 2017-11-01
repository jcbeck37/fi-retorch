USE [fi.retorch.com]
GO

/****** Object:  Table [dbo].[Transactions]    Script Date: 2/4/2017 2:34:56 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Transactions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AccountId] [int] NOT NULL,
	[CategoryId] [int] NULL,
	[Name] [nvarchar](100) NOT NULL,
	[DisplayDate] [datetime] NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[IsCredit] [bit] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[DatePosted] [datetime] NULL,
	[DisplayOrder] [int] NOT NULL,
 CONSTRAINT [PK_Transactions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Transactions] ADD  CONSTRAINT [DF_Transactions_IsCredit]  DEFAULT ((0)) FOR [IsCredit]
GO

ALTER TABLE [dbo].[Transactions] ADD  CONSTRAINT [DF_Transactions_DisplayOrder]  DEFAULT ((0)) FOR [DisplayOrder]
GO

