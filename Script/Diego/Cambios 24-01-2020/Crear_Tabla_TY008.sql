USE [BDDinoMatnovo]
GO

/****** Object:  Table [dbo].[TY008]    Script Date: 24/01/2020 17:23:40 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TY008](
	[yiId] [int] IDENTITY(1,1) NOT NULL,
	[yiDesc] [nvarchar](150) NULL,
	[yiEst] [int] NULL,
	[yiFecha] [date] NULL,
	[yiHora] [nvarchar](10) NULL,
	[yiUsuario] [nvarchar](30) NULL,
PRIMARY KEY CLUSTERED 
(
	[yiId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


