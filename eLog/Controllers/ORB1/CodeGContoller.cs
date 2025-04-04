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
    public class CodeGController : Controller
    {
        private readonly DatabaseHelper _db;

        public CodeGController(DatabaseHelper db)
        {
            _db = db;
        }

        // Fetch data using stored procedure        
        [Route("ORB1/CodeG/GetCodeGData")]
        [HttpGet]
        public async Task<IActionResult> GetCodeGData(int pageNumber = 1, int pageSize = 10)
        {
            List<CodeGViewModel> records = new List<CodeGViewModel>();
            int totalRecords = 0;

            using (SqlConnection connection = _db.CreateConnection()) // Ensure this method exists in DatabaseHelper
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("proc_GetORB1_CodeG", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            records.Add(new CodeGViewModel
                            {
                                Id = reader.GetInt32(0),
                                UserId = reader.IsDBNull(1) ? null : reader.GetString(1),
                                EntryDate = reader.GetDateTime(2),
                                TimeofOccurrence = reader.IsDBNull(3) ? (TimeSpan?)null : reader.GetTimeSpan(3),
                                PositionofShip = reader.IsDBNull(4) ? null : reader.GetString(4),
                                ApproxQuantity = reader.IsDBNull(5) ? (decimal?)null : reader.GetDecimal(5),
                                TypeofOil = reader.IsDBNull(6) ? null : reader.GetString(6),
                                Reasons = reader.IsDBNull(7) ? null : reader.GetString(7),
                                StatusName = reader.IsDBNull(8) ? null : reader.GetString(8),
                                ApprovedBy = reader.IsDBNull(9) ? null : reader.GetString(9),
                                Comments = reader.IsDBNull(10) ? null : reader.GetString(10)
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

            return View("~/Views/ORB1/CodeG.cshtml", paginatedRecords);
        }


        // Data Entry Page
        public IActionResult DataEntry_CodeG(string userId, string userName, string userRoleName)
        {
            ViewBag.UserID = userId;
            ViewBag.UserName = userName;
            ViewBag.UserRoleName = userRoleName;
            return View("~/Views/ORB1/DataEntry_CodeG.cshtml", new CodeGModel());
        }

        // Edit data entry page (fetch record by Id)
        public async Task<IActionResult> Edit(int id, string userId, string userName, string userRoleName)
        {
            // Store user details in ViewBag (so they can be used in the view)
            ViewBag.UserID = userId;
            ViewBag.UserName = userName;
            ViewBag.UserRoleName = userRoleName;

            CodeGViewModel recordToEdit = null;

            using (SqlConnection connection = _db.CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("proc_GetCodeGById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync()) // Ensure data exists before reading
                        {
                            recordToEdit = new CodeGViewModel
                            {
                                Id = reader.GetInt32(0),
                                UserId = reader.IsDBNull(1) ? null : reader.GetValue(1).ToString(),
                                EntryDate = reader.GetDateTime(2),
                                TimeofOccurrence = reader.IsDBNull(3) ? (TimeSpan?)null : reader.GetTimeSpan(3),
                                PositionofShip = reader.IsDBNull(4) ? null : reader.GetString(4),
                                ApproxQuantity = reader.IsDBNull(5) ? (decimal?)null : reader.GetDecimal(5),
                                TypeofOil = reader.IsDBNull(6) ? null : reader.GetString(6),
                                Reasons = reader.IsDBNull(7) ? null : reader.GetString(7),
                                StatusName = reader.IsDBNull(8) ? null : reader.GetValue(8).ToString(),
                                ApprovedBy = reader.IsDBNull(9) ? null : reader.GetValue(9).ToString(),
                                Comments = reader.IsDBNull(10) ? null : reader.GetValue(10).ToString()
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
                "Level 1- Entry" => View("~/Views/ORB1/DataEdit_CodeG.cshtml", recordToEdit),
                "Level 2- Approver" => View("~/Views/ORB1/Approver_DataEdit_CodeG.cshtml", recordToEdit),
                _ => Forbid() // Handle unexpected roles
            };
        }

        [Route("ORB1/CodeG/Create")]
        [HttpPost]
        public IActionResult Create([FromBody] CodeGModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data received.");
            }

            try
            {
                string storedProcedure = "proc_InsertORB1CodeG";
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@UserId", model.UserId),
                    new SqlParameter("@EntryDate", model.EntryDate),
                    new SqlParameter("@TimeofOccurrence", (object)model.TimeofOccurrence ?? DBNull.Value),
                    new SqlParameter("@PositionofShip", (object)model.PositionofShip ?? DBNull.Value),
                    new SqlParameter("@ApproxQuantity", model.ApproxQuantity ?? (object)DBNull.Value),
                    new SqlParameter("@TypeofOil", (object)model.TypeofOil ?? DBNull.Value),
                    new SqlParameter("@Reasons", (object)model.Reasons ?? DBNull.Value)
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
        [Route("ORB1/CodeG/Update")]
        public IActionResult Update([FromBody] CodeGViewModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                string storedProcedure = "proc_UpdateORB1CodeG";
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@Id", model.Id),
                    new SqlParameter("@UserId", model.UserId),
                    new SqlParameter("@EntryDate", model.EntryDate),
                    new SqlParameter("@TimeofOccurrence", (object)model.TimeofOccurrence ?? DBNull.Value),
                    new SqlParameter("@PositionofShip", (object)model.PositionofShip ?? DBNull.Value),
                    new SqlParameter("@ApproxQuantity", model.ApproxQuantity ?? (object)DBNull.Value),
                    new SqlParameter("@TypeofOil", (object)model.TypeofOil ?? DBNull.Value),
                    new SqlParameter("@Reasons", (object)model.Reasons ?? DBNull.Value)
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
        [Route("ORB1/CodeG/ApproverUpdate")]
        public IActionResult ApproverUpdate([FromBody] CodeGViewModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                string storedProcedure = "proc_ApproverUpdateORB1CodeG";
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@Id", model.Id),
                    new SqlParameter("@UserId", model.UserId),
                    new SqlParameter("@EntryDate", model.EntryDate),
                    new SqlParameter("@TimeofOccurrence", (object)model.TimeofOccurrence ?? DBNull.Value),
                    new SqlParameter("@PositionofShip", (object)model.PositionofShip ?? DBNull.Value),
                    new SqlParameter("@ApproxQuantity", model.ApproxQuantity ?? (object)DBNull.Value),
                    new SqlParameter("@TypeofOil", (object)model.TypeofOil ?? DBNull.Value),
                    new SqlParameter("@Reasons", (object)model.Reasons ?? DBNull.Value),
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