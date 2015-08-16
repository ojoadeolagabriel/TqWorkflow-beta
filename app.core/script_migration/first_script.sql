USE [esb]
GO
/****** Object:  Table [dbo].[activitylog]    Script Date: 8/16/2015 8:30:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[activitylog](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Body] [varchar](max) NULL,
	[ExchangeId] [varchar](200) NULL,
	[ProcessorType] [varchar](100) NULL,
	[component] [varchar](100) NULL,
	[createdOn] [datetime] NULL,
 CONSTRAINT [PK_activitylog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[logMessage]    Script Date: 8/16/2015 8:30:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[logMessage]
@exhangeid varchar(200),
@body varchar(max),
@processorType varchar(100),
@component varchar(100)
AS
BEGIN

insert into activitylog (ExchangeId,Body,processorType,component, createdOn) 
values (@exhangeid, @body,@processorType,@component,GETDATE())

END

GO
