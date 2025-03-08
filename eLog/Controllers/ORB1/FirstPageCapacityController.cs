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
            List<FirstPage_OilyBilgeRetention> retentionData = new List<FirstPage_OilyBilgeRetention>();
            List<FirstPage_OilResidueBilge> sludgeData = new List<FirstPage_OilResidueBilge>();

            using (SqlConnection connection = _db.CreateConnection())
            {
                await connection.OpenAsync();

                // Fetch Oily Bilge Retention data
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

                // Fetch Oil Residue Bilge (Sludge) data
                using (var command = new SqlCommand("proc_GetORB1_FirstPage_OilResidueBilge", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            sludgeData.Add(new FirstPage_OilResidueBilge
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

            var viewModel = new FirstPageCapacityViewModel
            {
                OilyBilgeRetentions = retentionData,
                OilResidueBilges = sludgeData
            };

            if (retentionData.Any() || sludgeData.Any())
            {
                return View("~/Views/ORB1/Display_FirstPageCapacity.cshtml", viewModel);
            }
            else
            {
                return View("~/Views/ORB1/FirstPageCapacity.cshtml");
            }
        }

        [HttpPost]
        public IActionResult SaveData(List<FirstPage_OilyBilgeRetention> retentions, List<FirstPage_OilResidueBilge> sludges)
        {
            if ((retentions == null || !retentions.Any()) && (sludges == null || !sludges.Any()))
            {
                return BadRequest("No data received.");
            }

            using (SqlConnection connection = _db.CreateConnection())
            {
                connection.Open();

                // Save Oily Bilge Retention data
                if (retentions != null)
                {
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

                // Save Oil Residue Bilge (Sludge) data
                if (sludges != null)
                {
                    foreach (var sludge in sludges)
                    {
                        var query = @"
                            INSERT INTO ORB1_FirstPage_OilResidueBilge 
                            (TankIdentification, TankLocation_Frames_From_To, Volume_m3) 
                            VALUES (@TankIdentification, @TankLocation, @Volume)";

                        using (var command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@TankIdentification", sludge.TankIdentification ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@TankLocation", sludge.TankLocation_Frames_From_To ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Volume", (object)sludge.Volume_m3);
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            return Json(new { success = true });
        }
    }
}
