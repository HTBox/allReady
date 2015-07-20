/****** Object:  UserDefinedFunction [fnGetCoordinatesForPostalCode]    Script Date: 07/10/2015 09:01:23 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[fnGetCoordinatesForPostalCode]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [fnGetCoordinatesForPostalCode]
GO

/****** Object:  UserDefinedFunction [fnGetCoordinatesForPostalCode]    Script Date: 07/10/2015 09:01:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE FUNCTION [fnGetCoordinatesForPostalCode] 
( 
    @postalcode nvarchar(20)
) 
RETURNS @output TABLE
(
	latitude float,
	longitude float
) 
BEGIN
	INSERT INTO @output(latitude,longitude)
	SELECT	[GeoLocation].Lat as [Latitude]
			,[GeoLocation].Long as [Longitude]
	FROM	[PostalCodeGeo]
	WHERE	PostalCode = @postalcode
	
    RETURN 
END

GO


