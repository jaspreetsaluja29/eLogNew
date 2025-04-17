using eLog.Models;
using eLog.Models.ORB1;
using eLog.ViewModels.ORB1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace eLog.Controllers.ORB1
{
    public class CodeIController : Controller
    {
        private readonly DatabaseHelper _db;

        public CodeIController(DatabaseHelper db)
        {
            _db = db;
        }

        // Fetch data using stored procedure        
        [Route("ORB1/CodeI/GetCodeIData")]
        [HttpGet]
        public async Task<IActionResult> GetCodeIData(int pageNumber = 1, int pageSize = 10)
        {
            List<CodeIViewModel> records = new List<CodeIViewModel>();
            int totalRecords = 0;

            using (SqlConnection connection = _db.CreateConnection()) // Ensure this method exists in DatabaseHelper
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("proc_GetORB1_CodeI", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            records.Add(new CodeIViewModel
                            {
                                Id = reader.GetInt32(0),
                                UserId = reader.IsDBNull(1) ? null : reader.GetString(1),
                                EntryDate = reader.GetDateTime(2),
                                SelectType = reader.IsDBNull(3) ? null : reader.GetString(3),

                                WeeklyInventoryTanks = reader.IsDBNull(4) ? null : reader.GetString(4),
                                WeeklyInventoryCapacity = reader.IsDBNull(5) ? null : reader.GetDecimal(5),
                                WeeklyInventoryRetained = reader.IsDBNull(6) ? null : reader.GetDecimal(6),

                                DebunkeringQuantity = reader.IsDBNull(7) ? null : reader.GetDecimal(7),
                                DebunkeringGrade = reader.IsDBNull(8) ? null : reader.GetString(8),
                                DebunkeringSulphurContent = reader.IsDBNull(9) ? null : reader.GetString(9),
                                DebunkeringFrom = reader.IsDBNull(10) ? null : reader.GetString(10),
                                DebunkeringQuantityRetained = reader.IsDBNull(11) ? null : reader.GetDecimal(11),
                                DebunkeringTo = reader.IsDBNull(12) ? null : reader.GetString(12),
                                DebunkeringPortFacility = reader.IsDBNull(13) ? null : reader.GetString(13),
                                DebunkeringStartDateTime = reader.IsDBNull(14) ? (DateTime?)null : reader.GetDateTime(14),
                                DebunkeringStopDateTime = reader.IsDBNull(15) ? (DateTime?)null : reader.GetDateTime(15),

                                ValveName = reader.IsDBNull(16) ? null : reader.GetString(16),
                                ValveNo = reader.IsDBNull(17) ? null : reader.GetString(17),
                                ValveAssociatedEquipment = reader.IsDBNull(18) ? null : reader.GetString(18),
                                ValveSealNo = reader.IsDBNull(19) ? null : reader.GetString(19),

                                BreakingValveName = reader.IsDBNull(20) ? null : reader.GetString(20),
                                BreakingValveNo = reader.IsDBNull(21) ? null : reader.GetString(21),
                                BreakingAssociatedEquipment = reader.IsDBNull(22) ? null : reader.GetString(22),
                                BreakingReason = reader.IsDBNull(23) ? null : reader.GetString(23),
                                BreakingSealNo = reader.IsDBNull(24) ? null : reader.GetString(24),

                                StatusName = reader.IsDBNull(25) ? null : reader.GetString(25),
                                ApprovedBy = reader.IsDBNull(26) ? null : reader.GetString(26),
                                Comments = reader.IsDBNull(27) ? null : reader.GetString(27)

                            });
                        }
                    }
                }
            }

            totalRecords = records.Count;
            var paginatedRecords = records.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.TotalRecords = totalRecords;
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            return View("~/Views/ORB1/CodeI.cshtml", paginatedRecords);
        }

        // Make sure this method has the correct attribute and route
        [HttpGet]
        [Route("ORB1/CodeI/GetTanks")] // Add this if missing
        public JsonResult GetTanks()
        {
            List<object> tanks = new List<object>();

            try // Add error handling
            {
                using (SqlConnection connection = _db.CreateConnection())
                {
                    using (SqlCommand cmd = new SqlCommand("proc_GetORB1_FirstPage_OilResidueBilge", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        connection.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tanks.Add(new
                                {
                                    TankIdentification = reader["TankIdentification"].ToString(),
                                    TankLocation_Frames_From_To = reader["TankLocation_Frames_From_To"].ToString(),
                                    VolumeCapacity = Convert.ToDecimal(reader["Volume_m3"])
                                });
                            }
                        }
                    }
                }
                return Json(tanks);
            }
            catch (Exception ex)
            {
                // Log the exception and return an error message
                return Json(new { error = ex.Message });
            }
        }

        // Data Entry Page
        public IActionResult DataEntry_CodeI(string userId, string userName, string userRoleName)
        {
            ViewBag.UserID = userId;
            ViewBag.UserName = userName;
            ViewBag.UserRoleName = userRoleName;
            return View("~/Views/ORB1/DataEntry_CodeI.cshtml", new CodeIModel());
        }

        // Edit data entry page (fetch record by Id)
        public async Task<IActionResult> Edit(int id, string userId, string userName, string userRoleName)
        {
            // Store user details in ViewBag (so they can be used in the view)
            ViewBag.UserID = userId;
            ViewBag.UserName = userName;
            ViewBag.UserRoleName = userRoleName;

            CodeIViewModel recordToEdit = null;

            using (SqlConnection connection = _db.CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("proc_GetCodeIById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync()) // Ensure data exists before reading
                        {
                            recordToEdit = new CodeIViewModel
                            {
                                Id = reader.GetInt32(0),
                                UserId = reader.IsDBNull(1) ? null : reader.GetValue(1).ToString(),
                                EntryDate = reader.GetDateTime(2),
                                SelectType = reader.IsDBNull(3) ? null : reader.GetValue(3).ToString(),

                                WeeklyInventoryTanks = reader.IsDBNull(4) ? null : reader.GetValue(4).ToString(),
                                WeeklyInventoryCapacity = reader.IsDBNull(5) ? (decimal?)null : reader.GetDecimal(5),
                                WeeklyInventoryRetained = reader.IsDBNull(6) ? (decimal?)null : reader.GetDecimal(6),

                                DebunkeringQuantity = reader.IsDBNull(7) ? (decimal?)null : reader.GetDecimal(7),
                                DebunkeringGrade = reader.IsDBNull(8) ? null : reader.GetValue(8).ToString(),
                                DebunkeringSulphurContent = reader.IsDBNull(9) ? null : reader.GetValue(9).ToString(),
                                DebunkeringFrom = reader.IsDBNull(10) ? null : reader.GetValue(10).ToString(),
                                DebunkeringQuantityRetained = reader.IsDBNull(11) ? (decimal?)null : reader.GetDecimal(11),
                                DebunkeringTo = reader.IsDBNull(12) ? null : reader.GetValue(12).ToString(),
                                DebunkeringPortFacility = reader.IsDBNull(13) ? null : reader.GetValue(13).ToString(),
                                DebunkeringStartDateTime = reader.IsDBNull(14) ? (DateTime?)null : reader.GetDateTime(14),
                                DebunkeringStopDateTime = reader.IsDBNull(15) ? (DateTime?)null : reader.GetDateTime(15),

                                ValveName = reader.IsDBNull(16) ? null : reader.GetValue(16).ToString(),
                                ValveNo = reader.IsDBNull(17) ? null : reader.GetValue(17).ToString(),
                                ValveAssociatedEquipment = reader.IsDBNull(18) ? null : reader.GetValue(18).ToString(),
                                ValveSealNo = reader.IsDBNull(19) ? null : reader.GetValue(19).ToString(),

                                BreakingValveName = reader.IsDBNull(20) ? null : reader.GetValue(20).ToString(),
                                BreakingValveNo = reader.IsDBNull(21) ? null : reader.GetValue(21).ToString(),
                                BreakingAssociatedEquipment = reader.IsDBNull(22) ? null : reader.GetValue(22).ToString(),
                                BreakingReason = reader.IsDBNull(23) ? null : reader.GetValue(23).ToString(),
                                BreakingSealNo = reader.IsDBNull(24) ? null : reader.GetValue(24).ToString(),

                                StatusName = reader.IsDBNull(25) ? null : reader.GetValue(25).ToString(),
                                ApprovedBy = reader.IsDBNull(26) ? null : reader.GetValue(26).ToString(),
                                Comments = reader.IsDBNull(27) ? null : reader.GetValue(27).ToString()
                            };
                        }
                    }
                }
            }

            if (recordToEdit == null)
            {
                return NotFound(); // Return 404 if no record is found
            }

            return userRoleName switch
            {
                "Level 1- Entry" => View("~/Views/ORB1/DataEdit_CodeI.cshtml", recordToEdit),
                "Level 2- Approver" => View("~/Views/ORB1/Approver_DataEdit_CodeI.cshtml", recordToEdit),
                _ => Forbid() // Handle unexpected roles
            };
        }

        [Route("ORB1/CodeI/Create")]
        [HttpPost]
        public IActionResult Create([FromBody] CodeIModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data received.");
            }

            try
            {
                string storedProcedure = "proc_InsertORB1CodeI";
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@UserId", model.UserId),
                    new SqlParameter("@EntryDate", model.EntryDate),
                    new SqlParameter("@SelectType", model.SelectType ?? (object)DBNull.Value),

                    // Weekly Inventory
                    new SqlParameter("@WeeklyInventoryTanks", model.WeeklyInventoryTanks ?? (object)DBNull.Value),
                    new SqlParameter("@WeeklyInventoryCapacity", (object)model.WeeklyInventoryCapacity ?? DBNull.Value),
                    new SqlParameter("@WeeklyInventoryRetained", (object)model.WeeklyInventoryRetained ?? DBNull.Value),

                    // Debunkering
                    new SqlParameter("@DebunkeringQuantity", (object)model.DebunkeringQuantity ?? DBNull.Value),
                    new SqlParameter("@DebunkeringGrade", model.DebunkeringGrade ?? (object)DBNull.Value),
                    new SqlParameter("@DebunkeringSulphurContent", model.DebunkeringSulphurContent ?? (object)DBNull.Value),
                    new SqlParameter("@DebunkeringFrom", model.DebunkeringFrom ?? (object)DBNull.Value),
                    new SqlParameter("@DebunkeringQuantityRetained", (object)model.DebunkeringQuantityRetained ?? DBNull.Value),
                    new SqlParameter("@DebunkeringTo", model.DebunkeringTo ?? (object)DBNull.Value),
                    new SqlParameter("@DebunkeringPortFacility", model.DebunkeringPortFacility ?? (object)DBNull.Value),
                    new SqlParameter("@DebunkeringStartDateTime", (object)model.DebunkeringStartDateTime ?? DBNull.Value),
                    new SqlParameter("@DebunkeringStopDateTime", (object)model.DebunkeringStopDateTime ?? DBNull.Value),

                    // Sealing of Valve
                    new SqlParameter("@ValveName", model.ValveName ?? (object)DBNull.Value),
                    new SqlParameter("@ValveNo", model.ValveNo ?? (object)DBNull.Value),
                    new SqlParameter("@ValveAssociatedEquipment", model.ValveAssociatedEquipment ?? (object)DBNull.Value),
                    new SqlParameter("@ValveSealNo", model.ValveSealNo ?? (object)DBNull.Value),

                    // Breaking of Seal
                    new SqlParameter("@BreakingValveName", model.BreakingValveName ?? (object)DBNull.Value),
                    new SqlParameter("@BreakingValveNo", model.BreakingValveNo ?? (object)DBNull.Value),
                    new SqlParameter("@BreakingAssociatedEquipment", model.BreakingAssociatedEquipment ?? (object)DBNull.Value),
                    new SqlParameter("@BreakingReason", model.BreakingReason ?? (object)DBNull.Value),
                    new SqlParameter("@BreakingSealNo", model.BreakingSealNo ?? (object)DBNull.Value)
                };

                int InsertedId = _db.ExecuteInsertStoredProcedure(storedProcedure, parameters);

                if (InsertedId > 0)
                {
                    return Json(new { success = true, message = "Data Inserted Successfully!" });
                }
                else
                {
                    return Json(new { success = true, message = "Data Insertion Failed!" });
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("ORB1/CodeI/Update")]
        public IActionResult Update([FromBody] CodeIViewModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                string storedProcedure = "proc_UpdateORB1CodeI";
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@Id", model.Id),
                    new SqlParameter("@UserId", model.UserId),
                    new SqlParameter("@EntryDate", model.EntryDate),
                    new SqlParameter("@SelectType", model.SelectType ?? (object)DBNull.Value),

                    // Weekly Inventory
                    new SqlParameter("@WeeklyInventoryTanks", model.WeeklyInventoryTanks ?? (object)DBNull.Value),
                    new SqlParameter("@WeeklyInventoryCapacity", (object)model.WeeklyInventoryCapacity ?? DBNull.Value),
                    new SqlParameter("@WeeklyInventoryRetained", (object)model.WeeklyInventoryRetained ?? DBNull.Value),

                    // Debunkering
                    new SqlParameter("@DebunkeringQuantity", (object)model.DebunkeringQuantity ?? DBNull.Value),
                    new SqlParameter("@DebunkeringGrade", model.DebunkeringGrade ?? (object)DBNull.Value),
                    new SqlParameter("@DebunkeringSulphurContent", model.DebunkeringSulphurContent ?? (object)DBNull.Value),
                    new SqlParameter("@DebunkeringFrom", model.DebunkeringFrom ?? (object)DBNull.Value),
                    new SqlParameter("@DebunkeringQuantityRetained", (object)model.DebunkeringQuantityRetained ?? DBNull.Value),
                    new SqlParameter("@DebunkeringTo", model.DebunkeringTo ?? (object)DBNull.Value),
                    new SqlParameter("@DebunkeringPortFacility", model.DebunkeringPortFacility ?? (object)DBNull.Value),
                    new SqlParameter("@DebunkeringStartDateTime", (object)model.DebunkeringStartDateTime ?? DBNull.Value),
                    new SqlParameter("@DebunkeringStopDateTime", (object)model.DebunkeringStopDateTime ?? DBNull.Value),

                    // Sealing of Valve
                    new SqlParameter("@ValveName", model.ValveName ?? (object)DBNull.Value),
                    new SqlParameter("@ValveNo", model.ValveNo ?? (object)DBNull.Value),
                    new SqlParameter("@ValveAssociatedEquipment", model.ValveAssociatedEquipment ?? (object)DBNull.Value),
                    new SqlParameter("@ValveSealNo", model.ValveSealNo ?? (object)DBNull.Value),

                    // Breaking of Seal
                    new SqlParameter("@BreakingValveName", model.BreakingValveName ?? (object)DBNull.Value),
                    new SqlParameter("@BreakingValveNo", model.BreakingValveNo ?? (object)DBNull.Value),
                    new SqlParameter("@BreakingAssociatedEquipment", model.BreakingAssociatedEquipment ?? (object)DBNull.Value),
                    new SqlParameter("@BreakingReason", model.BreakingReason ?? (object)DBNull.Value),
                    new SqlParameter("@BreakingSealNo", model.BreakingSealNo ?? (object)DBNull.Value)
                };

                int result = _db.ExecuteUpdateStoredProcedure(storedProcedure, parameters);

                if (result > 0)
                {
                    return Json(new { success = true, message = "Record Updated Successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Record Update Failed!" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("ORB1/CodeI/ApproverUpdate")]
        public IActionResult ApproverUpdate([FromBody] CodeIViewModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                string storedProcedure = "proc_ApproverUpdateORB1CodeI";
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@Id", model.Id),
                    new SqlParameter("@UserId", model.UserId),
                    new SqlParameter("@EntryDate", model.EntryDate),
                    new SqlParameter("@SelectType", model.SelectType ?? (object)DBNull.Value),

                    // Weekly Inventory
                    new SqlParameter("@WeeklyInventoryTanks", model.WeeklyInventoryTanks ?? (object)DBNull.Value),
                    new SqlParameter("@WeeklyInventoryCapacity", (object)model.WeeklyInventoryCapacity ?? DBNull.Value),
                    new SqlParameter("@WeeklyInventoryRetained", (object)model.WeeklyInventoryRetained ?? DBNull.Value),

                    // Debunkering
                    new SqlParameter("@DebunkeringQuantity", (object)model.DebunkeringQuantity ?? DBNull.Value),
                    new SqlParameter("@DebunkeringGrade", model.DebunkeringGrade ?? (object)DBNull.Value),
                    new SqlParameter("@DebunkeringSulphurContent", model.DebunkeringSulphurContent ?? (object)DBNull.Value),
                    new SqlParameter("@DebunkeringFrom", model.DebunkeringFrom ?? (object)DBNull.Value),
                    new SqlParameter("@DebunkeringQuantityRetained", (object)model.DebunkeringQuantityRetained ?? DBNull.Value),
                    new SqlParameter("@DebunkeringTo", model.DebunkeringTo ?? (object)DBNull.Value),
                    new SqlParameter("@DebunkeringPortFacility", model.DebunkeringPortFacility ?? (object)DBNull.Value),
                    new SqlParameter("@DebunkeringStartDateTime", (object)model.DebunkeringStartDateTime ?? DBNull.Value),
                    new SqlParameter("@DebunkeringStopDateTime", (object)model.DebunkeringStopDateTime ?? DBNull.Value),

                    // Sealing of Valve
                    new SqlParameter("@ValveName", model.ValveName ?? (object)DBNull.Value),
                    new SqlParameter("@ValveNo", model.ValveNo ?? (object)DBNull.Value),
                    new SqlParameter("@ValveAssociatedEquipment", model.ValveAssociatedEquipment ?? (object)DBNull.Value),
                    new SqlParameter("@ValveSealNo", model.ValveSealNo ?? (object)DBNull.Value),

                    // Breaking of Seal
                    new SqlParameter("@BreakingValveName", model.BreakingValveName ?? (object)DBNull.Value),
                    new SqlParameter("@BreakingValveNo", model.BreakingValveNo ?? (object)DBNull.Value),
                    new SqlParameter("@BreakingAssociatedEquipment", model.BreakingAssociatedEquipment ?? (object)DBNull.Value),
                    new SqlParameter("@BreakingReason", model.BreakingReason ?? (object)DBNull.Value),
                    new SqlParameter("@BreakingSealNo", model.BreakingSealNo ?? (object)DBNull.Value),

                    new SqlParameter("@StatusName", (object)model.StatusName ?? DBNull.Value),
                    new SqlParameter("@Comments", (object)model.Comments ?? DBNull.Value)
                };

                int result = _db.ExecuteUpdateStoredProcedure(storedProcedure, parameters);

                if (result > 0)
                {
                    return Json(new { success = true, message = "Record Updated Successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Record Update Failed!" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}