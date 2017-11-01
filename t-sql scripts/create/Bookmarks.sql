USE [fi.retorch.com]
GO

/****** Object:  Table [dbo].[Bookmarks]    Script Date: 2/18/2017 10:29:56 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Bookmarks](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](36) NOT NULL,
	[URL] [nvarchar](255) NOT NULL,
	[Text] [nvarchar](100) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
 CONSTRAINT [PK_Bookmarks] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Bookmarks] ADD  CONSTRAINT [DF_Bookmarks_IsActive]  DEFAULT ((0)) FOR [IsActive]
GO

