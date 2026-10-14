using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zbuss_Proyect.Models.Ventas
{
    public class DetalleVenta
    {
        public int id { get; set; }
        public string nombrePasajero { get; set; }
        public string nroDocumento { get; set; }
        public string codigoAsiento { get; set; }
        public DateTime? fechaVenta { get; set; }
        public double subtotal { get; set; }
        public string metodoPago { get; set; }
        public double igv { get; set; }
        public double descuento { get; set; }
        public double total { get; set; }
        public int idCuenta { get; set; }
        public bool estado { get; set; }
        public DateTime? fechaViaje { get; set; }
        public string horaSalida { get; set; }
        public string horaLlegada { get; set; }
        public string puntoPartida { get; set; }
        public string puntoLlegada { get; set; }
    }
}
