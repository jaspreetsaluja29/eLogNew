using eLog.Models.ORB1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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
            List <FirstPage_MeanOilResidue> meanOilResidueData = new List<FirstPage_MeanOilResidue>();
            List<FirstPage_BunkerTanks> bunkerTanksData = new List<FirstPage_BunkerTanks>();

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
                                Id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                                TankIdentification = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                                TankLocation_Frames_From_To = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                                TankLocation_LateralPosition = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                                Volume_m3 = reader.IsDBNull(4) ? 0 : reader.GetDecimal(4)
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
                                Id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                                TankIdentification = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                                TankLocation_Frames_From_To = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                                TankLocation_LateralPosition = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                                Volume_m3 = reader.IsDBNull(4) ? 0 : reader.GetDecimal(4)
                            });
                        }
                    }
                }

                // Fetch Mean Oil Residue Data
                using (var command = new SqlCommand("proc_GetORB1_FirstPage_MeanOilResidue", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            meanOilResidueData.Add(new FirstPage_MeanOilResidue
                            {
                                Id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                                TankIdentification = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                                TankLocation_Frames_From_To = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                                TankLocation_LateralPosition = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                                Volume_m3 = reader.IsDBNull(4) ? 0 : reader.GetDecimal(4)
                            });
                        }
                    }
                }

                // Fetching Bunker Tanks Data
                using (var command = new SqlCommand("proc_GetORB1_FirstPage_BunkerTanks", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            bunkerTanksData.Add(new FirstPage_BunkerTanks
                            {
                                Id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                                TankIdentification = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                                Capacity100 = reader.IsDBNull(2) ? 0 : reader.GetDecimal(2),
                                Capacity90 = reader.IsDBNull(2) ? 0 : (reader.GetDecimal(2) * 0.9m)
                            });
                        }
                    }
                }
            }

            var viewModel = new FirstPageCapacityViewModel
            {
                OilyBilgeRetentions = retentionData,
                OilResidueBilges = sludgeData,
                MeanOilResidue = meanOilResidueData,
                BunkerTanks = bunkerTanksData

            };

            if (retentionData.Any() || sludgeData.Any() || meanOilResidueData.Any() || bunkerTanksData.Any())
            {
                return View("~/Views/ORB1/Display_FirstPageCapacity.cshtml", viewModel);
            }
            else
            {
                return View("~/Views/ORB1/FirstPageCapacity.cshtml");
            }
        }

        [HttpPost]
        public IActionResult SaveData(List<FirstPage_OilyBilgeRetention> retentions, List<FirstPage_OilResidueBilge> sludges, List<FirstPage_MeanOilResidue> means, List<FirstPage_BunkerTanks> bunkers)
        {
            if ((retentions == null || !retentions.Any()) && (sludges == null || !sludges.Any()) && (means == null || !means.Any()) && (bunkers == null || !bunkers.Any()))
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
                            (TankIdentification, TankLocation_Frames_From_To,TankLocation_LateralPosition, Volume_m3) 
                            VALUES (@TankIdentification, @TankLocation,@TankLateralPosition, @Volume)";

                        using (var command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@TankIdentification", retention.TankIdentification ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@TankLocation", retention.TankLocation_Frames_From_To ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@TankLateralPosition", retention.TankLocation_LateralPosition ?? (object)DBNull.Value);
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
                            (TankIdentification, TankLocation_Frames_From_To,TankLocation_LateralPosition, Volume_m3) 
                            VALUES (@TankIdentification, @TankLocation,@TankLateralPosition, @Volume)";

                        using (var command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@TankIdentification", sludge.TankIdentification ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@TankLocation", sludge.TankLocation_Frames_From_To ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@TankLateralPosition", sludge.TankLocation_LateralPosition ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Volume", (object)sludge.Volume_m3);
                            command.ExecuteNonQuery();
                        }
                    }
                }

                // Save Oil Residue Bilge (Sludge) data
                if (means != null)
                {
                    foreach (var meanOil in means)
                    {
                        var query = @"
                            INSERT INTO ORB1_FirstPage_MeanOilResidue 
                            (TankIdentification, TankLocation_Frames_From_To,TankLocation_LateralPosition, Volume_m3) 
                            VALUES (@TankIdentification, @TankLocation,@TankLateralPosition, @Volume)";

                        using (var command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@TankIdentification", meanOil.TankIdentification ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@TankLocation", meanOil.TankLocation_Frames_From_To ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@TankLateralPosition", meanOil.TankLocation_LateralPosition ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Volume", (object)meanOil.Volume_m3);
                            command.ExecuteNonQuery();
                        }
                    }
                }

                // Save Bunker Tanks data
                if (bunkers != null)
                {
                    foreach (var bunkertanks in bunkers)
                    {
                        var query = @"
                            INSERT INTO ORB1_FirstPage_BunkerTanks 
                            (TankIdentification, Capacity) 
                            VALUES (@TankIdentification, @Capacity)";

                        using (var command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@TankIdentification", bunkertanks.TankIdentification ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Capacity", (object)bunkertanks.Capacity100);
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            return Json(new { success = true });
        }
    }
}
