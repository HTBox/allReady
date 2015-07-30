USE [AllReady-Dev]
GO

/****** Object:  StoredProcedure [GetClosestActivitiesByPostalCode]    Script Date: 07/10/2015 09:04:01 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[GetClosestActivitiesByPostalCode]') AND type in (N'P', N'PC'))
DROP PROCEDURE [GetClosestActivitiesByPostalCode]
GO

USE [AllReady-Dev]
GO

/****** Object:  StoredProcedure [GetClosestActivitiesByPostalCode]    Script Date: 07/10/2015 09:04:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




CREATE PROCEDURE [GetClosestActivitiesByPostalCode]
	@postalCode nvarchar(20),
	@count int = 10,
	@distance int
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @lat float
	DECLARE @lon float
	
	SELECT	@lat = latitude,
			@lon = longitude
	FROM	fnGetCoordinatesForPostalCode(@postalCode)
	
    DECLARE @g geography 
	SET @g = CONVERT(NVARCHAR(200),'POINT(' + CONVERT(NVARCHAR(20),@lon) + ' ' + CONVERT(NVARCHAR(20),@lat) +')')
	
	SET @distance = @distance * 1000
	
	SELECT		a.* 
	FROM		Activity a
	WHERE		a.LocationId
	IN
	(
		SELECT		l.Id
		FROM		Location l
		JOIN		PostalCodeGeo g on l.PostalCodePostalCode = g.PostalCode
		WHERE		(g.GeoLocation.STDistance(@g) < @distance)
	)
END



GO


