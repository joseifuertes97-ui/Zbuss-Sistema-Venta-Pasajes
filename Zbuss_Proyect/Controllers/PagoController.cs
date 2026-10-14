using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using Zbuss_Proyect.Dao;
using Zbuss_Proyect.Models;
using Zbuss_Proyect.Models.Ventas;

namespace Zbuss_Proyect.Controllers
{
    public class PagoController : Controller
    {
        private readonly bd_VENTAS_ZBUSSContext _context;

        public PagoController(bd_VENTAS_ZBUSSContext context)
        {
            _context = context;
        }

        public IActionResult Procesar()
        {
            return View();
        }

        public TbDetalleVenta ProcesarCompra(TbDetalleVenta pVenta, int idViaje, string correoBoleta)
        {
            DetalleVentaDao daoDetalle = new DetalleVentaDao();

            if (!VentaExists(pVenta.Idasiento))
            {
                TbAsientosBus asientoBd = _context.TbAsientosBus.FirstOrDefault(x => x.Idasiento == pVenta.Idasiento);
                asientoBd.Estado = false;
                _context.Entry(asientoBd).State = EntityState.Modified;

                var subtotal = Convert.ToDouble(pVenta.SubTotal);
                var igv = subtotal * 0.18;

                var usuario = HttpContext.User.Identity.Name;
                var usuarioBd = _context.TbUsuarios.FirstOrDefault(x => x.Correo == usuario);

                TbDetalleVenta ventaBd = new TbDetalleVenta()
                {
                    Iduser = pVenta.Iduser,
                    Idasiento = pVenta.Idasiento,
                    FechaVenta = DateTime.Now,
                    SubTotal = pVenta.SubTotal,
                    MetodoPago = "TARJETA",
                    Igv = Convert.ToDecimal(igv),
                    Descuento = 0,
                    Total = pVenta.Total,
                    Idcuenta = usuarioBd.IdUsuario,
                    FechaViaje = pVenta.FechaViaje,
                    Estado = true,
                    idViaje = idViaje

                };
                if (ModelState.IsValid)
                {
                    _context.TbDetalleVenta.Add(ventaBd);
                    _context.SaveChanges();

                    pVenta.Idventa = ventaBd.Idventa;

                    EnviarCorreo(ventaBd.Idventa, correoBoleta);

                    return ventaBd;
                }

                return null;
            }
            return null;
        }

        private void EnviarCorreo(int idventa, string correoBoleta)
        {
            DetalleVentaDao daoDetalle = new DetalleVentaDao();

            DetalleVenta oVentaDetalle = daoDetalle.BuscarDetalleVenta(idventa);
            string nom = oVentaDetalle.nombrePasajero;
            string dni = oVentaDetalle.nroDocumento;
            string codA = oVentaDetalle.codigoAsiento;
            string fechaV = oVentaDetalle.fechaVenta.ToString().Substring(0, 8);
            string subT = oVentaDetalle.subtotal.ToString();
            string mPago = oVentaDetalle.metodoPago;
            string igvVenta = oVentaDetalle.igv.ToString();
            string dsct = oVentaDetalle.descuento.ToString();
            string total = oVentaDetalle.total.ToString();
            string fechaViaje = oVentaDetalle.fechaViaje.ToString().Substring(0, 8);
            string hSalida = oVentaDetalle.horaSalida;
            string puntoP = oVentaDetalle.puntoPartida;
            string puntoF = oVentaDetalle.puntoLlegada;

            


            try
            {
                var fromAddress = new MailAddress("zbussventas.busseando@gmail.com", "Busseando");
                var toAddress = new MailAddress(correoBoleta, "Cliente");
                const string fromPassword = "CONFIGURA_TU_APP_PASSWORD";
                const string subject = "Comprobante de pago";

                string body = @"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional //EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"" xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:o=""urn:schemas-microsoft-com:office:office"">
<head>
<!--[if gte mso 9]>
<xml>
  <o:OfficeDocumentSettings>
    <o:AllowPNG/>
    <o:PixelsPerInch>96</o:PixelsPerInch>
  </o:OfficeDocumentSettings>
</xml>
<![endif]-->
  <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
  <meta name=""x-apple-disable-message-reformatting"">
  <!--[if !mso]><!--><meta http-equiv=""X-UA-Compatible"" content=""IE=edge""><!--<![endif]-->
  <title></title>
  
    <style type=""text/css"">
      @media only screen and (min-width: 620px) {
  .u-row {
    width: 600px !important;
  }
  .u-row .u-col {
    vertical-align: top;
  }

  .u-row .u-col-33p33 {
    width: 199.98px !important;
  }

  .u-row .u-col-50 {
    width: 300px !important;
  }

  .u-row .u-col-66p67 {
    width: 400.02px !important;
  }

  .u-row .u-col-100 {
    width: 600px !important;
  }

}

@media (max-width: 620px) {
  .u-row-container {
    max-width: 100% !important;
    padding-left: 0px !important;
    padding-right: 0px !important;
  }
  .u-row .u-col {
    min-width: 320px !important;
    max-width: 100% !important;
    display: block !important;
  }
  .u-row {
    width: 100% !important;
  }
  .u-col {
    width: 100% !important;
  }
  .u-col > div {
    margin: 0 auto;
  }
}
a[x-apple-data-detectors='true'] {
  color: inherit !important;
  text-decoration: none !important;
}

</style>
  
  

<!--[if !mso]><!--><link href=""https://fonts.googleapis.com/css?family=Open+Sans:400,700&display=swap"" rel=""stylesheet"" type=""text/css""><link href=""https://fonts.googleapis.com/css?family=Lato:400,700&display=swap"" rel=""stylesheet"" type=""text/css""><!--<![endif]-->

</head>

<body class=""clean-body u_body"" style=""margin: 0;padding: 0;-webkit-text-size-adjust: 100%;background-color: #707070;color: #000000"">
  <!--[if IE]><div class=""ie-container""><![endif]-->
  <!--[if mso]><div class=""mso-container""><![endif]-->
  <table style=""border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;min-width: 320px;Margin: 0 auto;background-color: #707070;width:100%"" cellpadding=""0"" cellspacing=""0"">
  <tbody>
  <tr style=""vertical-align: top"">
    <td style=""word-break: break-word;border-collapse: collapse !important;vertical-align: top"">
    <!--[if (mso)|(IE)]><table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""><tr><td align=""center"" style=""background-color: #707070;""><![endif]-->
    

<div class=""u-row-container"" style=""padding: 29px 10px 0px;background-color: rgba(255,255,255,0)"">
  <div class=""u-row"" style=""Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #17c297;"">
    <div style=""border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;"">
      <!--[if (mso)|(IE)]><table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""><tr><td style=""padding: 29px 10px 0px;background-color: rgba(255,255,255,0);"" align=""center""><table cellpadding=""0"" cellspacing=""0"" border=""0"" style=""width:600px;""><tr style=""background-color: #17c297;""><![endif]-->
      
<!--[if (mso)|(IE)]><td align=""center"" width=""200"" style=""width: 200px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;"" valign=""top""><![endif]-->
<div class=""u-col u-col-33p33"" style=""max-width: 320px;min-width: 200px;display: table-cell;vertical-align: top;"">
  <div style=""height: 100%;width: 100% !important;"">
  <!--[if (!mso)&(!IE)]><!--><div style=""box-sizing: border-box; height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;""><!--<![endif]-->
  
<table style=""font-family:'Lato',sans-serif;"" role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" border=""0"">
  <tbody>
    <tr>
      <td style=""overflow-wrap:break-word;word-break:break-word;padding:20px;font-family:'Lato',sans-serif;"" align=""left"">
        
<table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"">
  <tr>
    <td style=""padding-right: 0px;padding-left: 0px;"" align=""center"">
      
      <img align=""center"" border=""0"" src='https://drive.google.com/uc?export=download&id=1fSA8fiEEzWzR3gRxv0KojirNW1LLiRM9' alt=""Image"" title=""Image"" style=""outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: inline-block !important;border: none;height: auto;float: none;width: 100%;max-width: 160px;"" width=""160""/>
      
    </td>
  </tr>
</table>

      </td>
    </tr>
  </tbody>
</table>

  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->
  </div>
</div>
<!--[if (mso)|(IE)]></td><![endif]-->
<!--[if (mso)|(IE)]><td align=""center"" width=""400"" style=""width: 400px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;"" valign=""top""><![endif]-->
<div class=""u-col u-col-66p67"" style=""max-width: 320px;min-width: 400px;display: table-cell;vertical-align: top;"">
  <div style=""height: 100%;width: 100% !important;"">
  <!--[if (!mso)&(!IE)]><!--><div style=""box-sizing: border-box; height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;""><!--<![endif]-->
  
<table style=""font-family:'Lato',sans-serif;"" role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" border=""0"">
  <tbody>
    <tr>
      <td style=""overflow-wrap:break-word;word-break:break-word;padding:44px 20px 20px;font-family:'Lato',sans-serif;"" align=""left"">
        
  <div style=""color: #ffffff; line-height: 120%; text-align: right; word-wrap: break-word;"">
    <p style=""font-size: 14px; line-height: 120%;"">Reserva Confirmada ¡Muchas gracias por confiar en Busseando!</p>
  </div>

      </td>
    </tr>
  </tbody>
</table>

  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->
  </div>
</div>
<!--[if (mso)|(IE)]></td><![endif]-->
      <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->
    </div>
  </div>
</div>



<div class=""u-row-container"" style=""padding: 0px 10px;background-color: rgba(255,255,255,0)"">
  <div class=""u-row"" style=""Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #f5f5f5;"">
    <div style=""border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;"">
      <!--[if (mso)|(IE)]><table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""><tr><td style=""padding: 0px 10px;background-color: rgba(255,255,255,0);"" align=""center""><table cellpadding=""0"" cellspacing=""0"" border=""0"" style=""width:600px;""><tr style=""background-color: #f5f5f5;""><![endif]-->
      
<!--[if (mso)|(IE)]><td align=""center"" width=""300"" style=""width: 300px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;"" valign=""top""><![endif]-->
<div class=""u-col u-col-50"" style=""max-width: 320px;min-width: 300px;display: table-cell;vertical-align: top;"">
  <div style=""height: 100%;width: 100% !important;"">
  <!--[if (!mso)&(!IE)]><!--><div style=""box-sizing: border-box; height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;""><!--<![endif]-->
  
<table style=""font-family:'Lato',sans-serif;"" role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" border=""0"">
  <tbody>
    <tr>
      <td style=""overflow-wrap:break-word;word-break:break-word;padding:35px 20px 10px;font-family:'Lato',sans-serif;"" align=""left"">
        
  <div style=""line-height: 120%; text-align: left; word-wrap: break-word;"">
    <p style=""font-size: 14px; line-height: 120%;""><span style=""font-size: 24px; line-height: 28.8px; color: #18c197;""><strong>Nombre de Pasajero:</strong></span></p>
  </div>

      </td>
    </tr>
  </tbody>
</table>

<table style=""font-family:'Lato',sans-serif;"" role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" border=""0"">
  <tbody>
    <tr>
      <td style=""overflow-wrap:break-word;word-break:break-word;padding:10px 20px 30px;font-family:'Lato',sans-serif;"" align=""left"">
        
  <div style=""color: #757575; line-height: 160%; text-align: left; word-wrap: break-word;"">
    <p style=""font-size: 14px; line-height: 160%;""><span style=""font-size: 14px; line-height: 22.4px;"">Detalle del Viaje:</span></p>
<p style=""font-size: 14px; line-height: 160%;""><span style=""font-size: 14px; line-height: 22.4px;"">Punto de Partida: ${{puntoP}}</span></p>
<p style=""font-size: 14px; line-height: 160%;""><span style=""font-size: 14px; line-height: 22.4px;"">Punto de LLegada: ${{puntoF}}</span></p>
<p style=""font-size: 14px; line-height: 160%;""><span style=""font-size: 14px; line-height: 22.4px;"">DNI Pasajero: ${{dni}}</span></p>
<p style=""font-size: 14px; line-height: 160%;""><span style=""font-size: 14px; line-height: 22.4px;"">Fecha de Viaje ${{fechaViaje}}</span></p>
<p style=""font-size: 14px; line-height: 160%;""><span style=""font-size: 14px; line-height: 22.4px;"">Hora de Salida: ${{hSalida}}</span></p>
  </div>
      </td>
    </tr>
  </tbody>
</table>

  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->
  </div>
</div>
<!--[if (mso)|(IE)]></td><![endif]-->
<!--[if (mso)|(IE)]><td align=""center"" width=""300"" style=""width: 300px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;"" valign=""top""><![endif]-->
<div class=""u-col u-col-50"" style=""max-width: 320px;min-width: 300px;display: table-cell;vertical-align: top;"">
  <div style=""height: 100%;width: 100% !important;"">
  <!--[if (!mso)&(!IE)]><!--><div style=""box-sizing: border-box; height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;""><!--<![endif]-->
  
<table style=""font-family:'Lato',sans-serif;"" role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" border=""0"">
  <tbody>
    <tr>
      <td style=""overflow-wrap:break-word;word-break:break-word;padding:35px 20px 10px;font-family:'Lato',sans-serif;"" align=""left"">
        
  <div style=""color: #333333; line-height: 120%; text-align: left; word-wrap: break-word;"">
    <p style=""font-size: 14px; line-height: 120%;""><strong><span style=""font-size: 24px; line-height: 28.8px;"">${{nom}}</span></strong></p>
  </div>

      </td>
    </tr>
  </tbody>
</table>

<table style=""font-family:'Lato',sans-serif;"" role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" border=""0"">
  <tbody>
    <tr>
      <td style=""overflow-wrap:break-word;word-break:break-word;padding:10px 20px 30px;font-family:'Lato',sans-serif;"" align=""left"">
        
  <div style=""color: #333333; line-height: 120%; text-align: left; word-wrap: break-word;"">
    <p style=""font-size: 14px; line-height: 120%;""><span style=""font-size: 20px; line-height: 24px;""><strong>Cod Asiento: ${{codA}}</strong></span></p>
  </div>

      </td>
    </tr>
  </tbody>
</table>

  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->
  </div>
</div>
<!--[if (mso)|(IE)]></td><![endif]-->
      <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->
    </div>
  </div>
</div>



<div class=""u-row-container"" style=""padding: 0px 10px;background-color: rgba(255,255,255,0)"">
  <div class=""u-row"" style=""Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #ffffff;"">
    <div style=""border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;"">
      <!--[if (mso)|(IE)]><table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""><tr><td style=""padding: 0px 10px;background-color: rgba(255,255,255,0);"" align=""center""><table cellpadding=""0"" cellspacing=""0"" border=""0"" style=""width:600px;""><tr style=""background-color: #ffffff;""><![endif]-->
      
<!--[if (mso)|(IE)]><td align=""center"" width=""600"" style=""width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;"" valign=""top""><![endif]-->
<div class=""u-col u-col-100"" style=""max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;"">
  <div style=""height: 100%;width: 100% !important;"">
  <!--[if (!mso)&(!IE)]><!--><div style=""box-sizing: border-box; height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;""><!--<![endif]-->
  
<table style=""font-family:'Lato',sans-serif;"" role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" border=""0"">
  <tbody>
    <tr>
      <td style=""overflow-wrap:break-word;word-break:break-word;padding:30px 20px 20px;font-family:'Lato',sans-serif;"" align=""left"">
        
  <div style=""color: #333333; line-height: 120%; text-align: left; word-wrap: break-word;"">
    <p style=""font-size: 14px; line-height: 120%;""><strong><span style=""font-size: 24px; line-height: 28.8px;"">Detalle de Compra</span></strong></p>
  </div>

      </td>
    </tr>
  </tbody>
</table>

<table style=""font-family:'Lato',sans-serif;"" role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" border=""0"">
  <tbody>
    <tr>
      <td style=""overflow-wrap:break-word;word-break:break-word;padding:0px;font-family:'Lato',sans-serif;"" align=""left"">
        
  <table height=""0px"" align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;border-top: 1px solid #e3e3e3;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%"">
    <tbody>
      <tr style=""vertical-align: top"">
        <td style=""word-break: break-word;border-collapse: collapse !important;vertical-align: top;font-size: 0px;line-height: 0px;mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%"">
          <span>&#160;</span>
        </td>
      </tr>
    </tbody>
  </table>

      </td>
    </tr>
  </tbody>
</table>

  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->
  </div>
</div>
<!--[if (mso)|(IE)]></td><![endif]-->
      <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->
    </div>
  </div>
</div>



<div class=""u-row-container"" style=""padding: 0px 10px;background-color: rgba(255,255,255,0)"">
  <div class=""u-row"" style=""Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #ffffff;"">
    <div style=""border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;"">
      <!--[if (mso)|(IE)]><table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""><tr><td style=""padding: 0px 10px;background-color: rgba(255,255,255,0);"" align=""center""><table cellpadding=""0"" cellspacing=""0"" border=""0"" style=""width:600px;""><tr style=""background-color: #ffffff;""><![endif]-->
      
<!--[if (mso)|(IE)]><td align=""center"" width=""400"" style=""width: 400px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;"" valign=""top""><![endif]-->
<div class=""u-col u-col-66p67"" style=""max-width: 320px;min-width: 400px;display: table-cell;vertical-align: top;"">
  <div style=""height: 100%;width: 100% !important;"">
  <!--[if (!mso)&(!IE)]><!--><div style=""box-sizing: border-box; height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;""><!--<![endif]-->
  
<table style=""font-family:'Lato',sans-serif;"" role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" border=""0"">
  <tbody>
    <tr>
      <td style=""overflow-wrap:break-word;word-break:break-word;padding:28px 20px 20px;font-family:'Lato',sans-serif;"" align=""left"">
        
  <div style=""color: #333333; line-height: 140%; text-align: left; word-wrap: break-word;"">
    <p style=""font-size: 14px; line-height: 140%;""><span style=""font-size: 18px; line-height: 25.2px;"">Fecha de Compra</span></p>
  </div>

      </td>
    </tr>
  </tbody>
</table>

  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->
  </div>
</div>
<!--[if (mso)|(IE)]></td><![endif]-->
<!--[if (mso)|(IE)]><td align=""center"" width=""200"" style=""width: 200px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;"" valign=""top""><![endif]-->
<div class=""u-col u-col-33p33"" style=""max-width: 320px;min-width: 200px;display: table-cell;vertical-align: top;"">
  <div style=""height: 100%;width: 100% !important;"">
  <!--[if (!mso)&(!IE)]><!--><div style=""box-sizing: border-box; height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;""><!--<![endif]-->
  
<table style=""font-family:'Lato',sans-serif;"" role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" border=""0"">
  <tbody>
    <tr>
      <td style=""overflow-wrap:break-word;word-break:break-word;padding:40px 20px 20px;font-family:'Lato',sans-serif;"" align=""left"">
        
  <div style=""color: #333333; line-height: 120%; text-align: left; word-wrap: break-word;"">
    <p style=""font-size: 14px; line-height: 120%;""><span style=""font-size: 24px; line-height: 28.8px;""><strong><span style=""line-height: 28.8px; font-size: 24px;"">${{fechaV}}</span></strong></span></p>
  </div>

      </td>
    </tr>
  </tbody>
</table>

  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->
  </div>
</div>
<!--[if (mso)|(IE)]></td><![endif]-->
      <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->
    </div>
  </div>
</div>



<div class=""u-row-container"" style=""padding: 0px 10px;background-color: rgba(255,255,255,0)"">
  <div class=""u-row"" style=""Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #ffffff;"">
    <div style=""border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;"">
      <!--[if (mso)|(IE)]><table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""><tr><td style=""padding: 0px 10px;background-color: rgba(255,255,255,0);"" align=""center""><table cellpadding=""0"" cellspacing=""0"" border=""0"" style=""width:600px;""><tr style=""background-color: #ffffff;""><![endif]-->
      
<!--[if (mso)|(IE)]><td align=""center"" width=""600"" style=""width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;"" valign=""top""><![endif]-->
<div class=""u-col u-col-100"" style=""max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;"">
  <div style=""height: 100%;width: 100% !important;"">
  <!--[if (!mso)&(!IE)]><!--><div style=""box-sizing: border-box; height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;""><!--<![endif]-->
  
<table style=""font-family:'Lato',sans-serif;"" role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" border=""0"">
  <tbody>
    <tr>
      <td style=""overflow-wrap:break-word;word-break:break-word;padding:0px;font-family:'Lato',sans-serif;"" align=""left"">
        
  <table height=""0px"" align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;border-top: 1px solid #e3e3e3;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%"">
    <tbody>
      <tr style=""vertical-align: top"">
        <td style=""word-break: break-word;border-collapse: collapse !important;vertical-align: top;font-size: 0px;line-height: 0px;mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%"">
          <span>&#160;</span>
        </td>
      </tr>
    </tbody>
  </table>

      </td>
    </tr>
  </tbody>
</table>

  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->
  </div>
</div>
<!--[if (mso)|(IE)]></td><![endif]-->
      <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->
    </div>
  </div>
</div>



<div class=""u-row-container"" style=""padding: 0px 10px;background-color: rgba(255,255,255,0)"">
  <div class=""u-row"" style=""Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #ffffff;"">
    <div style=""border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;"">
      <!--[if (mso)|(IE)]><table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""><tr><td style=""padding: 0px 10px;background-color: rgba(255,255,255,0);"" align=""center""><table cellpadding=""0"" cellspacing=""0"" border=""0"" style=""width:600px;""><tr style=""background-color: #ffffff;""><![endif]-->
      
<!--[if (mso)|(IE)]><td align=""center"" width=""400"" style=""width: 400px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;"" valign=""top""><![endif]-->
<div class=""u-col u-col-66p67"" style=""max-width: 320px;min-width: 400px;display: table-cell;vertical-align: top;"">
  <div style=""height: 100%;width: 100% !important;"">
  <!--[if (!mso)&(!IE)]><!--><div style=""box-sizing: border-box; height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;""><!--<![endif]-->
  
<table style=""font-family:'Lato',sans-serif;"" role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" border=""0"">
  <tbody>
    <tr>
      <td style=""overflow-wrap:break-word;word-break:break-word;padding:28px 20px 20px;font-family:'Lato',sans-serif;"" align=""left"">
        
  <div style=""color: #333333; line-height: 140%; text-align: left; word-wrap: break-word;"">
    <p style=""font-size: 14px; line-height: 140%;""><span style=""font-size: 18px; line-height: 25.2px;"">SubTotal</span></p>
<p style=""font-size: 14px; line-height: 140%;""><span style=""font-size: 16px; line-height: 22.4px; color: #17c297;"">Igv: S/. ${{igvVenta}}</span></p>
  </div>

      </td>
    </tr>
  </tbody>
</table>

  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->
  </div>
</div>
<!--[if (mso)|(IE)]></td><![endif]-->
<!--[if (mso)|(IE)]><td align=""center"" width=""200"" style=""width: 200px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;"" valign=""top""><![endif]-->
<div class=""u-col u-col-33p33"" style=""max-width: 320px;min-width: 200px;display: table-cell;vertical-align: top;"">
  <div style=""height: 100%;width: 100% !important;"">
  <!--[if (!mso)&(!IE)]><!--><div style=""box-sizing: border-box; height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;""><!--<![endif]-->
  
<table style=""font-family:'Lato',sans-serif;"" role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" border=""0"">
  <tbody>
    <tr>
      <td style=""overflow-wrap:break-word;word-break:break-word;padding:40px 20px 20px;font-family:'Lato',sans-serif;"" align=""left"">
        
  <div style=""color: #333333; line-height: 120%; text-align: left; word-wrap: break-word;"">
    <p style=""font-size: 14px; line-height: 120%;""><span style=""font-size: 24px; line-height: 28.8px;""><strong><span style=""line-height: 28.8px; font-size: 24px;"">S/. ${{subT}}</span></strong></span></p>
  </div>

      </td>
    </tr>
  </tbody>
</table>

  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->
  </div>
</div>
<!--[if (mso)|(IE)]></td><![endif]-->
      <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->
    </div>
  </div>
</div>



<div class=""u-row-container"" style=""padding: 0px 10px;background-color: rgba(255,255,255,0)"">
  <div class=""u-row"" style=""Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #ffffff;"">
    <div style=""border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;"">
      <!--[if (mso)|(IE)]><table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""><tr><td style=""padding: 0px 10px;background-color: rgba(255,255,255,0);"" align=""center""><table cellpadding=""0"" cellspacing=""0"" border=""0"" style=""width:600px;""><tr style=""background-color: #ffffff;""><![endif]-->
      
<!--[if (mso)|(IE)]><td align=""center"" width=""600"" style=""width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;"" valign=""top""><![endif]-->
<div class=""u-col u-col-100"" style=""max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;"">
  <div style=""height: 100%;width: 100% !important;"">
  <!--[if (!mso)&(!IE)]><!--><div style=""box-sizing: border-box; height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;""><!--<![endif]-->
  
<table style=""font-family:'Lato',sans-serif;"" role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" border=""0"">
  <tbody>
    <tr>
      <td style=""overflow-wrap:break-word;word-break:break-word;padding:0px;font-family:'Lato',sans-serif;"" align=""left"">
        
  <table height=""0px"" align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;border-top: 1px solid #e3e3e3;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%"">
    <tbody>
      <tr style=""vertical-align: top"">
        <td style=""word-break: break-word;border-collapse: collapse !important;vertical-align: top;font-size: 0px;line-height: 0px;mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%"">
          <span>&#160;</span>
        </td>
      </tr>
    </tbody>
  </table>

      </td>
    </tr>
  </tbody>
</table>

  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->
  </div>
</div>
<!--[if (mso)|(IE)]></td><![endif]-->
      <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->
    </div>
  </div>
</div>



<div class=""u-row-container"" style=""padding: 0px 10px;background-color: rgba(255,255,255,0)"">
  <div class=""u-row"" style=""Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #ffffff;"">
    <div style=""border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;"">
      <!--[if (mso)|(IE)]><table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""><tr><td style=""padding: 0px 10px;background-color: rgba(255,255,255,0);"" align=""center""><table cellpadding=""0"" cellspacing=""0"" border=""0"" style=""width:600px;""><tr style=""background-color: #ffffff;""><![endif]-->
      
<!--[if (mso)|(IE)]><td align=""center"" width=""400"" style=""width: 400px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;"" valign=""top""><![endif]-->
<div class=""u-col u-col-66p67"" style=""max-width: 320px;min-width: 400px;display: table-cell;vertical-align: top;"">
  <div style=""height: 100%;width: 100% !important;"">
  <!--[if (!mso)&(!IE)]><!--><div style=""box-sizing: border-box; height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;""><!--<![endif]-->
  
<table style=""font-family:'Lato',sans-serif;"" role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" border=""0"">
  <tbody>
    <tr>
      <td style=""overflow-wrap:break-word;word-break:break-word;padding:28px 20px 20px;font-family:'Lato',sans-serif;"" align=""left"">
        
  <div style=""color: #333333; line-height: 140%; text-align: left; word-wrap: break-word;"">
    <p style=""font-size: 14px; line-height: 140%;""><span style=""color: #000000; font-family: 'Open Sans', Arial, sans-serif; text-align: justify; font-size: 18px; line-height: 25.2px;"">Descuento</span></p>
  </div>

      </td>
    </tr>
  </tbody>
</table>

  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->
  </div>
</div>
<!--[if (mso)|(IE)]></td><![endif]-->
<!--[if (mso)|(IE)]><td align=""center"" width=""200"" style=""width: 200px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;"" valign=""top""><![endif]-->
<div class=""u-col u-col-33p33"" style=""max-width: 320px;min-width: 200px;display: table-cell;vertical-align: top;"">
  <div style=""height: 100%;width: 100% !important;"">
  <!--[if (!mso)&(!IE)]><!--><div style=""box-sizing: border-box; height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;""><!--<![endif]-->
  
<table style=""font-family:'Lato',sans-serif;"" role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" border=""0"">
  <tbody>
    <tr>
      <td style=""overflow-wrap:break-word;word-break:break-word;padding:40px 20px 20px;font-family:'Lato',sans-serif;"" align=""left"">
        
  <div style=""color: #333333; line-height: 120%; text-align: left; word-wrap: break-word;"">
    <p style=""font-size: 14px; line-height: 120%;""><span style=""font-size: 24px; line-height: 28.8px;""><strong><span style=""line-height: 28.8px; font-size: 24px;"">S/. ${{dsct}}</span></strong></span></p>
  </div>

      </td>
    </tr>
  </tbody>
</table>

  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->
  </div>
</div>
<!--[if (mso)|(IE)]></td><![endif]-->
      <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->
    </div>
  </div>
</div>



<div class=""u-row-container"" style=""padding: 0px 10px;background-color: rgba(255,255,255,0)"">
  <div class=""u-row"" style=""Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #ffffff;"">
    <div style=""border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;"">
      <!--[if (mso)|(IE)]><table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""><tr><td style=""padding: 0px 10px;background-color: rgba(255,255,255,0);"" align=""center""><table cellpadding=""0"" cellspacing=""0"" border=""0"" style=""width:600px;""><tr style=""background-color: #ffffff;""><![endif]-->
      
<!--[if (mso)|(IE)]><td align=""center"" width=""600"" style=""width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;"" valign=""top""><![endif]-->
<div class=""u-col u-col-100"" style=""max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;"">
  <div style=""height: 100%;width: 100% !important;"">
  <!--[if (!mso)&(!IE)]><!--><div style=""box-sizing: border-box; height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;""><!--<![endif]-->
  
<table style=""font-family:'Lato',sans-serif;"" role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" border=""0"">
  <tbody>
    <tr>
      <td style=""overflow-wrap:break-word;word-break:break-word;padding:0px;font-family:'Lato',sans-serif;"" align=""left"">
        
  <table height=""0px"" align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;border-top: 1px solid #e3e3e3;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%"">
    <tbody>
      <tr style=""vertical-align: top"">
        <td style=""word-break: break-word;border-collapse: collapse !important;vertical-align: top;font-size: 0px;line-height: 0px;mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%"">
          <span>&#160;</span>
        </td>
      </tr>
    </tbody>
  </table>

      </td>
    </tr>
  </tbody>
</table>

  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->
  </div>
</div>
<!--[if (mso)|(IE)]></td><![endif]-->
      <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->
    </div>
  </div>
</div>



<div class=""u-row-container"" style=""padding: 0px 10px;background-color: rgba(255,255,255,0)"">
  <div class=""u-row"" style=""Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #ffffff;"">
    <div style=""border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;"">
      <!--[if (mso)|(IE)]><table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""><tr><td style=""padding: 0px 10px;background-color: rgba(255,255,255,0);"" align=""center""><table cellpadding=""0"" cellspacing=""0"" border=""0"" style=""width:600px;""><tr style=""background-color: #ffffff;""><![endif]-->
      
<!--[if (mso)|(IE)]><td align=""center"" width=""400"" style=""width: 400px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;"" valign=""top""><![endif]-->
<div class=""u-col u-col-66p67"" style=""max-width: 320px;min-width: 400px;display: table-cell;vertical-align: top;"">
  <div style=""height: 100%;width: 100% !important;"">
  <!--[if (!mso)&(!IE)]><!--><div style=""box-sizing: border-box; height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;""><!--<![endif]-->
  
<table style=""font-family:'Lato',sans-serif;"" role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" border=""0"">
  <tbody>
    <tr>
      <td style=""overflow-wrap:break-word;word-break:break-word;padding:28px 20px 25px;font-family:'Lato',sans-serif;"" align=""left"">
        
  <div style=""color: #333333; line-height: 140%; text-align: left; word-wrap: break-word;"">
    <p style=""font-size: 14px; line-height: 140%;""><span style=""font-size: 24px; line-height: 33.6px;""><strong><span style=""line-height: 33.6px; font-size: 24px;"">Total</span></strong></span></p>
<p style=""font-size: 14px; line-height: 140%;""><span style=""font-size: 16px; line-height: 22.4px; color: #17c297;"">Metodo de Pago: ${{mPago}}</span></p>
  </div>

      </td>
    </tr>
  </tbody>
</table>

  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->
  </div>
</div>
<!--[if (mso)|(IE)]></td><![endif]-->
<!--[if (mso)|(IE)]><td align=""center"" width=""200"" style=""width: 200px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;"" valign=""top""><![endif]-->
<div class=""u-col u-col-33p33"" style=""max-width: 320px;min-width: 200px;display: table-cell;vertical-align: top;"">
  <div style=""height: 100%;width: 100% !important;"">
  <!--[if (!mso)&(!IE)]><!--><div style=""box-sizing: border-box; height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;""><!--<![endif]-->
  
<table style=""font-family:'Lato',sans-serif;"" role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" border=""0"">
  <tbody>
    <tr>
      <td style=""overflow-wrap:break-word;word-break:break-word;padding:35px 20px 20px;font-family:'Lato',sans-serif;"" align=""left"">
        
  <div style=""color: #333333; line-height: 120%; text-align: left; word-wrap: break-word;"">
    <p style=""font-size: 14px; line-height: 120%;""><strong><span style=""font-size: 30px; line-height: 36px;"">S/. ${{total}}</span></strong></p>
  </div>

      </td>
    </tr>
  </tbody>
</table>

  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->
  </div>
</div>
<!--[if (mso)|(IE)]></td><![endif]-->
      <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->
    </div>
  </div>
</div>



<div class=""u-row-container"" style=""padding: 0px 10px 20px;background-color: rgba(255,255,255,0)"">
  <div class=""u-row"" style=""Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #17c297;"">
    <div style=""border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;"">
      <!--[if (mso)|(IE)]><table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""><tr><td style=""padding: 0px 10px 20px;background-color: rgba(255,255,255,0);"" align=""center""><table cellpadding=""0"" cellspacing=""0"" border=""0"" style=""width:600px;""><tr style=""background-color: #17c297;""><![endif]-->
      
<!--[if (mso)|(IE)]><td align=""center"" width=""600"" style=""width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;"" valign=""top""><![endif]-->
<div class=""u-col u-col-100"" style=""max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;"">
  <div style=""height: 100%;width: 100% !important;"">
  <!--[if (!mso)&(!IE)]><!--><div style=""box-sizing: border-box; height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;""><!--<![endif]-->
  
<table style=""font-family:'Lato',sans-serif;"" role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" border=""0"">
  <tbody>
    <tr>
      <td style=""overflow-wrap:break-word;word-break:break-word;padding:30px 20px;font-family:'Lato',sans-serif;"" align=""left"">
        
  <div style=""color: #ffffff; line-height: 140%; text-align: center; word-wrap: break-word;"">
    <p style=""font-size: 14px; line-height: 140%;"">Para cualquier tipo de devolución, reembolso, cancelación, etc. Por favor consultar en la sección 'Reservas' de la página web.</p>
<p style=""font-size: 14px; line-height: 140%;"">&nbsp;</p>
<p style=""font-size: 14px; line-height: 140%;"">&nbsp;www.Busseando.com</p>
<p style=""font-size: 14px; line-height: 140%;"">Política de devoluciones | Viaje </p>
  </div>

      </td>
    </tr>
  </tbody>
</table>

  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->
  </div>
</div>
<!--[if (mso)|(IE)]></td><![endif]-->
      <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->
    </div>
  </div>
</div>



<div class=""u-row-container"" style=""padding: 30px;background-color: #f0f0f0"">
  <div class=""u-row"" style=""Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;"">
    <div style=""border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;"">
      <!--[if (mso)|(IE)]><table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""><tr><td style=""padding: 30px;background-color: #f0f0f0;"" align=""center""><table cellpadding=""0"" cellspacing=""0"" border=""0"" style=""width:600px;""><tr style=""background-color: transparent;""><![endif]-->
      
<!--[if (mso)|(IE)]><td align=""center"" width=""600"" style=""width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;"" valign=""top""><![endif]-->
<div class=""u-col u-col-100"" style=""max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;"">
  <div style=""height: 100%;width: 100% !important;"">
  <!--[if (!mso)&(!IE)]><!--><div style=""box-sizing: border-box; height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;""><!--<![endif]-->
  
<table style=""font-family:'Lato',sans-serif;"" role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" border=""0"">
  <tbody>
    <tr>
      <td style=""overflow-wrap:break-word;word-break:break-word;padding:20px;font-family:'Lato',sans-serif;"" align=""left"">
        
  <div style=""line-height: 120%; text-align: left; word-wrap: break-word;"">
    <div style=""font-family: arial, helvetica, sans-serif;""><span style=""font-size: 12px; color: #999999; line-height: 14.4px;"">You received this email because you signed up for My Company Inc.</span></div>
<div style=""font-family: arial, helvetica, sans-serif;"">&nbsp;</div>
<div style=""font-family: arial, helvetica, sans-serif;""><span style=""font-size: 12px; color: #999999; line-height: 14.4px;"">Unsubscribe</span></div>
  </div>

      </td>
    </tr>
  </tbody>
</table>

  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->
  </div>
</div>
<!--[if (mso)|(IE)]></td><![endif]-->
      <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->
    </div>
  </div>
</div>


    <!--[if (mso)|(IE)]></td></tr></table><![endif]-->
    </td>
  </tr>
  </tbody>
  </table>
  <!--[if mso]></div><![endif]-->
  <!--[if IE]></div><![endif]-->
</body>

</html>

";


                body = body.Replace("${{puntoP}}", puntoP);
                body = body.Replace("${{puntoF}}", puntoF);
                body = body.Replace("${{nom}}", nom);
                body = body.Replace("${{dni}}", dni);
                body = body.Replace("${{codA}}", codA);
                body = body.Replace("${{subT}}", subT);
                body = body.Replace("${{mPago}}", mPago);
                body = body.Replace("${{igvVenta}}", igvVenta);
                body = body.Replace("${{fechaV}}", fechaV);
                body = body.Replace("${{dsct}}", dsct);

                body = body.Replace("${{total}}", total);
                body = body.Replace("${{fechaViaje}}", fechaViaje);
                body = body.Replace("${{hSalida}}", hSalida);





                /*"<html>" +
                "<body><h1>¡Gracias por su compra!</h1>" +
          "<p>Con este correo podrás garantizar tu embarque el día del viaje. Tendrás que mostrar los datos el día de revisión para que se compruebe que la persona que va a viajar sea realmente tú. Con eso sería todo, ¡Muchas gracias por confiar en Busseando!</p>" +
          "<h2>Detalles de la compra:</h2>" +
          $"<p>Nombre de pasajero: {nom}</p>" +
          $"<p>Dni de pasajero: {dni}</p>" +
          $"<p>Asiento asignado: {codA}</p>" +
          $"<p>Fecha de Venta: {fechaV}</p>" +
          $"<p>Subtotal: {subT}</p>" +
          $"<p>Método de Pago: {mPago}</p>" +
          $"<p>Igv: {igvVenta}</p>" +
          $"<p>Descuento: {dsct}</p>" +
          $"<p>Total: {total}</p>" +
          $"<h2>Detalles del viaje:</h2>" +
          $"<p>Fecha de viaje: {fechaViaje}</p>" +
          $"<p>Hora de salida: {hSalida}</p>" +
          $"<p>Punto de Partida: {puntoP}</p>" +
          $"<p>Punto de Llegada: {puntoF}</p>" +
          "<p>Para cualquier tipo de devolución, reembolso, cancelación, etc. Por favor consultar en la sección 'Reservas' de la página web.</p>" +
          "</body>" +
          "</html>";*/

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body =  body,
                    IsBodyHtml = true

                })
                {
                    smtp.Send(message);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private bool VentaExists(int idAsiento)
        {
            return _context.TbDetalleVenta.Any(e => (e.Idasiento == idAsiento && e.Estado == true));
        }
    }
}
