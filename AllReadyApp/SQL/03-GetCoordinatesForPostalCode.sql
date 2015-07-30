USE [AllReady-Dev]
GO

/****** Object:  StoredProcedure [GetCoordinatesForPostalCode]    Script Date: 07/10/2015 09:03:06 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[GetCoordinatesForPostalCode]') AND type in (N'P', N'PC'))
DROP PROCEDURE [GetCoordinatesForPostalCode]
GO

USE [AllReady-Dev]
GO

/****** Object:  StoredProcedure [GetCoordinatesForPostalCode]    Script Date: 07/10/2015 09:03:06 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [GetCoordinatesForPostalCode]
	@postalcode nvarchar(20)
AS
BEGIN
	SET NOCOUNT ON;

    SELECT	[GeoLocation].Lat as [Latitude]
			,[GeoLocation].Long as [Longitude]
	FROM	[PostalCodeGeo]
	WHERE	PostalCode = @postalcode
END


GO


