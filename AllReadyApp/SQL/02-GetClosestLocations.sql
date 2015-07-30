/****** Object:  StoredProcedure [GetClosestLocations]    Script Date: 07/10/2015 09:02:24 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[GetClosestLocations]') AND type in (N'P', N'PC'))
DROP PROCEDURE [GetClosestLocations]
GO

/****** Object:  StoredProcedure [GetClosestLocations]    Script Date: 07/10/2015 09:02:24 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [GetClosestLocations]
	@lat float,
	@lon float,
	@count int = 10,
	@distance int
AS
BEGIN
	SET NOCOUNT ON;

    DECLARE @g geography 
	SET @g = CONVERT(NVARCHAR(200),'POINT(' + CONVERT(NVARCHAR(20),@lon) + ' ' + CONVERT(NVARCHAR(20),@lat) +')')
	
	SET @distance = @distance * 1000
	
	SELECT TOP(@count) 
		PostalCode, 
		City, 
		[State],
		GeoLocation.STDistance(@g) as [Distance]
	FROM 
		PostalCodeGeo
	WHERE
		GeoLocation.STDistance(@g) < @distance
	ORDER BY GeoLocation.STDistance(@g)
END

GO


