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
    public class CodeCController : Controller
    {
        private readonly DatabaseHelper _db;
        private readonly IConfiguration _configuration;

        public CodeCController(DatabaseHelper db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        // Fetch data using stored procedure        
        [Route("ORB1/CodeC/GetCodeCData")]
        [HttpGet]
        public async Task<IActionResult> GetCodeCData(int pageNumber = 1, int pageSize = 10)
        {
            List<CodeCViewModel> records = new List<CodeCViewModel>();
            int totalRecords = 0;
            using (SqlConnection connection = _db.CreateConnection()) // Ensure this method exists in DatabaseHelper
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("proc_GetORB1_CodeC", connection)) // Fixed stored procedure name
                {
                    command.CommandType = CommandType.StoredProcedure;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            records.Add(new CodeCViewModel
                            {
                                Id = reader.GetInt32(0),
                                UserId = reader.IsDBNull(1) ? null : reader.GetString(1),
                                EntryDate = reader.GetDateTime(2),
                                CollectionType = reader.IsDBNull(3) ? null : reader.GetString(3),
                                WeeklyIdentityOfTanks = reader.IsDBNull(4) ? null : reader.GetString(4),
                                WeeklyCapacityOfTanks = reader.IsDBNull(5) ? (decimal?)null : reader.GetDecimal(5),
                                WeeklyTotalQuantityOfRetention = reader.IsDBNull(6) ? (decimal?)null : reader.GetDecimal(6),
                                CollectionIdentityOfTanks = reader.IsDBNull(7) ? null : reader.GetString(7),
                                CollectionCapacityOfTanks = reader.IsDBNull(8) ? (decimal?)null : reader.GetDecimal(8),
                                CollectionTotalQuantityOfRetention = reader.IsDBNull(9) ? (decimal?)null : reader.GetDecimal(9),
                                CollectionManualResidueQuantity = reader.IsDBNull(10) ? (decimal?)null : reader.GetDecimal(10),
                                CollectionCollectedFromTank = reader.IsDBNull(11) ? null : reader.GetString(11),
                                TransferOperationType = reader.IsDBNull(12) ? null : reader.GetString(12),
                                TransferQuantity = reader.IsDBNull(13) ? (decimal?)null : reader.GetDecimal(13),
                                TransferTanksFrom = reader.IsDBNull(14) ? null : reader.GetString(14),
                                TransferRetainedInTransfer = reader.IsDBNull(15) ? null : reader.GetString(15),
                                TransferTanksTo = reader.IsDBNull(16) ? null : reader.GetString(16),
                                TransferRetainedInReceiving = reader.IsDBNull(17) ? null : reader.GetString(17),
                                IncineratorOperationType = reader.IsDBNull(18) ? null : reader.GetString(18),
                                IncineratorQuantity = reader.IsDBNull(19) ? (decimal?)null : reader.GetDecimal(19),
                                IncineratorTanksFrom = reader.IsDBNull(20) ? null : reader.GetString(20),
                                IncineratorTotalRetainedContent = reader.IsDBNull(21) ? (decimal?)null : reader.GetDecimal(21),
                                IncineratorStartTime = reader.IsDBNull(22) ? (TimeSpan?)null : reader.GetTimeSpan(22),
                                IncineratorStopTime = reader.IsDBNull(23) ? (TimeSpan?)null : reader.GetTimeSpan(23),
                                IncineratorTotalOperationTime = reader.IsDBNull(24) ? (decimal?)null : reader.GetDecimal(24),
                                DisposalShipQuantity = reader.IsDBNull(25) ? (decimal?)null : reader.GetDecimal(25),
                                DisposalShipTanksFrom = reader.IsDBNull(26) ? null : reader.GetString(26),
                                DisposalShipRetainedIn = reader.IsDBNull(27) ? null : reader.GetString(27),
                                DisposalShipTanksTo = reader.IsDBNull(28) ? null : reader.GetString(28),
                                DisposalShipRetainedTo = reader.IsDBNull(29) ? null : reader.GetString(29),
                                DisposalShoreQuantity = reader.IsDBNull(30) ? (decimal?)null : reader.GetDecimal(30),
                                DisposalShoreTanksFrom = reader.IsDBNull(31) ? null : reader.GetString(31),
                                DisposalShoreRetainedInDischargeTanks = reader.IsDBNull(32) ? null : reader.GetString(32),
                                DisposalShoreBargeName = reader.IsDBNull(33) ? null : reader.GetString(33),
                                DisposalShoreReceptionFacility = reader.IsDBNull(34) ? null : reader.GetString(34),
                                DisposalShoreReceiptNo = reader.IsDBNull(35) ? null : reader.GetString(35),
                                StatusName = reader.IsDBNull(36) ? null : reader.GetString(36),
                                ApprovedBy = reader.IsDBNull(37) ? null : reader.GetString(37),
                                Comments = reader.IsDBNull(38) ? null : reader.GetString(38)
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
            return View("~/Views/ORB1/CodeC.cshtml", paginatedRecords);
        }

        // Make sure this method has the correct attribute and route
        [HttpGet]
        [Route("ORB1/CodeC/GetTanks")] // Add this if missing
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

        [HttpGet]
        [Route("ORB1/CodeC/GetLastWeeklyRetention")]
        public JsonResult GetLastWeeklyRetention(string WeeklyIdentityOfTanks)
        {
            if (string.IsNullOrEmpty(WeeklyIdentityOfTanks))
            {
                return Json(new { error = "Identity Of Tanks is required" });
            }

            List<object> lastWeekData = new List<object>();

            try
            {
                using (SqlConnection connection = _db.CreateConnection())
                {
                    using (SqlCommand cmd = new SqlCommand("proc_GetORB1_CodeC_LastWeeklyRetention", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@WeeklyIdentityOfTanks", WeeklyIdentityOfTanks); // Pass tank ID

                        connection.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lastWeekData.Add(new
                                {
                                    WeeklyTotalQuantityOfRetention = reader["WeeklyTotalQuantityOfRetention"]?.ToString() ?? "0"
                                });
                            }
                        }
                    }
                }

                return Json(new { success = true, data = lastWeekData });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }



        // Make sure this method has the correct attribute and route
        [HttpGet]
        [Route("ORB1/CodeC/GetLastWeeklyCollectionRetention")] // Add this if missing
        public JsonResult GetLastWeeklyCollectionRetention(string CollectionIdentityOfTanks)
        {
            List<object> lastWeekData = new List<object>();

            try // Add error handling
            {
                using (SqlConnection connection = _db.CreateConnection())
                {
                    using (SqlCommand cmd = new SqlCommand("proc_GetORB1_CodeC_LastWeeklyCollectionRetention", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CollectionIdentityOfTanks", CollectionIdentityOfTanks); // Pass tank ID

                        connection.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lastWeekData.Add(new
                                {
                                    CollectionTotalQuantityOfRetention = reader["CollectionTotalQuantityOfRetention"].ToString()
                                });
                            }
                        }
                    }
                }
                return Json(new { success = true, data = lastWeekData });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        // Data Entry Page
        public IActionResult DataEntry_CodeC(string userId, string userName, string userRoleName)
        {
            ViewBag.UserID = userId;
            ViewBag.UserName = userName;
            ViewBag.UserRoleName = userRoleName;
            return View("~/Views/ORB1/DataEntry_CodeC.cshtml", new CodeCModel());
        }

        // Edit data entry page (fetch record by Id)
        public async Task<IActionResult> Edit(int id, string userId, string userName, string userRoleName)
        {
            // Store user details in ViewBag (so they can be used in the view)
            ViewBag.UserID = userId;
            ViewBag.UserName = userName;
            ViewBag.UserRoleName = userRoleName;
            CodeCViewModel recordToEdit = null;

            try
            {
                using (SqlConnection connection = _db.CreateConnection())
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("proc_GetCodeCById", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync()) // Ensure data exists before reading
                            {
                                recordToEdit = new CodeCViewModel
                                {
                                    Id = reader.GetInt32(0),
                                    UserId = reader.IsDBNull(1) ? null : reader.GetString(1),
                                    EntryDate = reader.GetDateTime(2),
                                    CollectionType = reader.IsDBNull(3) ? null : reader.GetString(3),
                                    WeeklyIdentityOfTanks = reader.IsDBNull(4) ? null : reader.GetString(4),
                                    WeeklyCapacityOfTanks = reader.IsDBNull(5) ? (decimal?)null : reader.GetDecimal(5),
                                    WeeklyTotalQuantityOfRetention = reader.IsDBNull(6) ? (decimal?)null : reader.GetDecimal(6),
                                    CollectionIdentityOfTanks = reader.IsDBNull(7) ? null : reader.GetString(7),
                                    CollectionCapacityOfTanks = reader.IsDBNull(8) ? (decimal?)null : reader.GetDecimal(8),
                                    CollectionTotalQuantityOfRetention = reader.IsDBNull(9) ? (decimal?)null : reader.GetDecimal(9),
                                    CollectionManualResidueQuantity = reader.IsDBNull(10) ? (decimal?)null : reader.GetDecimal(10),
                                    CollectionCollectedFromTank = reader.IsDBNull(11) ? null : reader.GetString(11),
                                    TransferOperationType = reader.IsDBNull(12) ? null : reader.GetString(12),
                                    TransferQuantity = reader.IsDBNull(13) ? (decimal?)null : reader.GetDecimal(13),
                                    TransferTanksFrom = reader.IsDBNull(14) ? null : reader.GetString(14),
                                    TransferRetainedInTransfer = reader.IsDBNull(15) ? null : reader.GetString(15),
                                    TransferTanksTo = reader.IsDBNull(16) ? null : reader.GetString(16),
                                    TransferRetainedInReceiving = reader.IsDBNull(17) ? null : reader.GetString(17),  // Corrected index
                                    IncineratorOperationType = reader.IsDBNull(18) ? null : reader.GetString(18),
                                    IncineratorQuantity = reader.IsDBNull(19) ? (decimal?)null : reader.GetDecimal(19),
                                    IncineratorTanksFrom = reader.IsDBNull(20) ? null : reader.GetString(20),
                                    IncineratorTotalRetainedContent = reader.IsDBNull(21) ? (decimal?)null : reader.GetDecimal(21),
                                    IncineratorStartTime = reader.IsDBNull(22) ? (TimeSpan?)null : reader.GetTimeSpan(22),
                                    IncineratorStopTime = reader.IsDBNull(23) ? (TimeSpan?)null : reader.GetTimeSpan(23),
                                    IncineratorTotalOperationTime = reader.IsDBNull(24) ? (decimal?)null : reader.GetDecimal(24),
                                    DisposalShipQuantity = reader.IsDBNull(25) ? (decimal?)null : reader.GetDecimal(25),
                                    DisposalShipTanksFrom = reader.IsDBNull(26) ? null : reader.GetString(26),
                                    DisposalShipRetainedIn = reader.IsDBNull(27) ? null : reader.GetString(27),
                                    DisposalShipTanksTo = reader.IsDBNull(28) ? null : reader.GetString(28),
                                    DisposalShipRetainedTo = reader.IsDBNull(29) ? null : reader.GetString(29),
                                    DisposalShoreQuantity = reader.IsDBNull(30) ? (decimal?)null : reader.GetDecimal(30),
                                    DisposalShoreTanksFrom = reader.IsDBNull(31) ? null : reader.GetString(31),
                                    DisposalShoreRetainedInDischargeTanks = reader.IsDBNull(32) ? null : reader.GetString(32),
                                    DisposalShoreBargeName = reader.IsDBNull(33) ? null : reader.GetString(33),
                                    DisposalShoreReceptionFacility = reader.IsDBNull(34) ? null : reader.GetString(34),
                                    DisposalShoreReceiptNo = reader.IsDBNull(35) ? null : reader.GetString(35),
                                    StatusName = reader.IsDBNull(36) ? null : reader.GetValue(36).ToString(),
                                    ApprovedBy = reader.IsDBNull(37) ? null : reader.GetValue(37).ToString(),
                                    Comments = reader.IsDBNull(38) ? null : reader.GetValue(38).ToString()  // Updated index to 38
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
                    "Level 1- Entry" => View("~/Views/ORB1/DataEdit_CodeC.cshtml", recordToEdit),
                    "Level 2- Approver" => View("~/Views/ORB1/Approver_DataEdit_CodeC.cshtml", recordToEdit),
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

        [Route("ORB1/CodeC/Create")]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CodeCModel model, IFormFile attachment)
        {
            if (model == null)
            {
                return BadRequest("Invalid data received.");
            }
            try
            {

                // Fetch file storage path from appsettings.json
                var uploadPath = _configuration["FileStorage:CodeC_DisposalShoreAttachmentPath"];
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
                    string fileName = $"{model.DisposalShoreReceiptNo}_{Path.GetFileNameWithoutExtension(attachment.FileName)}_{timestamp}{fileExtension}";
                    filePath = Path.Combine(uploadPath, fileName);

                    // Save the file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await attachment.CopyToAsync(stream);
                    }
                }

                string storedProcedure = "proc_InsertORB1CodeC";
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@UserId", model.UserId),
                    new SqlParameter("@EntryDate", model.EntryDate),
                    new SqlParameter("@CollectionType", model.CollectionType),
            
                    // Weekly Inventory Fields
                    new SqlParameter("@WeeklyIdentityOfTanks", (object)model.WeeklyIdentityOfTanks ?? DBNull.Value),
                    new SqlParameter("@WeeklyCapacityOfTanks", model.WeeklyCapacityOfTanks ?? (object)DBNull.Value),
                    new SqlParameter("@WeeklyTotalQuantityOfRetention", model.WeeklyTotalQuantityOfRetention ?? (object)DBNull.Value),
            
                    // Collection Fields
                    new SqlParameter("@CollectionIdentityOfTanks", (object)model.CollectionIdentityOfTanks ?? DBNull.Value),
                    new SqlParameter("@CollectionCapacityOfTanks", model.CollectionCapacityOfTanks ?? (object)DBNull.Value),
                    new SqlParameter("@CollectionTotalQuantityOfRetention", model.CollectionTotalQuantityOfRetention ?? (object)DBNull.Value),
                    new SqlParameter("@CollectionManualResidueQuantity", model.CollectionManualResidueQuantity ?? (object)DBNull.Value),
                    new SqlParameter("@CollectionCollectedFromTank", (object)model.CollectionCollectedFromTank ?? DBNull.Value),
            
                    // Transfer Fields
                    new SqlParameter("@TransferOperationType", (object)model.TransferOperationType ?? DBNull.Value),
                    new SqlParameter("@TransferQuantity", model.TransferQuantity ?? (object)DBNull.Value),
                    new SqlParameter("@TransferTanksFrom", (object)model.TransferTanksFrom ?? DBNull.Value),
                    new SqlParameter("@TransferRetainedInTransfer", (object)model.TransferRetainedInTransfer ?? DBNull.Value),
                    new SqlParameter("@TransferTanksTo", (object)model.TransferTanksTo ?? DBNull.Value),
                    new SqlParameter("@TransferRetainedInReceiving", (object)model.TransferRetainedInReceiving ?? DBNull.Value),
            
                    // Incinerator Fields
                    new SqlParameter("@IncineratorOperationType", (object)model.IncineratorOperationType ?? DBNull.Value),
                    new SqlParameter("@IncineratorQuantity", model.IncineratorQuantity ?? (object)DBNull.Value),
                    new SqlParameter("@IncineratorTanksFrom", (object)model.IncineratorTanksFrom ?? DBNull.Value),
                    new SqlParameter("@IncineratorTotalRetainedContent", model.IncineratorTotalRetainedContent ?? (object)DBNull.Value),
                    new SqlParameter("@IncineratorStartTime", (object)model.IncineratorStartTime ?? DBNull.Value),
                    new SqlParameter("@IncineratorStopTime", (object)model.IncineratorStopTime ?? DBNull.Value),
                    new SqlParameter("@IncineratorTotalOperationTime", model.IncineratorTotalOperationTime ?? (object)DBNull.Value),
            
                    // Disposal Ship Fields
                    new SqlParameter("@DisposalShipQuantity", model.DisposalShipQuantity ?? (object)DBNull.Value),
                    new SqlParameter("@DisposalShipTanksFrom", (object)model.DisposalShipTanksFrom ?? DBNull.Value),
                    new SqlParameter("@DisposalShipRetainedIn", (object)model.DisposalShipRetainedIn ?? DBNull.Value),
                    new SqlParameter("@DisposalShipTanksTo", (object)model.DisposalShipTanksTo ?? DBNull.Value),
                    new SqlParameter("@DisposalShipRetainedTo", (object)model.DisposalShipRetainedTo ?? DBNull.Value),
            
                    // Disposal Shore Fields
                    new SqlParameter("@DisposalShoreQuantity", model.DisposalShoreQuantity ?? (object)DBNull.Value),
                    new SqlParameter("@DisposalShoreTanksFrom", (object)model.DisposalShoreTanksFrom ?? DBNull.Value),
                    new SqlParameter("@DisposalShoreRetainedInDischargeTanks", (object)model.DisposalShoreRetainedInDischargeTanks ?? DBNull.Value),
                    new SqlParameter("@DisposalShoreBargeName", (object)model.DisposalShoreBargeName ?? DBNull.Value),
                    new SqlParameter("@DisposalShoreReceptionFacility", (object)model.DisposalShoreReceptionFacility ?? DBNull.Value),
                    new SqlParameter("@DisposalShoreReceiptNo", (object)model.DisposalShoreReceiptNo ?? DBNull.Value),
                    new SqlParameter("@AttachmentPath", (object)filePath ?? DBNull.Value) // Save file path in DB
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

        [Route("ORB1/CodeC/Update")]
        [HttpPost]
        public IActionResult Update([FromBody] CodeCViewModel model)
        {
            if (model == null || model.Id <= 0)
            {
                return BadRequest("Invalid data received or ID is missing.");
            }
            if (model.TransferOperationType == "Select")
            {
                model.TransferOperationType = null;
            }
            if (model.IncineratorOperationType == "Incineration" && model.CollectionType != "Incinerator")
            {
                model.IncineratorOperationType = null;
            }

            try
            {
                string storedProcedure = "proc_UpdateORB1CodeC";
                var parameters = new SqlParameter[]
                {
            new SqlParameter("@Id", model.Id),
            new SqlParameter("@UserId", model.UserId),
            new SqlParameter("@EntryDate", model.EntryDate),
            new SqlParameter("@CollectionType", model.CollectionType),
    
            // Weekly Inventory Fields
            new SqlParameter("@WeeklyIdentityOfTanks", (object)model.WeeklyIdentityOfTanks ?? DBNull.Value),
            new SqlParameter("@WeeklyCapacityOfTanks", model.WeeklyCapacityOfTanks ?? (object)DBNull.Value),
            new SqlParameter("@WeeklyTotalQuantityOfRetention", model.WeeklyTotalQuantityOfRetention ?? (object)DBNull.Value),
    
            // Collection Fields
            new SqlParameter("@CollectionIdentityOfTanks", (object)model.CollectionIdentityOfTanks ?? DBNull.Value),
            new SqlParameter("@CollectionCapacityOfTanks", model.CollectionCapacityOfTanks ?? (object)DBNull.Value),
            new SqlParameter("@CollectionTotalQuantityOfRetention", model.CollectionTotalQuantityOfRetention ?? (object)DBNull.Value),
            new SqlParameter("@CollectionManualResidueQuantity", model.CollectionManualResidueQuantity ?? (object)DBNull.Value),
            new SqlParameter("@CollectionCollectedFromTank", (object)model.CollectionCollectedFromTank ?? DBNull.Value),
    
            // Transfer Fields
            new SqlParameter("@TransferOperationType", (object)model.TransferOperationType ?? DBNull.Value),
            new SqlParameter("@TransferQuantity", model.TransferQuantity ?? (object)DBNull.Value),
            new SqlParameter("@TransferTanksFrom", (object)model.TransferTanksFrom ?? DBNull.Value),
            new SqlParameter("@TransferRetainedInTransfer", (object)model.TransferRetainedInTransfer ?? DBNull.Value),
            new SqlParameter("@TransferTanksTo", (object)model.TransferTanksTo ?? DBNull.Value),
            new SqlParameter("@TransferRetainedInReceiving", (object)model.TransferRetainedInReceiving ?? DBNull.Value),
    
            // Incinerator Fields
            new SqlParameter("@IncineratorOperationType", (object)model.IncineratorOperationType ?? DBNull.Value),
            new SqlParameter("@IncineratorQuantity", model.IncineratorQuantity ?? (object)DBNull.Value),
            new SqlParameter("@IncineratorTanksFrom", (object)model.IncineratorTanksFrom ?? DBNull.Value),
            new SqlParameter("@IncineratorTotalRetainedContent", model.IncineratorTotalRetainedContent ?? (object)DBNull.Value),
            new SqlParameter("@IncineratorStartTime", (object)model.IncineratorStartTime ?? DBNull.Value),
            new SqlParameter("@IncineratorStopTime", (object)model.IncineratorStopTime ?? DBNull.Value),
            new SqlParameter("@IncineratorTotalOperationTime", model.IncineratorTotalOperationTime ?? (object)DBNull.Value),
    
            // Disposal Ship Fields
            new SqlParameter("@DisposalShipQuantity", model.DisposalShipQuantity ?? (object)DBNull.Value),
            new SqlParameter("@DisposalShipTanksFrom", (object)model.DisposalShipTanksFrom ?? DBNull.Value),
            new SqlParameter("@DisposalShipRetainedIn", (object)model.DisposalShipRetainedIn ?? DBNull.Value),
            new SqlParameter("@DisposalShipTanksTo", (object)model.DisposalShipTanksTo ?? DBNull.Value),
            new SqlParameter("@DisposalShipRetainedTo", (object)model.DisposalShipRetainedTo ?? DBNull.Value),
    
            // Disposal Shore Fields
            new SqlParameter("@DisposalShoreQuantity", model.DisposalShoreQuantity ?? (object)DBNull.Value),
            new SqlParameter("@DisposalShoreTanksFrom", (object)model.DisposalShoreTanksFrom ?? DBNull.Value),
            new SqlParameter("@DisposalShoreRetainedInDischargeTanks", (object)model.DisposalShoreRetainedInDischargeTanks ?? DBNull.Value),
            new SqlParameter("@DisposalShoreBargeName", (object)model.DisposalShoreBargeName ?? DBNull.Value),
            new SqlParameter("@DisposalShoreReceptionFacility", (object)model.DisposalShoreReceptionFacility ?? DBNull.Value),
            new SqlParameter("@DisposalShoreReceiptNo", (object)model.DisposalShoreReceiptNo ?? DBNull.Value)
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
        [Route("ORB1/CodeC/ApproverUpdate")]
        public IActionResult ApproverUpdate([FromBody] CodeCViewModel model)
        {
            if (model == null || model.Id <= 0)
            {
                return BadRequest("Invalid data received or ID is missing.");
            }
            if (model.TransferOperationType == "Select")
            {
                model.TransferOperationType = null;
            }
            if (model.IncineratorOperationType == "Incineration" && model.CollectionType != "Incinerator")
            {
                model.IncineratorOperationType = null;
            }

            try
            {
                string storedProcedure = "proc_ApproverUpdateORB1CodeC";
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@Id", model.Id),
                    new SqlParameter("@UserId", model.UserId),
                    new SqlParameter("@EntryDate", model.EntryDate),
                    new SqlParameter("@CollectionType", model.CollectionType),
    
                    // Weekly Inventory Fields
                    new SqlParameter("@WeeklyIdentityOfTanks", (object)model.WeeklyIdentityOfTanks ?? DBNull.Value),
                    new SqlParameter("@WeeklyCapacityOfTanks", model.WeeklyCapacityOfTanks ?? (object)DBNull.Value),
                    new SqlParameter("@WeeklyTotalQuantityOfRetention", model.WeeklyTotalQuantityOfRetention ?? (object)DBNull.Value),
    
                    // Collection Fields
                    new SqlParameter("@CollectionIdentityOfTanks", (object)model.CollectionIdentityOfTanks ?? DBNull.Value),
                    new SqlParameter("@CollectionCapacityOfTanks", model.CollectionCapacityOfTanks ?? (object)DBNull.Value),
                    new SqlParameter("@CollectionTotalQuantityOfRetention", model.CollectionTotalQuantityOfRetention ?? (object)DBNull.Value),
                    new SqlParameter("@CollectionManualResidueQuantity", model.CollectionManualResidueQuantity ?? (object)DBNull.Value),
                    new SqlParameter("@CollectionCollectedFromTank", (object)model.CollectionCollectedFromTank ?? DBNull.Value),
    
                    // Transfer Fields
                    new SqlParameter("@TransferOperationType", (object)model.TransferOperationType ?? DBNull.Value),
                    new SqlParameter("@TransferQuantity", model.TransferQuantity ?? (object)DBNull.Value),
                    new SqlParameter("@TransferTanksFrom", (object)model.TransferTanksFrom ?? DBNull.Value),
                    new SqlParameter("@TransferRetainedInTransfer", (object)model.TransferRetainedInTransfer ?? DBNull.Value),
                    new SqlParameter("@TransferTanksTo", (object)model.TransferTanksTo ?? DBNull.Value),
                    new SqlParameter("@TransferRetainedInReceiving", (object)model.TransferRetainedInReceiving ?? DBNull.Value),
    
                    // Incinerator Fields
                    new SqlParameter("@IncineratorOperationType", (object)model.IncineratorOperationType ?? DBNull.Value),
                    new SqlParameter("@IncineratorQuantity", model.IncineratorQuantity ?? (object)DBNull.Value),
                    new SqlParameter("@IncineratorTanksFrom", (object)model.IncineratorTanksFrom ?? DBNull.Value),
                    new SqlParameter("@IncineratorTotalRetainedContent", model.IncineratorTotalRetainedContent ?? (object)DBNull.Value),
                    new SqlParameter("@IncineratorStartTime", (object)model.IncineratorStartTime ?? DBNull.Value),
                    new SqlParameter("@IncineratorStopTime", (object)model.IncineratorStopTime ?? DBNull.Value),
                    new SqlParameter("@IncineratorTotalOperationTime", model.IncineratorTotalOperationTime ?? (object)DBNull.Value),
    
                    // Disposal Ship Fields
                    new SqlParameter("@DisposalShipQuantity", model.DisposalShipQuantity ?? (object)DBNull.Value),
                    new SqlParameter("@DisposalShipTanksFrom", (object)model.DisposalShipTanksFrom ?? DBNull.Value),
                    new SqlParameter("@DisposalShipRetainedIn", (object)model.DisposalShipRetainedIn ?? DBNull.Value),
                    new SqlParameter("@DisposalShipTanksTo", (object)model.DisposalShipTanksTo ?? DBNull.Value),
                    new SqlParameter("@DisposalShipRetainedTo", (object)model.DisposalShipRetainedTo ?? DBNull.Value),
    
                    // Disposal Shore Fields
                    new SqlParameter("@DisposalShoreQuantity", model.DisposalShoreQuantity ?? (object)DBNull.Value),
                    new SqlParameter("@DisposalShoreTanksFrom", (object)model.DisposalShoreTanksFrom ?? DBNull.Value),
                    new SqlParameter("@DisposalShoreRetainedInDischargeTanks", (object)model.DisposalShoreRetainedInDischargeTanks ?? DBNull.Value),
                    new SqlParameter("@DisposalShoreBargeName", (object)model.DisposalShoreBargeName ?? DBNull.Value),
                    new SqlParameter("@DisposalShoreReceptionFacility", (object)model.DisposalShoreReceptionFacility ?? DBNull.Value),
                    new SqlParameter("@DisposalShoreReceiptNo", (object)model.DisposalShoreReceiptNo ?? DBNull.Value),
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
        [Route("ORB1/CodeC/Download")]
        public async Task<IActionResult> Download(string format)
        {
            try
            {
                // Fetch all data without filters or pagination
                var data = await GetCodeCForExport("", 1, 0); // Assuming 0 returns all records

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

        private async Task<IActionResult> ExportToExcel(IEnumerable<CodeCViewModel> data)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Code C Records");

            // Define headers
            var headers = new List<string>
            {
                "Entered By",
                "Entry Date",
                "Collection Type",
                "Weekly Identity Of Tanks",
                "Weekly Capacity Of Tanks",
                "Weekly Total Quantity Of Retention",
                "Collection Identity Of Tanks",
                "Collection Capacity Of Tanks",
                "Collection Total Quantity Of Retention",
                "Collection Manual Residue Quantity",
                "Collection Collected From Tank",
                "Transfer Operation Type",
                "Transfer Quantity",
                "Transfer Tanks From",
                "Transfer Retained In Transfer",
                "Transfer Tanks To",
                "Transfer Retained In Receiving",
                "Incinerator Operation Type",
                "Incinerator Quantity",
                "Incinerator Tanks From",
                "Incinerator Total Retained Content",
                "Incinerator Start Time",
                "Incinerator Stop Time",
                "Incinerator Total Operation Time",
                "Disposal Ship Quantity",
                "Disposal Ship Tanks From",
                "Disposal Ship Retained In",
                "Disposal Ship Tanks To",
                "Disposal Ship Retained To",
                "Disposal Shore Quantity",
                "Disposal Shore Tanks From",
                "Disposal Shore Retained In Discharge Tanks",
                "Disposal Shore Barge Name",
                "Disposal Shore Reception Facility",
                "Disposal Shore Receipt No",
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
                worksheet.Cell(row, 3).Value = item.CollectionType ?? string.Empty;
                worksheet.Cell(row, 4).Value = item.WeeklyIdentityOfTanks ?? string.Empty;
                worksheet.Cell(row, 5).Value = item.WeeklyCapacityOfTanks?.ToString() ?? string.Empty;
                worksheet.Cell(row, 6).Value = item.WeeklyTotalQuantityOfRetention?.ToString() ?? string.Empty;
                worksheet.Cell(row, 7).Value = item.CollectionIdentityOfTanks ?? string.Empty;
                worksheet.Cell(row, 8).Value = item.CollectionCapacityOfTanks?.ToString() ?? string.Empty;
                worksheet.Cell(row, 9).Value = item.CollectionTotalQuantityOfRetention?.ToString() ?? string.Empty;
                worksheet.Cell(row, 10).Value = item.CollectionManualResidueQuantity?.ToString() ?? string.Empty;
                worksheet.Cell(row, 11).Value = item.CollectionCollectedFromTank ?? string.Empty;
                worksheet.Cell(row, 12).Value = item.TransferOperationType ?? string.Empty;
                worksheet.Cell(row, 13).Value = item.TransferQuantity?.ToString() ?? string.Empty;
                worksheet.Cell(row, 14).Value = item.TransferTanksFrom ?? string.Empty;
                worksheet.Cell(row, 15).Value = item.TransferRetainedInTransfer ?? string.Empty;
                worksheet.Cell(row, 16).Value = item.TransferTanksTo ?? string.Empty;
                worksheet.Cell(row, 17).Value = item.TransferRetainedInReceiving ?? string.Empty;
                worksheet.Cell(row, 18).Value = item.IncineratorOperationType ?? string.Empty;
                worksheet.Cell(row, 19).Value = item.IncineratorQuantity?.ToString() ?? string.Empty;
                worksheet.Cell(row, 20).Value = item.IncineratorTanksFrom ?? string.Empty;
                worksheet.Cell(row, 21).Value = item.IncineratorTotalRetainedContent?.ToString() ?? string.Empty;
                worksheet.Cell(row, 22).Value = item.IncineratorStartTime?.ToString(@"hh\:mm") ?? string.Empty;
                worksheet.Cell(row, 23).Value = item.IncineratorStopTime?.ToString(@"hh\:mm") ?? string.Empty;
                worksheet.Cell(row, 24).Value = item.IncineratorTotalOperationTime?.ToString() ?? string.Empty;
                worksheet.Cell(row, 25).Value = item.DisposalShipQuantity?.ToString() ?? string.Empty;
                worksheet.Cell(row, 26).Value = item.DisposalShipTanksFrom ?? string.Empty;
                worksheet.Cell(row, 27).Value = item.DisposalShipRetainedIn ?? string.Empty;
                worksheet.Cell(row, 28).Value = item.DisposalShipTanksTo ?? string.Empty;
                worksheet.Cell(row, 29).Value = item.DisposalShipRetainedTo ?? string.Empty;
                worksheet.Cell(row, 30).Value = item.DisposalShoreQuantity?.ToString() ?? string.Empty;
                worksheet.Cell(row, 31).Value = item.DisposalShoreTanksFrom ?? string.Empty;
                worksheet.Cell(row, 32).Value = item.DisposalShoreRetainedInDischargeTanks ?? string.Empty;
                worksheet.Cell(row, 33).Value = item.DisposalShoreBargeName ?? string.Empty;
                worksheet.Cell(row, 34).Value = item.DisposalShoreReceptionFacility ?? string.Empty;
                worksheet.Cell(row, 35).Value = item.DisposalShoreReceiptNo ?? string.Empty;
                worksheet.Cell(row, 36).Value = item.StatusName ?? string.Empty;
                worksheet.Cell(row, 37).Value = item.ApprovedBy ?? string.Empty;
                worksheet.Cell(row, 38).Value = item.Comments ?? string.Empty;
                row++;
            }


            worksheet.Columns().AdjustToContents(); // Auto-fit columns

            // Save to memory stream
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            // Set the correct headers to force download
            var fileName = $"CodeC_Records_{DateTime.Now:yyyyMMdd}.xlsx";
            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName
            );
        }

        private FileContentResult ExportToPdf(IEnumerable<CodeCViewModel> data)
        {
            byte[] pdfBytes;

            using (var stream = new MemoryStream())
            {
                var document = new Document(PageSize.A1.Rotate(), 20f, 20f, 20f, 20f);
                PdfWriter.GetInstance(document, stream);

                document.Open();

                // Title
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                var title = new Paragraph("Code C Records", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER
                };
                document.Add(title);
                document.Add(new Paragraph(" "));

                // Table setup
                var table = new PdfPTable(38)
                {
                    WidthPercentage = 100
                };
                float[] columnWidths = Enumerable.Repeat(4f, 38).ToArray();
                table.SetWidths(columnWidths);

                var headers = new[]
                {
                "Entered By",
                "Entry Date",
                "Collection Type",
                "Weekly Identity Of Tanks",
                "Weekly Capacity Of Tanks",
                "Weekly Total Quantity Of Retention",
                "Collection Identity Of Tanks",
                "Collection Capacity Of Tanks",
                "Collection Total Quantity Of Retention",
                "Collection Manual Residue Quantity",
                "Collection Collected From Tank",
                "Transfer Operation Type",
                "Transfer Quantity",
                "Transfer Tanks From",
                "Transfer Retained In Transfer",
                "Transfer Tanks To",
                "Transfer Retained In Receiving",
                "Incinerator Operation Type",
                "Incinerator Quantity",
                "Incinerator Tanks From",
                "Incinerator Total Retained Content",
                "Incinerator Start Time",
                "Incinerator Stop Time",
                "Incinerator Total Operation Time",
                "Disposal Ship Quantity",
                "Disposal Ship Tanks From",
                "Disposal Ship Retained In",
                "Disposal Ship Tanks To",
                "Disposal Ship Retained To",
                "Disposal Shore Quantity",
                "Disposal Shore Tanks From",
                "Disposal Shore Retained In Discharge Tanks",
                "Disposal Shore Barge Name",
                "Disposal Shore Reception Facility",
                "Disposal Shore Receipt No",
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
                    AddCell(table, item.CollectionType ?? "", dataFont);
                    AddCell(table, item.WeeklyIdentityOfTanks ?? "", dataFont);
                    AddCell(table, item.WeeklyCapacityOfTanks?.ToString() ?? "", dataFont);
                    AddCell(table, item.WeeklyTotalQuantityOfRetention?.ToString() ?? "", dataFont);
                    AddCell(table, item.CollectionIdentityOfTanks ?? "", dataFont);
                    AddCell(table, item.CollectionCapacityOfTanks?.ToString() ?? "", dataFont);
                    AddCell(table, item.CollectionTotalQuantityOfRetention?.ToString() ?? "", dataFont);
                    AddCell(table, item.CollectionManualResidueQuantity?.ToString() ?? "", dataFont);
                    AddCell(table, item.CollectionCollectedFromTank ?? "", dataFont);
                    AddCell(table, item.TransferOperationType ?? "", dataFont);
                    AddCell(table, item.TransferQuantity?.ToString() ?? "", dataFont);
                    AddCell(table, item.TransferTanksFrom ?? "", dataFont);
                    AddCell(table, item.TransferRetainedInTransfer ?? "", dataFont);
                    AddCell(table, item.TransferTanksTo ?? "", dataFont);
                    AddCell(table, item.TransferRetainedInReceiving ?? "", dataFont);
                    AddCell(table, item.IncineratorOperationType ?? "", dataFont);
                    AddCell(table, item.IncineratorQuantity?.ToString() ?? "", dataFont);
                    AddCell(table, item.IncineratorTanksFrom ?? "", dataFont);
                    AddCell(table, item.IncineratorTotalRetainedContent?.ToString() ?? "", dataFont);
                    AddCell(table, item.IncineratorStartTime?.ToString(@"hh\:mm") ?? "", dataFont);
                    AddCell(table, item.IncineratorStopTime?.ToString(@"hh\:mm") ?? "", dataFont);
                    AddCell(table, item.IncineratorTotalOperationTime?.ToString() ?? "", dataFont);
                    AddCell(table, item.DisposalShipQuantity?.ToString() ?? "", dataFont);
                    AddCell(table, item.DisposalShipTanksFrom ?? "", dataFont);
                    AddCell(table, item.DisposalShipRetainedIn ?? "", dataFont);
                    AddCell(table, item.DisposalShipTanksTo ?? "", dataFont);
                    AddCell(table, item.DisposalShipRetainedTo ?? "", dataFont);
                    AddCell(table, item.DisposalShoreQuantity?.ToString() ?? "", dataFont);
                    AddCell(table, item.DisposalShoreTanksFrom ?? "", dataFont);
                    AddCell(table, item.DisposalShoreRetainedInDischargeTanks ?? "", dataFont);
                    AddCell(table, item.DisposalShoreBargeName ?? "", dataFont);
                    AddCell(table, item.DisposalShoreReceptionFacility ?? "", dataFont);
                    AddCell(table, item.DisposalShoreReceiptNo ?? "", dataFont);
                    AddCell(table, item.StatusName ?? "", dataFont);
                    AddCell(table, item.ApprovedBy ?? "", dataFont);
                    AddCell(table, item.Comments ?? "", dataFont);
                }

                document.Add(table);
                document.Close();

                // ✅ Copy to byte array BEFORE stream is disposed
                pdfBytes = stream.ToArray();
            }

            return File(pdfBytes, "application/pdf", $"CodeC_Records_{DateTime.Now:yyyyMMdd}.pdf");
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

        public async Task<List<CodeCViewModel>> GetCodeCForExport(string searchTerm = "", int pageNumber = 1, int pageSize = 10)
        {
            List<CodeCViewModel> records = new List<CodeCViewModel>();

            using (SqlConnection connection = _db.CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("proc_GetORB1_CodeC", connection))
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
                            records.Add(new CodeCViewModel
                            {
                                Id = reader.GetInt32(0),
                                UserId = reader.IsDBNull(1) ? null : reader.GetString(1),
                                EntryDate = reader.GetDateTime(2),
                                CollectionType = reader.IsDBNull(3) ? null : reader.GetString(3),
                                WeeklyIdentityOfTanks = reader.IsDBNull(4) ? null : reader.GetString(4),
                                WeeklyCapacityOfTanks = reader.IsDBNull(5) ? (decimal?)null : reader.GetDecimal(5),
                                WeeklyTotalQuantityOfRetention = reader.IsDBNull(6) ? (decimal?)null : reader.GetDecimal(6),
                                CollectionIdentityOfTanks = reader.IsDBNull(7) ? null : reader.GetString(7),
                                CollectionCapacityOfTanks = reader.IsDBNull(8) ? (decimal?)null : reader.GetDecimal(8),
                                CollectionTotalQuantityOfRetention = reader.IsDBNull(9) ? (decimal?)null : reader.GetDecimal(9),
                                CollectionManualResidueQuantity = reader.IsDBNull(10) ? (decimal?)null : reader.GetDecimal(10),
                                CollectionCollectedFromTank = reader.IsDBNull(11) ? null : reader.GetString(11),
                                TransferOperationType = reader.IsDBNull(12) ? null : reader.GetString(12),
                                TransferQuantity = reader.IsDBNull(13) ? (decimal?)null : reader.GetDecimal(13),
                                TransferTanksFrom = reader.IsDBNull(14) ? null : reader.GetString(14),
                                TransferRetainedInTransfer = reader.IsDBNull(15) ? null : reader.GetString(15),
                                TransferTanksTo = reader.IsDBNull(16) ? null : reader.GetString(16),
                                TransferRetainedInReceiving = reader.IsDBNull(17) ? null : reader.GetString(17),
                                IncineratorOperationType = reader.IsDBNull(18) ? null : reader.GetString(18),
                                IncineratorQuantity = reader.IsDBNull(19) ? (decimal?)null : reader.GetDecimal(19),
                                IncineratorTanksFrom = reader.IsDBNull(20) ? null : reader.GetString(20),
                                IncineratorTotalRetainedContent = reader.IsDBNull(21) ? (decimal?)null : reader.GetDecimal(21),
                                IncineratorStartTime = reader.IsDBNull(22) ? (TimeSpan?)null : reader.GetTimeSpan(22),
                                IncineratorStopTime = reader.IsDBNull(23) ? (TimeSpan?)null : reader.GetTimeSpan(23),
                                IncineratorTotalOperationTime = reader.IsDBNull(24) ? (decimal?)null : reader.GetDecimal(24),
                                DisposalShipQuantity = reader.IsDBNull(25) ? (decimal?)null : reader.GetDecimal(25),
                                DisposalShipTanksFrom = reader.IsDBNull(26) ? null : reader.GetString(26),
                                DisposalShipRetainedIn = reader.IsDBNull(27) ? null : reader.GetString(27),
                                DisposalShipTanksTo = reader.IsDBNull(28) ? null : reader.GetString(28),
                                DisposalShipRetainedTo = reader.IsDBNull(29) ? null : reader.GetString(29),
                                DisposalShoreQuantity = reader.IsDBNull(30) ? (decimal?)null : reader.GetDecimal(30),
                                DisposalShoreTanksFrom = reader.IsDBNull(31) ? null : reader.GetString(31),
                                DisposalShoreRetainedInDischargeTanks = reader.IsDBNull(32) ? null : reader.GetString(32),
                                DisposalShoreBargeName = reader.IsDBNull(33) ? null : reader.GetString(33),
                                DisposalShoreReceptionFacility = reader.IsDBNull(34) ? null : reader.GetString(34),
                                DisposalShoreReceiptNo = reader.IsDBNull(35) ? null : reader.GetString(35),
                                StatusName = reader.IsDBNull(36) ? null : reader.GetString(36),
                                ApprovedBy = reader.IsDBNull(37) ? null : reader.GetString(37),
                                Comments = reader.IsDBNull(38) ? null : reader.GetString(38)
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