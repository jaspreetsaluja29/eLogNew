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
    public class CodeHController : Controller
    {
        private readonly DatabaseHelper _db;

        public CodeHController(DatabaseHelper db)
        {
            _db = db;
        }

        // Fetch data using stored procedure        
        [Route("ORB1/CodeH/GetCodeHData")]
        [HttpGet]
        public async Task<IActionResult> GetCodeHData(int pageNumber = 1, int pageSize = 10)
        {
            List<CodeHViewModel> records = new List<CodeHViewModel>();
            int totalRecords = 0;

            using (SqlConnection connection = _db.CreateConnection()) // Ensure this method exists in DatabaseHelper
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("proc_GetORB1_CodeH", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            records.Add(new CodeHViewModel
                            {
                                Id = reader.GetInt32(0),
                                UserId = reader.IsDBNull(1) ? null : reader.GetString(1),
                                EntryDate = reader.GetDateTime(2),
                                Port = reader.IsDBNull(3) ? null : reader.GetString(3),
                                Location = reader.IsDBNull(4) ? null : reader.GetString(4),
                                StartDateTime = reader.GetDateTime(5),
                                StopDateTime = reader.GetDateTime(6),
                                Quantity = reader.GetDecimal(7),
                                Grade = reader.IsDBNull(8) ? null : reader.GetString(8),
                                SulphurContent = reader.IsDBNull(9) ? null : reader.GetString(9),
                                TankLoaded1 = reader.IsDBNull(10) ? null : reader.GetString(10),
                                TankRetained1 = reader.IsDBNull(11) ? null : reader.GetString(11),
                                TankLoaded2 = reader.IsDBNull(12) ? null : reader.GetString(12),
                                TankRetained2 = reader.IsDBNull(13) ? null : reader.GetString(13),
                                TankLoaded3 = reader.IsDBNull(14) ? null : reader.GetString(14),
                                TankRetained3 = reader.IsDBNull(15) ? null : reader.GetString(15),
                                TankLoaded4 = reader.IsDBNull(16) ? null : reader.GetString(16),
                                TankRetained4 = reader.IsDBNull(17) ? null : reader.GetString(17),
                                StatusName = reader.IsDBNull(18) ? null : reader.GetString(18),
                                ApprovedBy = reader.IsDBNull(19) ? null : reader.GetString(19),
                                Comments = reader.IsDBNull(20) ? null : reader.GetString(20)
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

            return View("~/Views/ORB1/CodeH.cshtml", paginatedRecords);
        }

        // Data Entry Page
        public IActionResult DataEntry_CodeH(string userId, string userName, string userRoleName)
        {
            ViewBag.UserID = userId;
            ViewBag.UserName = userName;
            ViewBag.UserRoleName = userRoleName;
            return View("~/Views/ORB1/DataEntry_CodeH.cshtml", new CodeHModel());
        }


        // Edit data entry page (fetch record by Id)
        public async Task<IActionResult> Edit(int id, string userId, string userName, string userRoleName)
        {
            // Store user details in ViewBag (so they can be used in the view)
            ViewBag.UserID = userId;
            ViewBag.UserName = userName;
            ViewBag.UserRoleName = userRoleName;
            CodeHViewModel recordToEdit = null;

            using (SqlConnection connection = _db.CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("proc_GetCodeHById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync()) // Ensure data exists before reading
                        {
                            recordToEdit = new CodeHViewModel

                            {
                                Id = reader.GetInt32(0),
                                UserId = reader.IsDBNull(1) ? null : reader.GetValue(1).ToString(),
                                EntryDate = reader.GetDateTime(2),
                                Port = reader.IsDBNull(3) ? null : reader.GetString(3).ToString(),
                                Location = reader.IsDBNull(4) ? null : reader.GetString(4).ToString(),
                                StartDateTime = reader.GetDateTime(5),
                                StopDateTime = reader.GetDateTime(6),
                                Quantity = reader.GetDecimal(7),
                                Grade = reader.IsDBNull(8) ? null : reader.GetString(8).ToString(),
                                SulphurContent = reader.IsDBNull(9) ? null : reader.GetString(9).ToString(),
                                TankLoaded1 = reader.IsDBNull(10) ? null : reader.GetString(10).ToString(),
                                TankRetained1 = reader.IsDBNull(11) ? null : reader.GetString(11).ToString(),
                                TankLoaded2 = reader.IsDBNull(12) ? null : reader.GetString(12).ToString(),
                                TankRetained2 = reader.IsDBNull(13) ? null : reader.GetString(13).ToString(),
                                TankLoaded3 = reader.IsDBNull(14) ? null : reader.GetString(14).ToString(),
                                TankRetained3 = reader.IsDBNull(15) ? null : reader.GetString(15).ToString(),
                                TankLoaded4 = reader.IsDBNull(16) ? null : reader.GetString(16).ToString(),
                                TankRetained4 = reader.IsDBNull(17) ? null : reader.GetString(17).ToString(),
                                StatusName = reader.IsDBNull(18) ? null : reader.GetValue(18).ToString(),
                                ApprovedBy = reader.IsDBNull(19) ? null : reader.GetValue(19).ToString(),
                                Comments = reader.IsDBNull(20) ? null : reader.GetValue(20).ToString()
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
                "Level 1- Entry" => View("~/Views/ORB1/DataEdit_CodeH.cshtml", recordToEdit),
                "Level 2- Approver" => View("~/Views/ORB1/Approver_DataEdit_CodeH.cshtml", recordToEdit),
                "SuperAdmin" => View("~/Views/ORB1/Approver_DataEdit_CodeH.cshtml", recordToEdit),
                _ => Forbid() // Handle unexpected roles
            };
        }

        [Route("ORB1/CodeH/Create")]
        [HttpPost]
        public IActionResult Create([FromBody] CodeHModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data received.");
            }

            try
            {
                string storedProcedure = "proc_InsertORB1CodeH";
                var parameters = new SqlParameter[]
                {
            new SqlParameter("@UserId", model.UserId),
            new SqlParameter("@EntryDate", model.EntryDate),
            new SqlParameter("@Port", (object)model.Port ?? DBNull.Value),
            new SqlParameter("@Location", (object)model.Location ?? DBNull.Value),
            new SqlParameter("@StartDateTime", model.StartDateTime),
            new SqlParameter("@StopDateTime", model.StopDateTime),
            new SqlParameter("@Quantity", model.Quantity),
            new SqlParameter("@Grade", (object)model.Grade ?? DBNull.Value),
            new SqlParameter("@SulphurContent", (object)model.SulphurContent ?? DBNull.Value),
            new SqlParameter("@TankLoaded1", (object)model.TankLoaded1 ?? DBNull.Value),
            new SqlParameter("@TankRetained1", (object)model.TankRetained1 ?? DBNull.Value),
            new SqlParameter("@TankLoaded2", (object)model.TankLoaded2 ?? DBNull.Value),
            new SqlParameter("@TankRetained2", (object)model.TankRetained2 ?? DBNull.Value),
            new SqlParameter("@TankLoaded3", (object)model.TankLoaded3 ?? DBNull.Value),
            new SqlParameter("@TankRetained3", (object)model.TankRetained3 ?? DBNull.Value),
            new SqlParameter("@TankLoaded4", (object)model.TankLoaded4 ?? DBNull.Value),
            new SqlParameter("@TankRetained4", (object)model.TankRetained4 ?? DBNull.Value)
                };

                int insertedId = _db.ExecuteInsertStoredProcedure(storedProcedure, parameters);

                if (insertedId > 0)
                {
                    return Json(new { success = true, message = "Data Inserted Successfully!" });
                }
                else
                {
                    return Json(new { success = false, message = "Data Insertion Failed!" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPost]
        [Route("ORB1/CodeH/Update")]
        public IActionResult Update([FromBody] CodeHViewModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                string storedProcedure = "proc_UpdateORB1CodeH";
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@Id", model.Id),
                    new SqlParameter("@UserId", model.UserId),
                    new SqlParameter("@EntryDate", model.EntryDate),
                    new SqlParameter("@Port", (object)model.Port ?? DBNull.Value),
                    new SqlParameter("@Location", (object)model.Location ?? DBNull.Value),
                    new SqlParameter("@StartDateTime", model.StartDateTime),
                    new SqlParameter("@StopDateTime", model.StopDateTime),
                    new SqlParameter("@Quantity", (object)model.Quantity ?? DBNull.Value),
                    new SqlParameter("@Grade", (object)model.Grade ?? DBNull.Value),
                    new SqlParameter("@SulphurContent", (object)model.SulphurContent ?? DBNull.Value),
                    new SqlParameter("@TankLoaded1", (object)model.TankLoaded1 ?? DBNull.Value),
                    new SqlParameter("@TankRetained1", (object)model.TankRetained1 ?? DBNull.Value),
                    new SqlParameter("@TankLoaded2", (object)model.TankLoaded2 ?? DBNull.Value),
                    new SqlParameter("@TankRetained2", (object)model.TankRetained2 ?? DBNull.Value),
                    new SqlParameter("@TankLoaded3", (object)model.TankLoaded3 ?? DBNull.Value),
                    new SqlParameter("@TankRetained3", (object)model.TankRetained3 ?? DBNull.Value),
                    new SqlParameter("@TankLoaded4", (object)model.TankLoaded4 ?? DBNull.Value),
                    new SqlParameter("@TankRetained4", (object)model.TankRetained4 ?? DBNull.Value)
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
        [Route("ORB1/CodeH/ApproverUpdate")]
        public IActionResult ApproverUpdate([FromBody] CodeHViewModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                string storedProcedure = "proc_ApproverUpdateORB1CodeH";
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@Id", model.Id),
                    new SqlParameter("@UserId", model.UserId),
                    new SqlParameter("@EntryDate", model.EntryDate),
                    new SqlParameter("@Port", (object)model.Port ?? DBNull.Value),
                    new SqlParameter("@Location", (object)model.Location ?? DBNull.Value),
                    new SqlParameter("@StartDateTime", model.StartDateTime),
                    new SqlParameter("@StopDateTime", model.StopDateTime),
                    new SqlParameter("@Quantity", model.Quantity),
                    new SqlParameter("@Grade", (object)model.Grade ?? DBNull.Value),
                    new SqlParameter("@SulphurContent", (object)model.SulphurContent ?? DBNull.Value),
                    new SqlParameter("@TankLoaded1", (object)model.TankLoaded1 ?? DBNull.Value),
                    new SqlParameter("@TankRetained1", (object)model.TankRetained1 ?? DBNull.Value),
                    new SqlParameter("@TankLoaded2", (object)model.TankLoaded2 ?? DBNull.Value),
                    new SqlParameter("@TankRetained2", (object)model.TankRetained2 ?? DBNull.Value),
                    new SqlParameter("@TankLoaded3", (object)model.TankLoaded3 ?? DBNull.Value),
                    new SqlParameter("@TankRetained3", (object)model.TankRetained3 ?? DBNull.Value),
                    new SqlParameter("@TankLoaded4", (object)model.TankLoaded4 ?? DBNull.Value),
                    new SqlParameter("@TankRetained4", (object)model.TankRetained4 ?? DBNull.Value),
                    new SqlParameter("@StatusName", (object)model.StatusName ?? DBNull.Value),
                    new SqlParameter("@Comments", (object)model.Comments ?? DBNull.Value),
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

        // Make sure this method has the correct attribute and route
        [HttpGet]
        [Route("ORB1/CodeH/GetTanks")] // Add this if missing
        public JsonResult GetTanks()
        {
            List<object> tanks = new List<object>();

            try // Add error handling
            {
                using (SqlConnection connection = _db.CreateConnection())
                {
                    using (SqlCommand cmd = new SqlCommand("proc_GetORB1_FirstPageOilyBilgeRetention", connection))
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
    }
}