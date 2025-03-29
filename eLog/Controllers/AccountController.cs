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

        public AccountController(DatabaseHelper db)
        {
            _db = db;
        }

        public IActionResult GetSession()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            var userName = HttpContext.Session.GetString("UserName");
            var userRoleName = HttpContext.Session.GetString("UserRoleName");

            return Content($"UserID: {userId}, UserName: {userName}, UserRoleName: {userRoleName}");
        }

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

            // Fetch UserID and Role from the database
            int userId = Convert.ToInt32(dt.Rows[0]["UserID"]);
            int userRoleId = Convert.ToInt32(dt.Rows[0]["UserRoleId"]);
            string userRoleName = string.Empty;
            if (userRoleId == 1)
            {
                userRoleName = "SuperAdmin";
            }
            if (userRoleId == 2)
            {
                userRoleName = "Level 2- Approver";
            }
            if (userRoleId == 3)
            {
                userRoleName = "Level 1- Entry";
            }

            // Set session values
            HttpContext.Session.SetInt32("UserID", userId);
            HttpContext.Session.SetString("UserName", username);
            HttpContext.Session.SetString("UserRoleName", userRoleName);

            TempData["UserID"] = userId;
            TempData["UserName"] = username;
            TempData["UserRoleName"] = userRoleName;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, userRoleName)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            if (userRoleId == 1)
            {
                return RedirectToAction("GetISMCompanyDetails", "ISMCompanyDetails");
            }
            else
            {
                return RedirectToAction("FirstPageCapacity", "FirstPageCapacity");
            }
        }

        public IActionResult Reset()
        {
            // Clear session on logout
            HttpContext.Session.Clear();
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
