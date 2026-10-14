using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zbuss_Proyect.Models.Viajes
{
    public class Viaje
    {
        public int idViaje { get; set; }
        public string fechaSalida { get; set; }
        public string horaSalida { get; set; }
        public string horaLlegada { get; set; }
        public string puntoPartida { get; set; }
        public string puntoLlegada { get; set; }
        public int nroAsientos { get; set; }
        public int duracionViaje { get; set; }
    }
}
