USE [BDDinoMatnovo]
GO
/****** Object:  StoredProcedure [dbo].[sp_Mam_TY008]    Script Date: 24/01/2020 17:23:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		DIEGO CAYO
-- Create date: 13-06-2019
-- Description:	GESTION DE SERVICIOS
-- =============================================
CREATE PROCEDURE [dbo].[sp_Mam_TY008]
(
--yiId, yiDesc, scPrecio, yiEst, yiFecha, yiHora, yiUsuario
@yiId INT = -1,
@yiDesc NVARCHAR(150)='',
@scPrecio DECIMAL(18,2)=-1, 
@yiEst INT = -1, 
@yiFecha DATE='',
@yiHora NVARCHAR(10)='',
@yiUsuario NVARCHAR(30)='',
@Tipo INT = -1)
AS
BEGIN
	DECLARE @newHora nvarchar(5)=CONCAT(DATEPART(HOUR,GETDATE()),':',DATEPART(MINUTE,GETDATE()))
	DECLARE @newFecha date=GETDATE()
	set @newHora=CONCAT(DATEPART(HOUR,GETDATE()),':',DATEPART(MINUTE,GETDATE()))
	set @newFecha=GETDATE()	
	IF @Tipo = 1 --REGISTRAR
	BEGIN
	--@yiDesc, @scPrecio, @yiEst, @yiFecha, @yiHora, @yiUsuario
		INSERT INTO TY008(yiDesc,  yiEst, yiFecha, yiHora, yiUsuario)
		VALUES(@yiDesc,  @yiEst, @newFecha, @newHora, @yiUsuario)
		set @yiId = (SELECT SCOPE_IDENTITY())
		SELECT @yiId AS fbid 
	END
	IF @Tipo = 2
	BEGIN
		UPDATE TY008--MODIFICAR
		SET 
			yiDesc = @yiDesc,			
			yiEst=@yiEst, 
			yiFecha=@newFecha, 
			yiHora=@newHora, 
			yiUsuario=@yiUsuario
		where
			yiId = @yiId
		SELECT @yiId AS fbid 
	END	
	IF @Tipo = 3--ELIMINAR
	BEGIN
		DELETE  FROM TY008 WHERE yiId =@yiId
		SELECT @yiId AS yiId
	END	
	IF @Tipo = 4--MOSTRAR
	BEGIN
	--yiDesc, scPrecio, yiEst, yiFecha, yiHora, yiUsuario
		SELECT 
			ser.yiId,
			ser.yiDesc,			
			ser.yiEst,
						CASE ser.yiEst WHEN 1 THEN 'ACTIVO'
						   WHEN 2 THEN 'INACTIVO' END AS Estado,
			ser.yiFecha,
			ser.yiHora,
			ser.yiUsuario			
		FROM
			TY008 ser
	END	
END
