using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Net.Mail;
using System.Net;

namespace eLog.Controllers
{
    public class AccountController : Controller
    {
        private readonly DatabaseHelper _db;
        private readonly IConfiguration _configuration;

        public AccountController(DatabaseHelper db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
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

            // Fetch UserID and Role Name
            int userId = Convert.ToInt32(dt.Rows[0]["UserID"]);
            string userRoleName = dt.Rows[0]["UserRoleName"]?.ToString() ?? string.Empty;

            // Set session values
            HttpContext.Session.SetInt32("UserID", userId);
            HttpContext.Session.SetString("UserName", username);
            HttpContext.Session.SetString("UserRoleName", userRoleName);

            // Store TempData for OTP verification
            TempData["UserID"] = userId;
            TempData["UserName"] = username;
            TempData["UserRoleName"] = userRoleName;

            // Check if OTP is enabled
            bool isOtpEnabled = Convert.ToBoolean(_configuration["EnableOTP"]);
            ViewBag.EnableOTP = isOtpEnabled;
            if (isOtpEnabled)
            {
                // Generate OTP
                Random random = new Random();
                int otp = random.Next(100000, 999999);
                int otpTimeout = Convert.ToInt32(_configuration["OTPTimeout"]);
                DateTime expiryTime = DateTime.UtcNow.AddSeconds(otpTimeout);

                // Store OTP in the database
                string otpQuery = "INSERT INTO UserOTP (UserID, OTP, ExpiryTime) VALUES (@UserID, @OTP, @ExpiryTime)";
                var otpParams = new SqlParameter[]
                {
                new SqlParameter("@UserID", userId),
                new SqlParameter("@OTP", otp.ToString()),
                new SqlParameter("@ExpiryTime", expiryTime)
                };
                _db.ExecuteQuery(otpQuery, otpParams);

                // Send OTP to User
                string message = SendOtpToUser(username, otp);

                ViewBag.SuccessMessage = $"{message} Please enter OTP to verify.";
                ViewBag.OTPExpiryTime = otpTimeout;
                return View();
            }
            else
            {
                if (TempData["UserRoleName"]?.ToString() == "SuperAdmin")
                {
                    return RedirectToAction("GetISMCompanyDetails", "ISMCompanyDetails");
                }
                else
                {
                    return RedirectToAction("FirstPageCapacity", "FirstPageCapacity");
                }
            }


        }

        public IActionResult Reset()
        {
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

        // Function for OTP sending to user email
        private string SendOtpToUser(string username, int otp)
        {
            string message = string.Empty;
            // Fetch user email from database
            string storedProcedureName = "GetUserEmail";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Username", username)
            };

            DataTable dt = _db.ExecuteStoredProcedure(storedProcedureName, parameters);

            if (dt.Rows.Count == 0)
            {
                message = $"Email not found for the user {username}";
                return message;
            }

            string userEmail = dt.Rows[0]["Email"].ToString();

            // SMTP Configuration
            string smtpHost = _configuration["Email:smtpHost"] ?? string.Empty;
            int smtpPort = Convert.ToInt32(_configuration["Email:smtpPort"]);
            string fromEmail = _configuration["Email:fromEmail"] ?? string.Empty;
            string fromPassword = _configuration["Email:fromEmailPassword"] ?? string.Empty;
            int otpTimeout = Convert.ToInt32(_configuration["OTPTimeout"]);

            try
            {
                using (SmtpClient smtpClient = new SmtpClient(smtpHost, smtpPort))
                {
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential(fromEmail, fromPassword);
                    smtpClient.EnableSsl = true;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                    MailMessage mailMessage = new MailMessage
                    {
                        From = new MailAddress(fromEmail),
                        Subject = _configuration["Email:Subject"] ?? "Your OTP Code",
                        Body = $"Dear {username},\n\nYour OTP for login is: {otp}\n\nThis OTP is valid for {otpTimeout} seconds.\n\nRegards,\nDigitalLog Team",
                        IsBodyHtml = false
                    };

                    mailMessage.To.Add(userEmail);
                    smtpClient.Send(mailMessage);
                }
                message = $"OTP sent successfully to user {username} with Email {userEmail}.";
                return message;
            }
            catch (Exception ex)
            {
                message = $"Error sending OTP email: {ex.Message}";
                return message;
            }
        }

        [HttpPost]
        public IActionResult VerifyOtp(string otp)
        {
            if (TempData["UserID"] == null)
            {
                return RedirectToAction("Login");
            }

            int userId = (int)TempData["UserID"];

            string storedProcedure = "VerifyUserOTP";

            var otpParams = new SqlParameter[]
            {
                new SqlParameter("@UserID", userId),
                new SqlParameter("@OTP", otp)
            };

            // Call stored procedure
            DataTable dt = _db.ExecuteStoredProcedure(storedProcedure, otpParams);

            if (dt.Rows.Count == 0)
            {
                ViewBag.ErrorMessage = "Invalid or Expired OTP.";
                return View();
            }

            // Clear OTP records after successful validation
            string deleteOtpQuery = "DELETE FROM UserOTP WHERE UserID = @UserID";
            var deleteOtpParams = new SqlParameter[]
            {
                new SqlParameter("@UserID", userId),
            };
            _db.ExecuteQuery(deleteOtpQuery, deleteOtpParams);

            // Store session data
            HttpContext.Session.SetInt32("UserID", userId);

            // Log user login time
            string logQuery = "INSERT INTO UserLoginHistory (UserID, UserLoginTime) VALUES (@UserID, GETDATE())";
            var logQueryParams = new SqlParameter[]
            {
                new SqlParameter("@UserID", userId),
            };
            _db.ExecuteQuery(logQuery, logQueryParams);

            // Redirect based on UserRoleName
            if (TempData["UserRoleName"]?.ToString() == "SuperAdmin")
            {
                return RedirectToAction("GetISMCompanyDetails", "ISMCompanyDetails");
            }
            else
            {
                return RedirectToAction("FirstPageCapacity", "FirstPageCapacity");
            }
        }
    }
}
