using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Data;
using Microsoft.Data.SqlClient;

namespace eLog.Controllers
{
    public class AccountController : Controller
    {
        private readonly DatabaseHelper _db;
        public AccountController(DatabaseHelper db) { _db = db; }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            string storedProcedure = "proc_UserLogin";

            var parameters = new SqlParameter[]
            {
        new SqlParameter("@Username", username),
        new SqlParameter("@Password", password)
            };

            DataTable dt = _db.ExecuteStoredProcedure(storedProcedure, parameters);

            if (dt.Rows.Count == 0)
            {
                ViewBag.ErrorMessage = "Username/Password Mismatch.";
                return View();
            }

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, username),
        new Claim(ClaimTypes.Role, dt.Rows[0]["UserRoleId"].ToString())
    };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            return RedirectToAction("FirstPageCapacity", "FirstPageCapacity");
        }

        public IActionResult Reset()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            ViewBag.ErrorMessage = "UserLogin Failed";
            return RedirectToAction("Login");
        }
    }
}
