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

                                DebunkeringQuantity = reader.IsDBNull(4) ? null : reader.GetDecimal(4),
                                DebunkeringGrade = reader.IsDBNull(5) ? null : reader.GetString(5),
                                DebunkeringSulphurContent = reader.IsDBNull(6) ? null : reader.GetString(6),
                                DebunkeringFrom = reader.IsDBNull(7) ? null : reader.GetString(7),
                                DebunkeringQuantityRetained = reader.IsDBNull(8) ? null : reader.GetDecimal(8),
                                DebunkeringTo = reader.IsDBNull(9) ? null : reader.GetString(9),
                                DebunkeringPortFacility = reader.IsDBNull(10) ? null : reader.GetString(10),
                                DebunkeringStartDateTime = reader.IsDBNull(11) ? (DateTime?)null : reader.GetDateTime(11),
                                DebunkeringStopDateTime = reader.IsDBNull(12) ? (DateTime?)null : reader.GetDateTime(12),

                                ValveName = reader.IsDBNull(13) ? null : reader.GetString(13),
                                ValveNo = reader.IsDBNull(14) ? null : reader.GetString(14),
                                ValveAssociatedEquipment = reader.IsDBNull(15) ? null : reader.GetString(15),
                                ValveSealNo = reader.IsDBNull(16) ? null : reader.GetString(16),

                                BreakingValveName = reader.IsDBNull(17) ? null : reader.GetString(17),
                                BreakingValveNo = reader.IsDBNull(18) ? null : reader.GetString(18),
                                BreakingAssociatedEquipment = reader.IsDBNull(19) ? null : reader.GetString(19),
                                BreakingReason = reader.IsDBNull(20) ? null : reader.GetString(20),
                                BreakingSealNo = reader.IsDBNull(21) ? null : reader.GetString(21),

                                StatusName = reader.IsDBNull(22) ? null : reader.GetString(22),
                                ApprovedBy = reader.IsDBNull(23) ? null : reader.GetString(23),
                                Comments = reader.IsDBNull(24) ? null : reader.GetString(24)
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
                                DebunkeringQuantity = reader.IsDBNull(4) ? (decimal?)null : reader.GetDecimal(4),
                                DebunkeringGrade = reader.IsDBNull(5) ? null : reader.GetValue(5).ToString(),
                                DebunkeringSulphurContent = reader.IsDBNull(6) ? null : reader.GetValue(6).ToString(),
                                DebunkeringFrom = reader.IsDBNull(7) ? null : reader.GetValue(7).ToString(),
                                DebunkeringQuantityRetained = reader.IsDBNull(8) ? (decimal?)null : reader.GetDecimal(8),
                                DebunkeringTo = reader.IsDBNull(9) ? null : reader.GetValue(9).ToString(),
                                DebunkeringPortFacility = reader.IsDBNull(10) ? null : reader.GetValue(10).ToString(),
                                DebunkeringStartDateTime = reader.IsDBNull(11) ? (DateTime?)null : reader.GetDateTime(11),
                                DebunkeringStopDateTime = reader.IsDBNull(12) ? (DateTime?)null : reader.GetDateTime(12),
                                ValveName = reader.IsDBNull(13) ? null : reader.GetValue(13).ToString(),
                                ValveNo = reader.IsDBNull(14) ? null : reader.GetValue(14).ToString(),
                                ValveAssociatedEquipment = reader.IsDBNull(15) ? null : reader.GetValue(15).ToString(),
                                ValveSealNo = reader.IsDBNull(16) ? null : reader.GetValue(16).ToString(),
                                BreakingValveName = reader.IsDBNull(17) ? null : reader.GetValue(18).ToString(),
                                BreakingValveNo = reader.IsDBNull(18) ? null : reader.GetValue(18).ToString(),
                                BreakingAssociatedEquipment = reader.IsDBNull(19) ? null : reader.GetValue(19).ToString(),
                                BreakingReason = reader.IsDBNull(20) ? null : reader.GetValue(20).ToString(),
                                BreakingSealNo = reader.IsDBNull(21) ? null : reader.GetValue(21).ToString(),
                                StatusName = reader.IsDBNull(22) ? null : reader.GetValue(22).ToString(),
                                ApprovedBy = reader.IsDBNull(23) ? null : reader.GetValue(23).ToString(),
                                Comments = reader.IsDBNull(24) ? null : reader.GetValue(24).ToString()
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