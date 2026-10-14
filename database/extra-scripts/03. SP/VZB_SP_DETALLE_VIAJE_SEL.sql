DROP PROCEDURE IF EXISTS VZB_SP_DETALLE_VIAJE_SEL;
GO

CREATE PROCEDURE VZB_SP_DETALLE_VIAJE_SEL
AS
BEGIN
	select idviaje
	  ,fecha_salida
      ,hora_salida
      ,hora_llegada
      ,punto_partida
      ,punto_llegada
      ,nro_asientos
      ,duracion_viaje
	FROM  tb_DETALLE_VIAJE;
END
GO

EXEC VZB_SP_DETALLE_VIAJE_SEL;