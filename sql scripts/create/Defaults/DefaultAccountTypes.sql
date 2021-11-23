USE [fi.retorch.com]
GO

/****** Object:  Table [dbo].[DefaultAccountTypes]    Script Date: 2/16/2017 9:30:51 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DefaultAccountTypes](
	[Name] [nvarchar](100) NOT NULL,
	[IsDebt] [bit] NOT NULL,
	[PositiveText] [nvarchar](100) NOT NULL,
	[NegativeText] [nvarchar](100) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
 CONSTRAINT [PK_DefaultAccountTypes] PRIMARY KEY CLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[DefaultAccountTypes] ADD  CONSTRAINT [DF_DefaultAccountTypes_IsDebt]  DEFAULT ((0)) FOR [IsDebt]
GO

ALTER TABLE [dbo].[DefaultAccountTypes] ADD  CONSTRAINT [DF_DefaultAccountTypes_DateCreated]  DEFAULT (getdate()) FOR [DateCreated]
GO

