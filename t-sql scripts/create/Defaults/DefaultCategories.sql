USE [fi.retorch.com]
GO

/****** Object:  Table [dbo].[DefaultCategories]    Script Date: 2/18/2017 8:40:23 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DefaultCategories](
	[GroupName] [nvarchar](100) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
 CONSTRAINT [PK_DefaultCategories] PRIMARY KEY CLUSTERED 
(
	[GroupName] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[DefaultCategories] ADD  CONSTRAINT [DF_DefaultCategories_DateCreated]  DEFAULT (getdate()) FOR [DateCreated]
GO

