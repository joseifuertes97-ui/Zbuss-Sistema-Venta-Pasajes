using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Zbuss_Proyect.Models.Ventas;

namespace Zbuss_Proyect.Dao
{
    public class DetalleVentaDao
    {
        public DetalleVenta BuscarDetalleVenta(int id)
        {
            DetalleVenta oVenta = new DetalleVenta();
            try
            {
                IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

                using (SqlConnection cn = new SqlConnection(config.GetConnectionString("Zbuss_ProyectDBConn")))
                {
                    SqlCommand cmd = new SqlCommand();
                    cn.Open();
                    cmd.Connection = cn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = @"VZB_SP_DETALLE_VENTA_SEL";
                    cmd.Parameters.Add("@IN_CODIGO_DETALLE_VENTA", SqlDbType.Int).Value = id;


                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            oVenta.id = (reader["idventa"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["idventa"]);
                            oVenta.nombrePasajero = (reader["VC_NOMBRE_PASAJERO"] == DBNull.Value) ? String.Empty : Convert.ToString(reader["VC_NOMBRE_PASAJERO"]);
                            oVenta.nroDocumento = (reader["nro_documento"] == DBNull.Value) ? String.Empty : Convert.ToString(reader["nro_documento"]);
                            oVenta.codigoAsiento = (reader["cod_asiento"] == DBNull.Value) ? String.Empty : Convert.ToString(reader["cod_asiento"]);
                            oVenta.fechaVenta = (reader["fecha_venta"] == DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(reader["fecha_venta"]);
                            oVenta.subtotal = (reader["sub_total"] == DBNull.Value) ? 0 : Convert.ToDouble(reader["sub_total"]);
                            oVenta.metodoPago = (reader["metodo_pago"] == DBNull.Value) ? String.Empty : Convert.ToString(reader["metodo_pago"]);
                            oVenta.igv = (reader["igv"] == DBNull.Value) ? 0 : Convert.ToDouble(reader["igv"]);
                            oVenta.descuento = (reader["descuento"] == DBNull.Value) ? 0 : Convert.ToDouble(reader["descuento"]);
                            oVenta.total = (reader["total"] == DBNull.Value) ? 0 : Convert.ToDouble(reader["total"]);
                            oVenta.idCuenta = (reader["idcuenta"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["idcuenta"]);
                            oVenta.estado = Convert.ToBoolean(reader["estado"]);
                            oVenta.fechaViaje = (reader["fecha_viaje"] == DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(reader["fecha_viaje"]);
                            oVenta.horaSalida = ((reader["hora_salida"] == DBNull.Value) ? String.Empty : Convert.ToString(reader["hora_salida"])).Substring(0, 8);
                            oVenta.horaLlegada = ((reader["hora_llegada"] == DBNull.Value) ? String.Empty : Convert.ToString(reader["hora_llegada"])).Substring(0, 8);
                            oVenta.puntoPartida = (reader["punto_partida"] == DBNull.Value) ? String.Empty : Convert.ToString(reader["punto_partida"]);
                            oVenta.puntoLlegada = (reader["punto_llegada"] == DBNull.Value) ? String.Empty : Convert.ToString(reader["punto_llegada"]);
                        }
                    }
                    cn.Close();
                }

                return oVenta;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
