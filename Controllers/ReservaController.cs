using MVCSistemaLibros.ServiceSistemaLibros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using MVCSistemaLibros.Models;

namespace MVCSistemaLibros.Controllers
{
    public class ReservaController : Controller
    {
        // GET: Reserva
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult listadoTablaJs(string varCode)
        {
            Service1Client service1Client = new Service1Client();
            DtoLibro[] lstLibrosDto = service1Client.SP_LISTARVISTALIBROS(varCode);
            return Json(lstLibrosDto, JsonRequestBehavior.AllowGet);
        }

        public JsonResult validarReservaJs(string varCode)
        {
            Service1Client service1Client = new Service1Client();
            int validar = service1Client.SP_VALIDARRESERVAXLIBRO(varCode);
            /*
            if (validar > 0) 
            {
                if (service1Client.SP_VALIDARRESERVAXUSUARIOXLIBRO((int)Session["idUser"], varCode) == 0) validar = 2;
            }    */
            return Json(new { resultado = validar }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult registrarReserva(DtoReserva dtoReserva)
        {
            try
            {
                Service1Client service1Client = new Service1Client();
                int validar = service1Client.SP_VALIDARRESERVAXLIBRO(dtoReserva.varCode);
                Book book = service1Client.SP_BUSCARLIBROXCODE(dtoReserva.varCode);
                Reservation reservation = new Reservation();
                if (validar == 0)
                {
                    //Reserva normal
                    
                    reservation.dmeDateReservation = DateTime.Now;
                    reservation.dmeDateCreate = DateTime.Now;
                    reservation.bolIsActive = true;
                    reservation.intStatus = 1;
                    reservation.idUser = dtoReserva.idUser;
                    reservation.idBook = book.idBook;
                    service1Client.SP_RESERVARLIBRO(reservation);
                }
                else
                {
                    validar = service1Client.SP_VALIDARRESERVAXUSUARIOXLIBRO((int)Session["idUser"], dtoReserva.varCode); //Si la reserva es del usuario actual 
                    if (validar == 0)
                    {
                        validar = service1Client.SP_VALIDARCOLA((int)Session["idUser"], book.idBook);
                        if(validar == 0) //SI NO HAY EXISTE COLA DEL LIBRO CON EL USUARIO Y EL LIBRO
                        {
                            service1Client.SP_RESERVARCOLA((int)Session["idUser"], book.idBook);
                            return Json(new { resultado = "USTED SE ENCUENTRA EN LA LISTA DE ESPERA" }, JsonRequestBehavior.AllowGet);
                        }
                        else if(validar >= 3) //SI SUPERA MAS DE 3 EN COLA POR UN LIBRO
                        {
                            return Json(new { resultado = "NO SE PUEDE RESERVAR EL LIBRO, NUMERO DE RESERVAS EXCEDIDOS" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            //SI EL USUARIO ESTA EN COLA DEL LIBRO ACTUAL
                            return Json(new { resultado = "USTED SE ENCUENTRA EN LA LISTA DE ESPERA, POR FAVOR ESPERAR SU LIBERACION" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { resultado = "NO ES POSIBLE RESERVAR EL LIBRO TITULO DEL LIBRO, YA HA SIDO RESERVADO POR USTED" }, JsonRequestBehavior.AllowGet);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                return Json(new { resultado = ex.Message }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { resultado = 1 }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult listarNotificaciones()
        { 
            Service1Client service1Client = new Service1Client();
            MensajeNotificacion[] listadoNotificaciones = service1Client.SP_LISTARNOTIFICACIONESXUSUARIO((int)Session["idUser"]);
            return Json(listadoNotificaciones, JsonRequestBehavior.AllowGet);
        }
    }

}