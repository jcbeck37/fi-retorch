USE [fi.retorch.com]
GO

/****** Object:  Table [dbo].[Reminders]    Script Date: 2/8/2017 7:22:25 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Reminders](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AccountId] [int] NOT NULL,
	[CategoryId] [int] NULL,
	[ScheduleId] [int] NOT NULL,
	[TypeId] [int] NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Amount] [decimal](18, 2) NULL,
	[Rate] [decimal](18, 2) NULL,
	[IsCredit] [bit] NOT NULL CONSTRAINT [DF_Reminders_IsCredit]  DEFAULT ((0)),
	[NextDate] [datetime] NOT NULL,
	[LastDate] [datetime] NULL,
 CONSTRAINT [PK_Reminders] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

