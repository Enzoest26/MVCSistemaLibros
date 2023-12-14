using Microsoft.IdentityModel.Tokens;
using MVCSistemaLibros.Models;
using MVCSistemaLibros.ServiceSistemaLibros;
using System;
using System.Collections.Generic;
using System.Configuration;
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
            if (user == null || user.idUser < 1)
            {
                return Json(new { error = "Acceso denegado" }, JsonRequestBehavior.AllowGet);
            }
            List<Claim> claims = new List<Claim>
            {
                new Claim("name", user.varEmail),
            };
            var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(llave_secreta));
            

            var cred = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256Signature);
            //var cred = new SigningCredentials(symmetricKey, "HS256") --- validar;
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: cred);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return Json(new { userId = user.idUser, token = jwt }, JsonRequestBehavior.AllowGet);
           
        }
    }
}