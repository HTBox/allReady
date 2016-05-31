USE [AllReady-Dev]
GO

/****** Object:  StoredProcedure [GetCoordinatesForPostalCode]    Script Date: 07/10/2015 09:03:06 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[GetCoordinatesForPostalCode]') AND type in (N'P', N'PC'))
DROP PROCEDURE [GetCoordinatesForPostalCode]
GO
