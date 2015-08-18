USE [esb]
GO
/****** Object:  StoredProcedure [dbo].[logMessage]    Script Date: 8/18/2015 11:39:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[logMessage]
@exhangeid varchar(200),
@body varchar(max),
@processorType varchar(100),
@component varchar(100),
@routeId varchar(200),
@errorMessage varchar(max)
AS
BEGIN

insert into activitylog (ExchangeId,Body,processorType,component, createdOn,routeId ,errorMessage) 
values (@exhangeid, @body,@processorType,@component,GETDATE(),@routeId,@errorMessage)

END
