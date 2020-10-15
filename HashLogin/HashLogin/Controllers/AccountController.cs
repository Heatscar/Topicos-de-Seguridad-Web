using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HashLogin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HashLogin.Controllers
{
    public class AccountController : Controller
    {
        private readonly hashtopicosContext _context;
        public AccountController(hashtopicosContext context)
        {
            _context = context;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Login(Users model)
        {
            if (ModelState.IsValid)
            {
                var userdetails = await _context.Users.SingleOrDefaultAsync(m => m.Email == model.Email && m.Password == model.Password);
                if (userdetails == null)
                {
                    ModelState.AddModelError("Password", "Invalid login attempt.");
                    return View("Index");
                }
                HttpContext.Session.SetString("IdUser", userdetails.Username);

            }
            else
            {
                return View("Index");
            }
            return RedirectToAction("Index", "Home");
        }


        public IActionResult Profile()
        {
            ViewBag.mail = HttpContext.Session.GetString("mail");
            ViewBag.data = HttpContext.Session.GetString("id");
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return View("Login");
        }

        public ActionResult Validate(Users admin)
        {
            var _admin = _context.Users.Where(s => s.Email == admin.Email).FirstOrDefault();
            if (_admin != null && _admin.Password != null)
            {
                String test1 = String.Join(",", Split(admin.Password, 4));
                // StrongPass

                string result = String.Concat(test1.Select(x => ((int)x).ToString("x")));
                if (result == _admin.Password)
                {
                    //HttpContext.Session.SetString("mail", admin.Email);
                    //HttpContext.Session.SetString("id", _admin.Id.ToString());
                    
                    return Json(new { status = true, message = "Inicio de sesión exitoso!" });
                }
                else
                {
                    return Json(new { status = false, message = "Contraseña Invalida!" });
                }
            }
            else
            {
                return Json(new { status = false, message = "Email invalido!" });
            }
        }

        public static IEnumerable<String> Split(String value, int chunkSize)
        {
            if (null == value)
                throw new ArgumentNullException("value");
            else if (chunkSize <= 0)
                throw new ArgumentOutOfRangeException("Fragmento", "El tamaño del fragmento debe ser positivo");

            return Enumerable
            .Range(0, value.Length / chunkSize + ((value.Length % chunkSize) == 0 ? 0 : 1))
            .Select(index => (index + 1) * chunkSize < value.Length
             ? value.Substring(index * chunkSize, chunkSize)
             : value.Substring(index * chunkSize));
        }
    }
}
