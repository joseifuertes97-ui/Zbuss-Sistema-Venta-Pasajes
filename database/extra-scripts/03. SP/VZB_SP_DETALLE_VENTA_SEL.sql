DROP PROCEDURE IF EXISTS VZB_SP_DETALLE_VENTA_SEL;
GO

CREATE PROCEDURE VZB_SP_DETALLE_VENTA_SEL
	@IN_CODIGO_DETALLE_VENTA INT
AS
BEGIN
	select D.idventa
      ,P.nombre + ' ' + P.ape_paterno + ' ' + P.ape_materno AS VC_NOMBRE_PASAJERO
	  ,P.nro_documento
      ,A.cod_asiento
      ,D.fecha_venta
      ,D.sub_total
      ,D.metodo_pago
      ,D.igv
      ,D.descuento
      ,D.total
      ,D.idcuenta
      ,D.estado
      ,D.fecha_viaje
	  ,V.hora_salida
	  ,V.hora_llegada
	  ,V.punto_partida
	  ,V.punto_llegada
	FROM  tb_DETALLE_VENTA D 
	INNER JOIN tb_PASAJERO P ON D.iduser = P.iduser
	INNER JOIN tb_ASIENTOS_BUS A ON D.idasiento = A.idasiento
	INNER JOIN tb_DETALLE_VIAJE V ON D.IN_ID_VIAJE = V.idviaje
	WHERE idventa = @IN_CODIGO_DETALLE_VENTA;
END
GO

EXEC VZB_SP_DETALLE_VENTA_SEL @IN_CODIGO_DETALLE_VENTA =3020;