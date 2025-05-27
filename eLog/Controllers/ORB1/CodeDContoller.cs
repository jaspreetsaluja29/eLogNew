using ClosedXML.Excel;
using eLog.Models;
using eLog.Models.ORB1;
using eLog.ViewModels.ORB1;
using iTextSharp.text.pdf;
using iTextSharp.text;
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
        public IActionResult DataEntry_CodeD(string userId, string userName, string userRoleName, string jobRank)
        {
            ViewBag.UserID = userId;
            ViewBag.UserName = userName;
            ViewBag.UserRoleName = userRoleName;
            ViewBag.JobRank = jobRank;
            return View("~/Views/ORB1/DataEntry_CodeD.cshtml", new CodeDModel());
        }

        // Edit data entry page (fetch record by Id)
        public async Task<IActionResult> Edit(int id, string userId, string userName, string userRoleName, string jobRank)
        {
            ViewBag.UserID = userId;
            ViewBag.UserName = userName;
            ViewBag.UserRoleName = userRoleName;
            ViewBag.JobRank = jobRank;
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

        [HttpGet]
        [Route("ORB1/CodeD/Download")]
        public async Task<IActionResult> Download(string format)
        {
            try
            {
                // Fetch all data without filters or pagination
                var data = await GetCodeDForExport("", 1, 0); // Assuming 0 returns all records

                if (format?.ToLower() == "excel")
                {
                    return await ExportToExcel(data);
                }
                else if (format?.ToLower() == "pdf")
                {
                    return ExportToPdf(data); // Implement this method if not already
                }
                else
                {
                    return BadRequest("Invalid format specified.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private async Task<IActionResult> ExportToExcel(IEnumerable<CodeDViewModel> data)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Code D Records");

            // Define headers
            var headers = new List<string>
            {
                "Entered By",
                "Entry Date",
                "Method of Discharge/Transfer/Disposal",
                "Equipment Quantity",
                "Equipment Residue",
                "Equipment Transferred From",
                "Equipment Quantity Retained",
                "Equipment Start Time",
                "Equipment Position Start",
                "Equipment Stop Time",
                "Equipment Position Stop",
                "Reception Quantity",
                "Reception Residue",
                "Reception Transferred From",
                "Reception Quantity Retained",
                "Reception Start Time",
                "Reception Stop Time",
                "Reception Port Facilities",
                "Reception Receipt No",
                "Slop Transferred To",
                "Slop Quantity",
                "Slop Residue",
                "Slop Transferred From",
                "Slop Quantity Retained From",
                "Slop Start Time",
                "Slop Stop Time",
                "Slop Quantity Retained To",
                "Status Name",
                "Approved By",
                "Comments"
            };

            // Write headers
            for (int i = 0; i < headers.Count; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
                worksheet.Cell(1, i + 1).Style.Font.Bold = true;
            }

            // Write data
            int row = 2;
            foreach (var item in data)
            {
                worksheet.Cell(row, 1).Value = item.UserId?.Trim() ?? string.Empty;
                worksheet.Cell(row, 2).Value = item.EntryDate.ToShortDateString();
                worksheet.Cell(row, 3).Value = item.MethodDischargeTransferDisposal ?? string.Empty;
                worksheet.Cell(row, 4).Value = item.EquipmentQuantity?.ToString() ?? string.Empty;
                worksheet.Cell(row, 5).Value = item.EquipmentResidue ?? string.Empty;
                worksheet.Cell(row, 6).Value = item.EquipmentTransferredFrom ?? string.Empty;
                worksheet.Cell(row, 7).Value = item.EquipmentQuantityRetained?.ToString() ?? string.Empty;
                worksheet.Cell(row, 8).Value = item.EquipmentStartTime?.ToString(@"hh\:mm") ?? string.Empty;
                worksheet.Cell(row, 9).Value = item.EquipmentPositionStart ?? string.Empty;
                worksheet.Cell(row, 10).Value = item.EquipmentStopTime?.ToString(@"hh\:mm") ?? string.Empty;
                worksheet.Cell(row, 11).Value = item.EquipmentPositionStop ?? string.Empty;
                worksheet.Cell(row, 12).Value = item.ReceptionQuantity?.ToString() ?? string.Empty;
                worksheet.Cell(row, 13).Value = item.ReceptionResidue ?? string.Empty;
                worksheet.Cell(row, 14).Value = item.ReceptionTransferredFrom ?? string.Empty;
                worksheet.Cell(row, 15).Value = item.ReceptionQuantityRetained?.ToString() ?? string.Empty;
                worksheet.Cell(row, 16).Value = item.ReceptionStartTime?.ToString(@"hh\:mm") ?? string.Empty;
                worksheet.Cell(row, 17).Value = item.ReceptionStopTime?.ToString(@"hh\:mm") ?? string.Empty;
                worksheet.Cell(row, 18).Value = item.ReceptionPortFacilities ?? string.Empty;
                worksheet.Cell(row, 19).Value = item.ReceptionReceiptNo ?? string.Empty;
                worksheet.Cell(row, 20).Value = item.SlopTransferredTo ?? string.Empty;
                worksheet.Cell(row, 21).Value = item.SlopQuantity?.ToString() ?? string.Empty;
                worksheet.Cell(row, 22).Value = item.SlopResidue ?? string.Empty;
                worksheet.Cell(row, 23).Value = item.SlopTransferredFrom ?? string.Empty;
                worksheet.Cell(row, 24).Value = item.SlopQuantityRetainedFrom?.ToString() ?? string.Empty;
                worksheet.Cell(row, 25).Value = item.SlopStartTime?.ToString(@"hh\:mm") ?? string.Empty;
                worksheet.Cell(row, 26).Value = item.SlopStopTime?.ToString(@"hh\:mm") ?? string.Empty;
                worksheet.Cell(row, 27).Value = item.SlopQuantityRetainedTo?.ToString() ?? string.Empty;
                worksheet.Cell(row, 28).Value = item.StatusName ?? string.Empty;
                worksheet.Cell(row, 29).Value = item.ApprovedBy ?? string.Empty;
                worksheet.Cell(row, 30).Value = item.Comments ?? string.Empty;
                row++;
            }

            worksheet.Columns().AdjustToContents(); // Auto-fit columns

            // Save to memory stream
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            // Set the correct headers to force download
            var fileName = $"CodeD_Records_{DateTime.Now:yyyyMMdd}.xlsx";
            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName
            );
        }

        private FileContentResult ExportToPdf(IEnumerable<CodeDViewModel> data)
        {
            byte[] pdfBytes;

            using (var stream = new MemoryStream())
            {
                var document = new Document(PageSize.A1.Rotate(), 20f, 20f, 20f, 20f);
                PdfWriter.GetInstance(document, stream);

                document.Open();

                // Title
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                var title = new Paragraph("Code D Records", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER
                };
                document.Add(title);
                document.Add(new Paragraph(" "));

                // Table setup
                var table = new PdfPTable(30)
                {
                    WidthPercentage = 100
                };
                float[] columnWidths = Enumerable.Repeat(4f, 30).ToArray();
                table.SetWidths(columnWidths);

                var headers = new[]
                {
                    "Entered By",
                    "Entry Date",
                    "Method of Discharge/Transfer/Disposal",
                    "Equipment Quantity",
                    "Equipment Residue",
                    "Equipment Transferred From",
                    "Equipment Quantity Retained",
                    "Equipment Start Time",
                    "Equipment Position Start",
                    "Equipment Stop Time",
                    "Equipment Position Stop",
                    "Reception Quantity",
                    "Reception Residue",
                    "Reception Transferred From",
                    "Reception Quantity Retained",
                    "Reception Start Time",
                    "Reception Stop Time",
                    "Reception Port Facilities",
                    "Reception Receipt No",
                    "Slop Transferred To",
                    "Slop Quantity",
                    "Slop Residue",
                    "Slop Transferred From",
                    "Slop Quantity Retained From",
                    "Slop Start Time",
                    "Slop Stop Time",
                    "Slop Quantity Retained To",
                    "Status Name",
                    "Approved By",
                    "Comments"
                };

                var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8);
                foreach (var header in headers)
                {
                    var cell = new PdfPCell(new Phrase(header, headerFont))
                    {
                        BackgroundColor = BaseColor.LIGHT_GRAY,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        Padding = 4
                    };
                    table.AddCell(cell);
                }

                var dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 7);
                foreach (var item in data)
                {
                    AddCell(table, item.UserId ?? "", dataFont);
                    AddCell(table, item.EntryDate.ToShortDateString(), dataFont);
                    AddCell(table, item.MethodDischargeTransferDisposal ?? "", dataFont);
                    AddCell(table, item.EquipmentResidue ?? "", dataFont);
                    AddCell(table, item.EquipmentTransferredFrom ?? "", dataFont);
                    AddCell(table, item.EquipmentQuantity?.ToString() ?? "", dataFont);
                    AddCell(table, item.EquipmentQuantityRetained?.ToString() ?? "", dataFont);
                    AddCell(table, item.EquipmentStartTime?.ToString(@"hh\:mm") ?? "", dataFont);
                    AddCell(table, item.EquipmentPositionStart ?? "", dataFont);
                    AddCell(table, item.EquipmentStopTime?.ToString(@"hh\:mm") ?? "", dataFont);
                    AddCell(table, item.EquipmentPositionStop ?? "", dataFont);
                    AddCell(table, item.ReceptionTransferredFrom ?? "", dataFont);
                    AddCell(table, item.ReceptionResidue ?? "", dataFont);
                    AddCell(table, item.ReceptionQuantity?.ToString() ?? "", dataFont);
                    AddCell(table, item.ReceptionQuantityRetained?.ToString() ?? "", dataFont);
                    AddCell(table, item.ReceptionStartTime?.ToString(@"hh\:mm") ?? "", dataFont);
                    AddCell(table, item.ReceptionStopTime?.ToString(@"hh\:mm") ?? "", dataFont);
                    AddCell(table, item.ReceptionPortFacilities ?? "", dataFont);
                    AddCell(table, item.ReceptionReceiptNo ?? "", dataFont);
                    AddCell(table, item.SlopTransferredTo ?? "", dataFont);
                    AddCell(table, item.SlopQuantity?.ToString() ?? "", dataFont);
                    AddCell(table, item.SlopResidue ?? "", dataFont);
                    AddCell(table, item.SlopTransferredFrom ?? "", dataFont);
                    AddCell(table, item.SlopQuantityRetainedFrom?.ToString() ?? "", dataFont);
                    AddCell(table, item.SlopStartTime?.ToString(@"hh\:mm") ?? "", dataFont);
                    AddCell(table, item.SlopStopTime?.ToString(@"hh\:mm") ?? "", dataFont);
                    AddCell(table, item.SlopQuantityRetainedTo?.ToString() ?? "", dataFont);
                    AddCell(table, item.StatusName ?? "", dataFont);
                    AddCell(table, item.ApprovedBy ?? "", dataFont);
                    AddCell(table, item.Comments ?? "", dataFont);
                }

                document.Add(table);
                document.Close();

                // ✅ Copy to byte array BEFORE stream is disposed
                pdfBytes = stream.ToArray();
            }

            return File(pdfBytes, "application/pdf", $"CodeD_Records_{DateTime.Now:yyyyMMdd}.pdf");
        }

        private void AddCell(PdfPTable table, string text, iTextSharp.text.Font font)
        {
            var cell = new PdfPCell(new Phrase(text, font))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Padding = 3
            };
            table.AddCell(cell);
        }

        public async Task<List<CodeDViewModel>> GetCodeDForExport(string searchTerm = "", int pageNumber = 1, int pageSize = 10)
        {
            List<CodeDViewModel> records = new List<CodeDViewModel>();

            using (SqlConnection connection = _db.CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("proc_GetORB1_CodeD", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add search parameter if provided
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        command.Parameters.AddWithValue("@SearchTerm", searchTerm);
                    }

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

            // Apply pagination if needed - for exports, you might want all data
            if (pageSize > 0)
            {
                return records.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            }
            return records;
        }
    }
}