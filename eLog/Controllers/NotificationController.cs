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
        public async Task<IActionResult> GetNotifications(int userId)
        {
            try
            {
                List<object> notifications = new List<object>();

                using (SqlConnection connection = _db.CreateConnection())
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("proc_GetNotifications", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserId", userId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                notifications.Add(new
                                {
                                    id = reader.GetInt32("Id"),
                                    message = reader.GetString("Message"),
                                    isRead = reader.GetBoolean("IsRead"),
                                    entryDate = reader.GetDateTime("EntryDate").ToString("yyyy-MM-dd HH:mm:ss")
                                });
                            }
                        }
                    }
                }

                return Json(new { success = true, notifications });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetNotifications: {ex.Message}");
                return Json(new { success = false, message = "Error fetching notifications.", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetNotificationCount(int userId)
        {
            try
            {
                int unreadCount = 0;
                int totalCount = 0;

                using (SqlConnection connection = _db.CreateConnection())
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("proc_GetNotificationCount", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserId", userId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                unreadCount = reader.GetInt32("UnreadCount");
                                totalCount = reader.GetInt32("TotalCount");
                            }
                        }
                    }
                }

                return Json(new { unreadCount, totalCount });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetNotificationCount: {ex.Message}");
                return Json(new { unreadCount = 0, totalCount = 0, error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int notificationId, int userId)
        {
            try
            {
                using (SqlConnection connection = _db.CreateConnection())
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("proc_MarkNotificationAsRead", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@NotificationId", notificationId);
                        command.Parameters.AddWithValue("@UserId", userId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                bool success = reader.GetInt32("Success") == 1;
                                string message = reader.GetString("Message");

                                return Json(new { success, message });
                            }
                        }
                    }
                }

                return Json(new { success = false, message = "No response from database" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in MarkAsRead: {ex.Message}");
                return Json(new { success = false, message = "Error marking notification as read", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> MarkAllAsRead(int userId)
        {
            try
            {
                using (SqlConnection connection = _db.CreateConnection())
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("proc_MarkAllNotificationsAsRead", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserId", userId);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        return Json(new { success = true, message = $"{rowsAffected} notifications marked as read" });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in MarkAllAsRead: {ex.Message}");
                return Json(new { success = false, message = "Error marking all notifications as read", error = ex.Message });
            }
        }
    }
}