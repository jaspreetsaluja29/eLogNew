using eLog.Models.ORB1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace eLog.Controllers.ORB1
{
    public class FirstPageCapacityController : Controller
    {
        private readonly DatabaseHelper _db;

        public FirstPageCapacityController(DatabaseHelper db)
        {
            _db = db;
        }
        public async Task<IActionResult> FirstPageCapacity()
        {
            // return View("~/Views/ORB1/FirstPageCapacity.cshtml");

            List<FirstPage_OilyBilgeRetention> retentionData = new List<FirstPage_OilyBilgeRetention>();

            using (SqlConnection connection = _db.CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("proc_GetORB1_FirstPageOilyBilgeRetention", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            retentionData.Add(new FirstPage_OilyBilgeRetention
                            {
                                Id = reader.GetInt32(0),
                                TankIdentification = reader.GetString(1),
                                TankLocation_Frames_From_To = reader.GetString(2),
                                Volume_m3 = reader.GetDecimal(3)
                            });
                        }
                    }
                }
            }

            // Check if data exists — if not, return empty form
            if (retentionData.Any())
            {
                return View("~/Views/ORB1/Display_FirstPageCapacity.cshtml", retentionData); // Show existing data
            }
            else
            {
                return View("~/Views/ORB1/FirstPageCapacity.cshtml"); // Show empty form
            }
        }

        [HttpPost]
        public IActionResult SaveData(List<FirstPage_OilyBilgeRetention> retentions)
        {
            if (retentions == null || !retentions.Any())
            {
                return BadRequest("No data received.");
            }

            using (SqlConnection connection = _db.CreateConnection())
            {
                connection.Open();
                foreach (var retention in retentions)
                {
                    var query = @"
                INSERT INTO ORB1_FirstPage_OilyBilgeRetention 
                (TankIdentification, TankLocation_Frames_From_To, Volume_m3) 
                VALUES (@TankIdentification, @TankLocation, @Volume)";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@TankIdentification", retention.TankIdentification ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@TankLocation", retention.TankLocation_Frames_From_To ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Volume", (object)retention.Volume_m3);
                        command.ExecuteNonQuery();
                    }
                }
            }
            return Json(new { success = true });
        }

        //[HttpPost]
        //public IActionResult SaveData(List<FirstPage_OilyBilgeRetention> retentions)
        //{
        //    if (retentions == null || !retentions.Any())
        //    {
        //        return BadRequest("No data received.");
        //    }

        //    using (SqlConnection connection = _db.CreateConnection())
        //    {
        //        connection.Open();

        //        // Check if data already exists
        //        var checkQuery = "SELECT COUNT(1) FROM ORB1_FirstPage_OilyBilgeRetention";
        //        using (var checkCommand = new SqlCommand(checkQuery, connection))
        //        {
        //            int existingRecords = (int)checkCommand.ExecuteScalar();
        //            if (existingRecords > 0)
        //            {
        //                return Json(new { success = false, message = "Data already exists. You cannot enter data again." });
        //            }
        //        }

        //        // If no data exists, insert the new data
        //        foreach (var retention in retentions)
        //        {
        //            var insertQuery = @"
        //    INSERT INTO ORB1_FirstPage_OilyBilgeRetention 
        //    (TankIdentification, TankLocation_Frames_From_To, Volume_m3) 
        //    VALUES (@TankIdentification, @TankLocation, @Volume)";

        //            using (var insertCommand = new SqlCommand(insertQuery, connection))
        //            {
        //                insertCommand.Parameters.AddWithValue("@TankIdentification", retention.TankIdentification ?? (object)DBNull.Value);
        //                insertCommand.Parameters.AddWithValue("@TankLocation", retention.TankLocation_Frames_From_To ?? (object)DBNull.Value);
        //                insertCommand.Parameters.AddWithValue("@Volume", (object)retention.Volume_m3);
        //                insertCommand.ExecuteNonQuery();
        //            }
        //        }
        //    }
        //    return Json(new { success = true });
        //}
    }
}
