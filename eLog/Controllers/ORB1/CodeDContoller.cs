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
    public class CodeDController : Controller
    {
        private readonly DatabaseHelper _db;
        private readonly IConfiguration _configuration;

        public CodeDController(DatabaseHelper db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        // Fetch data using stored procedure        
        [Route("ORB1/CodeD/GetCodeDData")]
        [HttpGet]
        public async Task<IActionResult> GetCodeDData(int pageNumber = 1, int pageSize = 10)
        {
            List<CodeDViewModel> records = new List<CodeDViewModel>();
            int totalRecords = 0;
            using (SqlConnection connection = _db.CreateConnection()) // Ensure this method exists in DatabaseHelper
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("proc_GetORB1_CodeD", connection)) // Fixed stored procedure name
                {
                    command.CommandType = CommandType.StoredProcedure;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            records.Add(new CodeDViewModel
                            {
                                Id = reader.GetInt32(0),
                                UserId = reader.IsDBNull(1) ? null : reader.GetString(1),
                                EntryDate = reader.GetDateTime(2),
                                MethodDischargeTransferDisposal = reader.IsDBNull(3) ? null : reader.GetString(3),
                                EquipmentQuantity = reader.IsDBNull(4) ? (decimal?)null : reader.GetDecimal(4),
                                EquipmentResidue = reader.IsDBNull(5) ? null : reader.GetString(5),
                                EquipmentTransferredFrom = reader.IsDBNull(6) ? null : reader.GetString(6),
                                EquipmentQuantityRetained = reader.IsDBNull(7) ? (decimal?)null : reader.GetDecimal(7),
                                EquipmentStartTime = reader.IsDBNull(8) ? (TimeSpan?)null : reader.GetTimeSpan(8),
                                EquipmentPositionStart = reader.IsDBNull(9) ? null : reader.GetString(9),
                                EquipmentStopTime = reader.IsDBNull(10) ? (TimeSpan?)null : reader.GetTimeSpan(10),
                                EquipmentPositionStop = reader.IsDBNull(11) ? null : reader.GetString(11),
                                ReceptionQuantity = reader.IsDBNull(12) ? (decimal?)null : reader.GetDecimal(12),
                                ReceptionResidue = reader.IsDBNull(13) ? null : reader.GetString(13),
                                ReceptionTransferredFrom = reader.IsDBNull(14) ? null : reader.GetString(14),
                                ReceptionQuantityRetained = reader.IsDBNull(15) ? (decimal?)null : reader.GetDecimal(15),
                                ReceptionStartTime = reader.IsDBNull(16) ? (TimeSpan?)null : reader.GetTimeSpan(16),
                                ReceptionStopTime = reader.IsDBNull(17) ? (TimeSpan?)null : reader.GetTimeSpan(17),
                                ReceptionPortFacilities = reader.IsDBNull(18) ? null : reader.GetString(18),
                                ReceptionReceiptNo = reader.IsDBNull(19) ? null : reader.GetString(19),
                                SlopTransferredTo = reader.IsDBNull(20) ? null : reader.GetString(20),
                                SlopQuantity = reader.IsDBNull(21) ? (decimal?)null : reader.GetDecimal(21),
                                SlopResidue = reader.IsDBNull(22) ? null : reader.GetString(22),
                                SlopTransferredFrom = reader.IsDBNull(23) ? null : reader.GetString(23),
                                SlopQuantityRetainedFrom = reader.IsDBNull(24) ? (decimal?)null : reader.GetDecimal(24),
                                SlopStartTime = reader.IsDBNull(25) ? (TimeSpan?)null : reader.GetTimeSpan(25),
                                SlopStopTime = reader.IsDBNull(26) ? (TimeSpan?)null : reader.GetTimeSpan(26),
                                SlopQuantityRetainedTo = reader.IsDBNull(27) ? (decimal?)null : reader.GetDecimal(27),
                                StatusName = reader.IsDBNull(28) ? null : reader.GetString(28),
                                ApprovedBy = reader.IsDBNull(29) ? null : reader.GetString(29),
                                Comments = reader.IsDBNull(30) ? null : reader.GetString(30)
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
            return View("~/Views/ORB1/CodeD.cshtml", paginatedRecords);
        }

        // Make sure this method has the correct attribute and route
        [HttpGet]
        [Route("ORB1/CodeD/GetTanks")] // Add this if missing
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
        public IActionResult DataEntry_CodeD(string userId, string userName, string userRoleName)
        {
            ViewBag.UserID = userId;
            ViewBag.UserName = userName;
            ViewBag.UserRoleName = userRoleName;
            return View("~/Views/ORB1/DataEntry_CodeD.cshtml", new CodeDModel());
        }

        // Edit data entry page (fetch record by Id)
        public async Task<IActionResult> Edit(int id, string userId, string userName, string userRoleName)
        {
            // Store user details in ViewBag (so they can be used in the view)
            ViewBag.UserID = userId;
            ViewBag.UserName = userName;
            ViewBag.UserRoleName = userRoleName;
            CodeDViewModel recordToEdit = null;

            try
            {
                using (SqlConnection connection = _db.CreateConnection())
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("proc_GetCodeDById", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync()) // Ensure data exists before reading
                            {
                                recordToEdit = new CodeDViewModel
                                {
                                    Id = reader.GetInt32(0),
                                    UserId = reader.IsDBNull(1) ? null : reader.GetString(1),
                                    EntryDate = reader.GetDateTime(2),
                                    MethodDischargeTransferDisposal = reader.IsDBNull(3) ? null : reader.GetString(3),
                                    EquipmentQuantity = reader.IsDBNull(4) ? (decimal?)null : reader.GetDecimal(4),
                                    EquipmentResidue = reader.IsDBNull(5) ? null : reader.GetString(5),
                                    EquipmentTransferredFrom = reader.IsDBNull(6) ? null : reader.GetString(6),
                                    EquipmentQuantityRetained = reader.IsDBNull(7) ? (decimal?)null : reader.GetDecimal(7),
                                    EquipmentStartTime = reader.IsDBNull(8) ? (TimeSpan?)null : reader.GetTimeSpan(8),
                                    EquipmentPositionStart = reader.IsDBNull(9) ? null : reader.GetString(9),
                                    EquipmentStopTime = reader.IsDBNull(10) ? (TimeSpan?)null : reader.GetTimeSpan(10),
                                    EquipmentPositionStop = reader.IsDBNull(11) ? null : reader.GetString(11),
                                    ReceptionQuantity = reader.IsDBNull(12) ? (decimal?)null : reader.GetDecimal(12),
                                    ReceptionResidue = reader.IsDBNull(13) ? null : reader.GetString(13),
                                    ReceptionTransferredFrom = reader.IsDBNull(14) ? null : reader.GetString(14),
                                    ReceptionQuantityRetained = reader.IsDBNull(15) ? (decimal?)null : reader.GetDecimal(15),
                                    ReceptionStartTime = reader.IsDBNull(16) ? (TimeSpan?)null : reader.GetTimeSpan(16),
                                    ReceptionStopTime = reader.IsDBNull(17) ? (TimeSpan?)null : reader.GetTimeSpan(17),
                                    ReceptionPortFacilities = reader.IsDBNull(18) ? null : reader.GetString(18),
                                    ReceptionReceiptNo = reader.IsDBNull(19) ? null : reader.GetString(19),
                                    SlopTransferredTo = reader.IsDBNull(20) ? null : reader.GetString(20),
                                    SlopQuantity = reader.IsDBNull(21) ? (decimal?)null : reader.GetDecimal(21),
                                    SlopResidue = reader.IsDBNull(22) ? null : reader.GetString(22),
                                    SlopTransferredFrom = reader.IsDBNull(23) ? null : reader.GetString(23),
                                    SlopQuantityRetainedFrom = reader.IsDBNull(24) ? (decimal?)null : reader.GetDecimal(24),
                                    SlopStartTime = reader.IsDBNull(25) ? (TimeSpan?)null : reader.GetTimeSpan(25),
                                    SlopStopTime = reader.IsDBNull(26) ? (TimeSpan?)null : reader.GetTimeSpan(26),
                                    SlopQuantityRetainedTo = reader.IsDBNull(27) ? (decimal?)null : reader.GetDecimal(27),
                                    StatusName = reader.IsDBNull(28) ? null : reader.GetString(28),
                                    ApprovedBy = reader.IsDBNull(28) ? null : reader.GetString(28),
                                    Comments = reader.IsDBNull(30) ? null : reader.GetString(30)
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
                    "Level 1- Entry" => View("~/Views/ORB1/DataEdit_CodeD.cshtml", recordToEdit),
                    "Level 2- Approver" => View("~/Views/ORB1/Approver_DataEdit_CodeD.cshtml", recordToEdit),
                    _ => Forbid() // Handle unexpected roles
                };
            }
            catch (Exception ex)
            {
                // Include a detailed error message with stack trace for debugging
                string errorDetails = $"Error: {ex.Message}\nStack Trace: {ex.StackTrace}";
                // Log the error details
                // _logger.LogError(errorDetails);

                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [Route("ORB1/CodeD/Create")]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CodeDModel model, IFormFile attachment)
        {
            if (model == null)
            {
                return BadRequest("Invalid data received.");
            }
            try
            {

                // Fetch file storage path from appsettings.json
                var uploadPath = _configuration["FileStorage:CodeD_ReceptionAttachment"];
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                string filePath = null;
                if (attachment != null && attachment.Length > 0)
                {
                    // Generate unique file name: ReceiptNo_OriginalName_Timestamp.ext
                    string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                    string fileExtension = Path.GetExtension(attachment.FileName);
                    string fileName = $"{model.ReceptionReceiptNo}_{Path.GetFileNameWithoutExtension(attachment.FileName)}_{timestamp}{fileExtension}";
                    filePath = Path.Combine(uploadPath, fileName);

                    // Save the file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await attachment.CopyToAsync(stream);
                    }
                }

                string storedProcedure = "proc_InsertORB1CodeD";
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@UserId", model.UserId),
                    new SqlParameter("@EntryDate", model.EntryDate),
                    new SqlParameter("@MethodDischargeTransferDisposal", (object)model.MethodDischargeTransferDisposal ?? DBNull.Value),
            
                    // Equipment Fields
                    new SqlParameter("@EquipmentQuantity", model.EquipmentQuantity ?? (object)DBNull.Value),
                    new SqlParameter("@EquipmentResidue", (object)model.EquipmentResidue ?? DBNull.Value),
                    new SqlParameter("@EquipmentTransferredFrom", (object)model.EquipmentTransferredFrom ?? DBNull.Value),
                    new SqlParameter("@EquipmentQuantityRetained", model.EquipmentQuantityRetained ?? (object)DBNull.Value),
                    new SqlParameter("@EquipmentStartTime", (object)model.EquipmentStartTime ?? DBNull.Value),
                    new SqlParameter("@EquipmentPositionStart", (object)model.EquipmentPositionStart ?? DBNull.Value),
                    new SqlParameter("@EquipmentStopTime", (object)model.EquipmentStopTime ?? DBNull.Value),
                    new SqlParameter("@EquipmentPositionStop", (object)model.EquipmentPositionStop ?? DBNull.Value),
            
                    // Reception Fields
                    new SqlParameter("@ReceptionQuantity", model.ReceptionQuantity ?? (object)DBNull.Value),
                    new SqlParameter("@ReceptionResidue", (object)model.ReceptionResidue ?? DBNull.Value),
                    new SqlParameter("@ReceptionTransferredFrom", (object)model.ReceptionTransferredFrom ?? DBNull.Value),
                    new SqlParameter("@ReceptionQuantityRetained", model.ReceptionQuantityRetained ?? (object)DBNull.Value),
                    new SqlParameter("@ReceptionStartTime", (object)model.ReceptionStartTime ?? DBNull.Value),
                    new SqlParameter("@ReceptionStopTime", (object)model.ReceptionStopTime ?? DBNull.Value),
                    new SqlParameter("@ReceptionPortFacilities", (object)model.ReceptionPortFacilities ?? DBNull.Value),
                    new SqlParameter("@ReceptionReceiptNo", (object)model.ReceptionReceiptNo ?? DBNull.Value),
                    new SqlParameter("@AttachmentPath", (object)filePath ?? DBNull.Value),
            
                    // Slop Fields
                    new SqlParameter("@SlopTransferredTo", (object)model.SlopTransferredTo ?? DBNull.Value),
                    new SqlParameter("@SlopQuantity", model.SlopQuantity ?? (object)DBNull.Value),
                    new SqlParameter("@SlopResidue", (object)model.SlopResidue ?? DBNull.Value),
                    new SqlParameter("@SlopTransferredFrom", (object)model.SlopTransferredFrom ?? DBNull.Value),
                    new SqlParameter("@SlopQuantityRetainedFrom", model.SlopQuantityRetainedFrom ?? (object)DBNull.Value),
                    new SqlParameter("@SlopStartTime", (object)model.SlopStartTime ?? DBNull.Value),
                    new SqlParameter("@SlopStopTime", (object)model.SlopStopTime ?? DBNull.Value),
                    new SqlParameter("@SlopQuantityRetainedTo", model.SlopQuantityRetainedTo ?? (object)DBNull.Value)
                };

                int InsertedId = _db.ExecuteInsertStoredProcedure(storedProcedure, parameters);
                if (!Convert.IsDBNull(InsertedId) && InsertedId > 0)
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

        [Route("ORB1/CodeD/Update")]
        [HttpPost]
        public IActionResult Update([FromBody] CodeDViewModel model)
        {
            if (model == null || model.Id <= 0)
            {
                return BadRequest("Invalid data received or ID is missing.");
            }

            try
            {
                string storedProcedure = "proc_UpdateORB1CodeD";
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@Id", model.Id),
                    new SqlParameter("@UserId", model.UserId),
                    new SqlParameter("@EntryDate", model.EntryDate),
                    new SqlParameter("@MethodDischargeTransferDisposal", (object)model.MethodDischargeTransferDisposal ?? DBNull.Value),
            
                    // Equipment Fields
                    new SqlParameter("@EquipmentQuantity", model.EquipmentQuantity ?? (object)DBNull.Value),
                    new SqlParameter("@EquipmentResidue", (object)model.EquipmentResidue ?? DBNull.Value),
                    new SqlParameter("@EquipmentTransferredFrom", (object)model.EquipmentTransferredFrom ?? DBNull.Value),
                    new SqlParameter("@EquipmentQuantityRetained", model.EquipmentQuantityRetained ?? (object)DBNull.Value),
                    new SqlParameter("@EquipmentStartTime", (object)model.EquipmentStartTime ?? DBNull.Value),
                    new SqlParameter("@EquipmentPositionStart", (object)model.EquipmentPositionStart ?? DBNull.Value),
                    new SqlParameter("@EquipmentStopTime", (object)model.EquipmentStopTime ?? DBNull.Value),
                    new SqlParameter("@EquipmentPositionStop", (object)model.EquipmentPositionStop ?? DBNull.Value),
            
                    // Reception Fields
                    new SqlParameter("@ReceptionQuantity", model.ReceptionQuantity ?? (object)DBNull.Value),
                    new SqlParameter("@ReceptionResidue", (object)model.ReceptionResidue ?? DBNull.Value),
                    new SqlParameter("@ReceptionTransferredFrom", (object)model.ReceptionTransferredFrom ?? DBNull.Value),
                    new SqlParameter("@ReceptionQuantityRetained", model.ReceptionQuantityRetained ?? (object)DBNull.Value),
                    new SqlParameter("@ReceptionStartTime", (object)model.ReceptionStartTime ?? DBNull.Value),
                    new SqlParameter("@ReceptionStopTime", (object)model.ReceptionStopTime ?? DBNull.Value),
                    new SqlParameter("@ReceptionPortFacilities", (object)model.ReceptionPortFacilities ?? DBNull.Value),
                    new SqlParameter("@ReceptionReceiptNo", (object)model.ReceptionReceiptNo ?? DBNull.Value),
            
                    // Slop Fields
                    new SqlParameter("@SlopTransferredTo", (object)model.SlopTransferredTo ?? DBNull.Value),
                    new SqlParameter("@SlopQuantity", model.SlopQuantity ?? (object)DBNull.Value),
                    new SqlParameter("@SlopResidue", (object)model.SlopResidue ?? DBNull.Value),
                    new SqlParameter("@SlopTransferredFrom", (object)model.SlopTransferredFrom ?? DBNull.Value),
                    new SqlParameter("@SlopQuantityRetainedFrom", model.SlopQuantityRetainedFrom ?? (object)DBNull.Value),
                    new SqlParameter("@SlopStartTime", (object)model.SlopStartTime ?? DBNull.Value),
                    new SqlParameter("@SlopStopTime", (object)model.SlopStopTime ?? DBNull.Value),
                    new SqlParameter("@SlopQuantityRetainedTo", model.SlopQuantityRetainedTo ?? (object)DBNull.Value)
                };

                int rowsAffected = _db.ExecuteUpdateStoredProcedure(storedProcedure, parameters);
                if (rowsAffected > 0)
                {
                    return Json(new { success = true, message = "Data Updated Successfully!" });
                }
                else
                {
                    return Json(new { success = false, message = "No changes made or record not found." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("ORB1/CodeD/ApproverUpdate")]
        public IActionResult ApproverUpdate([FromBody] CodeDViewModel model)
        {
            if (model == null || model.Id <= 0)
            {
                return BadRequest("Invalid data received or ID is missing.");
            }

            try
            {
                string storedProcedure = "proc_ApproverUpdateORB1CodeD";
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@Id", model.Id),
                    new SqlParameter("@UserId", model.UserId),
                    new SqlParameter("@EntryDate", model.EntryDate),
                    new SqlParameter("@MethodDischargeTransferDisposal", (object)model.MethodDischargeTransferDisposal ?? DBNull.Value),
            
                    // Equipment Fields
                    new SqlParameter("@EquipmentQuantity", model.EquipmentQuantity ?? (object)DBNull.Value),
                    new SqlParameter("@EquipmentResidue", (object)model.EquipmentResidue ?? DBNull.Value),
                    new SqlParameter("@EquipmentTransferredFrom", (object)model.EquipmentTransferredFrom ?? DBNull.Value),
                    new SqlParameter("@EquipmentQuantityRetained", model.EquipmentQuantityRetained ?? (object)DBNull.Value),
                    new SqlParameter("@EquipmentStartTime", (object)model.EquipmentStartTime ?? DBNull.Value),
                    new SqlParameter("@EquipmentPositionStart", (object)model.EquipmentPositionStart ?? DBNull.Value),
                    new SqlParameter("@EquipmentStopTime", (object)model.EquipmentStopTime ?? DBNull.Value),
                    new SqlParameter("@EquipmentPositionStop", (object)model.EquipmentPositionStop ?? DBNull.Value),
            
                    // Reception Fields
                    new SqlParameter("@ReceptionQuantity", model.ReceptionQuantity ?? (object)DBNull.Value),
                    new SqlParameter("@ReceptionResidue", (object)model.ReceptionResidue ?? DBNull.Value),
                    new SqlParameter("@ReceptionTransferredFrom", (object)model.ReceptionTransferredFrom ?? DBNull.Value),
                    new SqlParameter("@ReceptionQuantityRetained", model.ReceptionQuantityRetained ?? (object)DBNull.Value),
                    new SqlParameter("@ReceptionStartTime", (object)model.ReceptionStartTime ?? DBNull.Value),
                    new SqlParameter("@ReceptionStopTime", (object)model.ReceptionStopTime ?? DBNull.Value),
                    new SqlParameter("@ReceptionPortFacilities", (object)model.ReceptionPortFacilities ?? DBNull.Value),
                    new SqlParameter("@ReceptionReceiptNo", (object)model.ReceptionReceiptNo ?? DBNull.Value),
            
                    // Slop Fields
                    new SqlParameter("@SlopTransferredTo", (object)model.SlopTransferredTo ?? DBNull.Value),
                    new SqlParameter("@SlopQuantity", model.SlopQuantity ?? (object)DBNull.Value),
                    new SqlParameter("@SlopResidue", (object)model.SlopResidue ?? DBNull.Value),
                    new SqlParameter("@SlopTransferredFrom", (object)model.SlopTransferredFrom ?? DBNull.Value),
                    new SqlParameter("@SlopQuantityRetainedFrom", model.SlopQuantityRetainedFrom ?? (object)DBNull.Value),
                    new SqlParameter("@SlopStartTime", (object)model.SlopStartTime ?? DBNull.Value),
                    new SqlParameter("@SlopStopTime", (object)model.SlopStopTime ?? DBNull.Value),
                    new SqlParameter("@SlopQuantityRetainedTo", model.SlopQuantityRetainedTo ?? (object)DBNull.Value),
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