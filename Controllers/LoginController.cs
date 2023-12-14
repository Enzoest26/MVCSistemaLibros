using Microsoft.IdentityModel.Tokens;
using MVCSistemaLibros.Models;
using MVCSistemaLibros.ServiceSistemaLibros;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MVCSistemaLibros.Controllers
{
    public class LoginController : Controller
    {
        private string llave_secreta = "AEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAE";
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult login(Login login)
        {
            Service1Client service1Client = new Service1Client();
            User user = service1Client.SP_VALIDARACCESO(login.username, login.password);
            var symmetricKey = Encoding.UTF8.GetBytes(llave_secreta);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, login.username)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var token2 = tokenHandler.WriteToken(token);
            if (user.idUser != 0) //No existe
            {
                return Json(new { userId = user.idUser, token = token2 }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { error = "Acceso denegado" }, JsonRequestBehavior.AllowGet);
        }
    }
}