using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Zbuss_Proyect.Models.Viajes;
using Zbuss_Proyect.Models.ViewModels;

namespace Zbuss_Proyect.Dao
{
    public class ViajesDao
    {
        public List<Viaje> ListarViajes()
        {
            List<Viaje> ListViajes = new List<Viaje>();
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
                    cmd.CommandText = @"VZB_SP_DETALLE_VIAJE_SEL";


                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Viaje oViaje = new Viaje();

                            oViaje.idViaje = (reader["idviaje"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["idviaje"]);
                            oViaje.fechaSalida = (reader["fecha_salida"] == DBNull.Value) ? String.Empty : Convert.ToString(reader["fecha_salida"]);
                            oViaje.horaSalida = ((reader["hora_salida"] == DBNull.Value) ? String.Empty : Convert.ToString(reader["hora_salida"])).Substring(0,8);
                            oViaje.horaLlegada = ((reader["hora_llegada"] == DBNull.Value) ? String.Empty : Convert.ToString(reader["hora_llegada"])).Substring(0, 8);
                            oViaje.puntoPartida = (reader["punto_partida"] == DBNull.Value) ? String.Empty : Convert.ToString(reader["punto_partida"]);
                            oViaje.puntoLlegada = (reader["punto_llegada"] == DBNull.Value) ? String.Empty : Convert.ToString(reader["punto_llegada"]);
                            oViaje.nroAsientos = (reader["nro_asientos"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["nro_asientos"]);
                            oViaje.duracionViaje = (reader["duracion_viaje"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["duracion_viaje"]);

                            ListViajes.Add(oViaje);
                        }
                    }
                    cn.Close();
                }

                return ListViajes;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
