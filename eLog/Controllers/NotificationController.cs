using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace eLog.Controllers
{
    public class NotificationController : Controller
    {
        private readonly DatabaseHelper _db;

        public NotificationController(DatabaseHelper db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            try
            {

                List<object> notifications = new List<object>();

                using (SqlConnection connection = _db.CreateConnection()) // Ensure this method exists in DatabaseHelper
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("proc_GetNotification", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                notifications.Add(new
                                {
                                    Message = reader.GetString(0),
                                    IsRead = reader.GetBoolean(1),
                                    EntryDate = reader.GetDateTime(2).ToString("yyyy-MM-dd HH:mm:ss")
                                });
                            }
                        }
                    }
                }

                return Json(new { success = true, notifications });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error fetching notifications.", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetNotificationCount(int userId)
        {
            int count = 0; // Initialize count variable

            using (SqlConnection connection = _db.CreateConnection()) // Ensure this method exists in DatabaseHelper
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("proc_GetNotificationCount", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    count = (int)await command.ExecuteScalarAsync(); // Retrieve the count
                }
            }
            return Json(new { count });
        }
    }
}
