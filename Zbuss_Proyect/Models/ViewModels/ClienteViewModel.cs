using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zbuss_Proyect.Models.Ventas;
using Zbuss_Proyect.Models.Viajes;

namespace Zbuss_Proyect.Models.ViewModels
{
    public class ClienteViewModel
    {
        public DetalleVenta DetalleVenta { get; set; }
        
        //Listas
        public List<Viaje> ListaViajes { get; set; }
    }
}
