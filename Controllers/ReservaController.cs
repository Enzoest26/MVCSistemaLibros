using MVCSistemaLibros.ServiceSistemaLibros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;

namespace MVCSistemaLibros.Controllers
{
    public class ReservaController : Controller
    {
        // GET: Reserva
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult listadoTablaJs()
        {
            Service1Client service1Client = new Service1Client();
            DtoLibro[] lstLibrosDto = service1Client.SP_LISTARVISTALIBROS();
            return Json(lstLibrosDto, JsonRequestBehavior.AllowGet);
        }

        public JsonResult validarReservaJs(int idLibro)
        {
            Service1Client service1Client = new Service1Client();
            int validar = service1Client.SP_VALIDARRESERVA(idLibro);
            return Json(new { resultado = validar }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult registrarReserva(Reservation reservation)
        {
            try
            {
                Service1Client service1Client = new Service1Client();
                int validar = service1Client.SP_VALIDARRESERVA(reservation.idBook);
                if(validar == 0)
                {
                    reservation.dmeDateReservation = DateTime.Now;
                    reservation.dmeDateCreate = DateTime.Now;
                    reservation.bolIsActive = true;
                    reservation.intStatus = 1;
                    service1Client.SP_RESERVARLIBRO(reservation);
                }
                else
                {
                    return Json(new { resultado = "NO ES POSIBLE RESERVAR EL LIBRO TITULO DEL LIBRO, YA HA SIDO RESERVADO" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { resultado = ex.Message }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { resultado = 1 }, JsonRequestBehavior.AllowGet);
        }
    }
}