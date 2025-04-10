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
    public class CodeEController : Controller
    {
        private readonly DatabaseHelper _db;

        public CodeEController(DatabaseHelper db)
        {
            _db = db;
        }

        // Fetch data using stored procedure        
        [Route("ORB1/CodeE/GetCodeEData")]
        [HttpGet]
        public async Task<IActionResult> GetCodeEData(int pageNumber = 1, int pageSize = 10)
        {
            List<CodeEViewModel> records = new List<CodeEViewModel>();
            int totalRecords = 0;

            using (SqlConnection connection = _db.CreateConnection()) // Ensure this method exists in DatabaseHelper
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("proc_GetORB1_CodeE", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            records.Add(new CodeEViewModel
                            {
                                Id = reader.GetInt32(0),
                                UserId = reader.IsDBNull(1) ? null : reader.GetString(1),
                                EntryDate = reader.GetDateTime(2),
                                AutomaticDischargeType = reader.IsDBNull(3) ? null : reader.GetString(3),
                                OverboardPositionShipStart = reader.IsDBNull(4) ? null : reader.GetString(4),
                                OverboardTimeSwitching = reader.IsDBNull(5) ? (TimeSpan?)null : reader.GetTimeSpan(5),
                                TransferTimeSwitching = reader.IsDBNull(6) ? (TimeSpan?)null : reader.GetTimeSpan(6),
                                TransferTankfrom = reader.IsDBNull(7) ? null : reader.GetString(7),
                                TransferTankTo = reader.IsDBNull(8) ? null : reader.GetString(8),
                                TimeBackToManual = reader.IsDBNull(9) ? (TimeSpan?)null : reader.GetTimeSpan(9),
                                StatusName = reader.IsDBNull(10) ? null : reader.GetString(10),
                                ApprovedBy = reader.IsDBNull(11) ? null : reader.GetString(11),
                                Comments = reader.IsDBNull(12) ? null : reader.GetString(12)
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

            return View("~/Views/ORB1/CodeE.cshtml", paginatedRecords);
        }


        // Data Entry Page
        public IActionResult DataEntry_CodeE(string userId, string userName, string userRoleName)
        {
            ViewBag.UserID = userId;
            ViewBag.UserName = userName;
            ViewBag.UserRoleName = userRoleName;
            return View("~/Views/ORB1/DataEntry_CodeE.cshtml", new CodeEModel());
        }

        public async Task<IActionResult> Edit(int id, string userId, string userName, string userRoleName)
        {
            // Store user details in ViewBag (so they can be used in the view)
            ViewBag.UserID = userId;
            ViewBag.UserName = userName;
            ViewBag.UserRoleName = userRoleName;
            CodeEViewModel recordToEdit = null;
            using (SqlConnection connection = _db.CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("proc_GetCodeEById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync()) // Ensure data exists before reading
                        {
                            recordToEdit = new CodeEViewModel
                            {
                                Id = reader.GetInt32(0),
                                UserId = reader.IsDBNull(1) ? null : reader.GetValue(1).ToString(),
                                EntryDate = reader.GetDateTime(2),
                                AutomaticDischargeType = reader.IsDBNull(3) ? null : reader.GetValue(3).ToString(),
                                OverboardPositionShipStart = reader.IsDBNull(4) ? null : reader.GetValue(4).ToString(),
                                OverboardTimeSwitching = reader.IsDBNull(5) ? (TimeSpan?)null : reader.GetTimeSpan(5),
                                TransferTimeSwitching = reader.IsDBNull(6) ? (TimeSpan?)null : reader.GetTimeSpan(6),
                                TransferTankfrom = reader.IsDBNull(7) ? null : reader.GetValue(7).ToString(),
                                TransferTankTo = reader.IsDBNull(8) ? null : reader.GetValue(8).ToString(),
                                TimeBackToManual = reader.IsDBNull(9) ? (TimeSpan?)null : reader.GetTimeSpan(9),
                                StatusName = reader.IsDBNull(10) ? null : reader.GetValue(10).ToString(),
                                ApprovedBy = reader.IsDBNull(11) ? null : reader.GetValue(11).ToString(),
                                Comments = reader.IsDBNull(12) ? null : reader.GetValue(12).ToString()
                            };
                        }
                    }
                }
            }
            if (recordToEdit == null)
            {
                return NotFound(); // Return 404 if no record is found
            }

            // Make sure you're using views that expect CodeEViewModel, not CodeFViewModel
            return userRoleName switch
            {
                "Level 1- Entry" => View("~/Views/ORB1/DataEdit_CodeE.cshtml", recordToEdit),
                "Level 2- Approver" => View("~/Views/ORB1/Approver_DataEdit_CodeE.cshtml", recordToEdit),
                _ => Forbid() // Handle unexpected roles
            };
        }


        [Route("ORB1/CodeE/Create")]
        [HttpPost]
        public IActionResult Create([FromBody] CodeEModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data received.");
            }

            try
            {
                string storedProcedure = "proc_InsertORB1CodeE";
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@UserId", model.UserId),
                    new SqlParameter("@EntryDate", model.EntryDate),
                    new SqlParameter("@AutomaticDischargeType", model.AutomaticDischargeType ?? (object)DBNull.Value),
                    new SqlParameter("@OverboardPositionShipStart", model.OverboardPositionShipStart ?? (object)DBNull.Value),
                    new SqlParameter("@OverboardTimeSwitching", (object)model.OverboardTimeSwitching ?? DBNull.Value),
                    new SqlParameter("@TransferTimeSwitching", (object)model.TransferTimeSwitching ?? DBNull.Value),
                    new SqlParameter("@TransferTankfrom", model.TransferTankfrom ?? (object)DBNull.Value),
                    new SqlParameter("@TransferTankTo", model.TransferTankTo ?? (object)DBNull.Value),
                    new SqlParameter("@TimeBackToManual", (object)model.TimeBackToManual ?? DBNull.Value)
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
        [Route("ORB1/CodeE/Update")]
        public IActionResult Update([FromBody] CodeEViewModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                string storedProcedure = "proc_UpdateORB1CodeE";
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@Id", model.Id),
                    new SqlParameter("@UserId", model.UserId),
                    new SqlParameter("@EntryDate", model.EntryDate),
                    new SqlParameter("@AutomaticDischargeType", model.AutomaticDischargeType ?? (object)DBNull.Value),
                    new SqlParameter("@OverboardPositionShipStart", model.OverboardPositionShipStart ?? (object)DBNull.Value),
                    new SqlParameter("@OverboardTimeSwitching", (object)model.OverboardTimeSwitching ?? DBNull.Value),
                    new SqlParameter("@TransferTimeSwitching", (object)model.TransferTimeSwitching ?? DBNull.Value),
                    new SqlParameter("@TransferTankfrom", model.TransferTankfrom ?? (object)DBNull.Value),
                    new SqlParameter("@TransferTankTo", model.TransferTankTo ?? (object)DBNull.Value),
                    new SqlParameter("@TimeBackToManual", (object)model.TimeBackToManual ?? DBNull.Value)
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
        [Route("ORB1/CodeE/ApproverUpdate")]
        public IActionResult ApproverUpdate([FromBody] CodeEViewModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                string storedProcedure = "proc_ApproverUpdateORB1CodeE";
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@Id", model.Id),
                    new SqlParameter("@UserId", model.UserId),
                    new SqlParameter("@EntryDate", model.EntryDate),
                    new SqlParameter("@AutomaticDischargeType", model.AutomaticDischargeType ?? (object)DBNull.Value),
                    new SqlParameter("@OverboardPositionShipStart", model.OverboardPositionShipStart ?? (object)DBNull.Value),
                    new SqlParameter("@OverboardTimeSwitching", (object)model.OverboardTimeSwitching ?? DBNull.Value),
                    new SqlParameter("@TransferTimeSwitching", (object)model.TransferTimeSwitching ?? DBNull.Value),
                    new SqlParameter("@TransferTankfrom", model.TransferTankfrom ?? (object)DBNull.Value),
                    new SqlParameter("@TransferTankTo", model.TransferTankTo ?? (object)DBNull.Value),
                    new SqlParameter("@TimeBackToManual", (object)model.TimeBackToManual ?? DBNull.Value),
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