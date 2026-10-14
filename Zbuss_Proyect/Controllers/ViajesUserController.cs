using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zbuss_Proyect.Models;
using Microsoft.EntityFrameworkCore;
using Zbuss_Proyect.Models.ViewModels;
using Zbuss_Proyect.Dao;

namespace Zbuss_Proyect.Controllers
{
    public class ViajesUserController : Controller
    {
        private readonly bd_VENTAS_ZBUSSContext _context;

        public ViajesUserController(bd_VENTAS_ZBUSSContext context)
        {
            _context = context;
        }

        public TbDetalleViaje RecuperarViaje(int? id)
        {
            if (id == null)
            {
                return null;
            }

            var viajeBd = _context.TbDetalleViaje.Find(id);
            if(viajeBd == null)
            {
                return null;
            }

            return viajeBd;
        }

        public async Task<IActionResult> Index()
        {
            ClienteViewModel model = new ClienteViewModel();
            ViajesDao dao = new ViajesDao();

            model.ListaViajes = dao.ListarViajes();

            /*var bd_VENTAS_ZBUSSContext = _context.TbDetalleViaje.Include(t => t.IdbusNavigation);*/
            return View(model);
        }
    }
}
