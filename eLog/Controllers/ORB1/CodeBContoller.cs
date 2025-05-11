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
    public class CodeBController : Controller
    {
        private readonly DatabaseHelper _db;
        private readonly IConfiguration _configuration;

        public CodeBController(DatabaseHelper db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        // Fetch data using stored procedure        
        [Route("ORB1/CodeB/GetCodeBData")]
        [HttpGet]
        public async Task<IActionResult> GetCodeBData(int pageNumber = 1, int pageSize = 10)
        {
            List<CodeBViewModel> records = new List<CodeBViewModel>();
            int totalRecords = 0;
            using (SqlConnection connection = _db.CreateConnection()) // Ensure this method exists in DatabaseHelper
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("proc_GetORB1_CodeB", connection)) // Using the correct stored procedure name
                {
                    command.CommandType = CommandType.StoredProcedure;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            records.Add(new CodeBViewModel
                            {
                                Id = reader.GetInt32(0),
                                UserId = reader.IsDBNull(1) ? null : reader.GetString(1),
                                EntryDate = reader.GetDateTime(2),
                                IdentityOfTanks = reader.IsDBNull(3) ? null : reader.GetString(3),
                                PositionOfShipStart = reader.IsDBNull(4) ? null : reader.GetString(4),
                                PositionOfShipCompletion = reader.IsDBNull(5) ? null : reader.GetString(5),
                                ShipSpeedDischarge = reader.IsDBNull(6) ? null : reader.GetString(6),
                                MethodOfDischarge = reader.IsDBNull(7) ? null : reader.GetString(7),
                                ReceiptNo = reader.IsDBNull(8) ? null : reader.GetString(8),
                                QuantityDischarged = reader.IsDBNull(9) ? (decimal?)null : reader.GetDecimal(9),
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
            return View("~/Views/ORB1/CodeB.cshtml", paginatedRecords);
        }

        // Data Entry Page
        public IActionResult DataEntry_CodeB(string userId, string userName, string userRoleName)
        {
            ViewBag.UserID = userId;
            ViewBag.UserName = userName;
            ViewBag.UserRoleName = userRoleName;
            return View("~/Views/ORB1/DataEntry_CodeB.cshtml", new CodeBModel());
        }

        // Edit data entry page (fetch record by Id)
        public async Task<IActionResult> Edit(int id, string userId, string userName, string userRoleName)
        {
            // Store user details in ViewBag (so they can be used in the view)
            ViewBag.UserID = userId;
            ViewBag.UserName = userName;
            ViewBag.UserRoleName = userRoleName;
            CodeBViewModel recordToEdit = null;

            try
            {
                using (SqlConnection connection = _db.CreateConnection())
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("proc_GetCodeBById", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync()) // Ensure data exists before reading
                            {
                                recordToEdit = new CodeBViewModel
                                {
                                    Id = reader.GetInt32(0),
                                    UserId = reader.IsDBNull(1) ? null : reader.GetString(1),
                                    EntryDate = reader.GetDateTime(2),
                                    IdentityOfTanks = reader.IsDBNull(3) ? null : reader.GetString(3),
                                    PositionOfShipStart = reader.IsDBNull(4) ? null : reader.GetString(4),
                                    PositionOfShipCompletion = reader.IsDBNull(5) ? null : reader.GetString(5),
                                    ShipSpeedDischarge = reader.IsDBNull(6) ? null : reader.GetString(6),
                                    MethodOfDischarge = reader.IsDBNull(7) ? null : reader.GetString(7),
                                    ReceiptNo = reader.IsDBNull(8) ? null : reader.GetString(8),
                                    QuantityDischarged = reader.IsDBNull(9) ? (decimal?)null : reader.GetDecimal(9),
                                    StatusName = reader.IsDBNull(10) ? null : reader.GetString(10),
                                    ApprovedBy = reader.IsDBNull(11) ? null : reader.GetString(11),
                                    Comments = reader.IsDBNull(12) ? null : reader.GetString(12)
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
                    "Level 1- Entry" => View("~/Views/ORB1/DataEdit_CodeB.cshtml", recordToEdit),
                    "Level 2- Approver" => View("~/Views/ORB1/Approver_DataEdit_CodeB.cshtml", recordToEdit),
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

        [Route("ORB1/CodeB/Create")]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CodeBModel model, IFormFile attachment)
        {
            if (model == null)
            {
                return BadRequest("Invalid data received.");
            }
            try
            {

                // Fetch file storage path from appsettings.json
                var uploadPath = _configuration["FileStorage:CodeB_ReceptionAttachment"];
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
                    string fileName = $"{model.ReceiptNo}_{Path.GetFileNameWithoutExtension(attachment.FileName)}_{timestamp}{fileExtension}";
                    filePath = Path.Combine(uploadPath, fileName);

                    // Save the file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await attachment.CopyToAsync(stream);
                    }
                }

                string storedProcedure = "proc_InsertORB1CodeB";
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@UserId", (object)model.UserId ?? DBNull.Value),
                    new SqlParameter("@EntryDate", model.EntryDate),
                    new SqlParameter("@IdentityOfTanks", (object)model.IdentityOfTanks ?? DBNull.Value),
                    new SqlParameter("@PositionOfShipStart", (object)model.PositionOfShipStart ?? DBNull.Value),
                    new SqlParameter("@PositionOfShipCompletion", (object)model.PositionOfShipCompletion ?? DBNull.Value),
                    new SqlParameter("@ShipSpeedDischarge", (object)model.ShipSpeedDischarge ?? DBNull.Value),
                    new SqlParameter("@MethodOfDischarge", (object)model.MethodOfDischarge ?? DBNull.Value),
                    new SqlParameter("@ReceiptNo", (object)model.ReceiptNo ?? DBNull.Value),
                    new SqlParameter("@QuantityDischarged", model.QuantityDischarged ?? (object)DBNull.Value),
                    new SqlParameter("@AttachmentPath", (object)filePath ?? DBNull.Value)
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

        [Route("ORB1/CodeB/Update")]
        [HttpPost]
        public IActionResult Update([FromBody] CodeBViewModel model)
        {
            if (model == null || model.Id <= 0)
            {
                return BadRequest("Invalid data received or ID is missing.");
            }

            try
            {
                string storedProcedure = "proc_UpdateORB1CodeB";
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@Id", model.Id),
                    new SqlParameter("@UserId", (object)model.UserId ?? DBNull.Value),
                    new SqlParameter("@EntryDate", model.EntryDate),
                    new SqlParameter("@IdentityOfTanks", (object)model.IdentityOfTanks ?? DBNull.Value),
                    new SqlParameter("@PositionOfShipStart", (object)model.PositionOfShipStart ?? DBNull.Value),
                    new SqlParameter("@PositionOfShipCompletion", (object)model.PositionOfShipCompletion ?? DBNull.Value),
                    new SqlParameter("@ShipSpeedDischarge", (object)model.ShipSpeedDischarge ?? DBNull.Value),
                    new SqlParameter("@MethodOfDischarge", (object)model.MethodOfDischarge ?? DBNull.Value),
                    new SqlParameter("@ReceiptNo", (object)model.ReceiptNo ?? DBNull.Value),
                    new SqlParameter("@QuantityDischarged", model.QuantityDischarged ?? (object)DBNull.Value)
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
        [Route("ORB1/CodeB/ApproverUpdate")]
        public IActionResult ApproverUpdate([FromBody] CodeBViewModel model)
        {
            if (model == null || model.Id <= 0)
            {
                return BadRequest("Invalid data received or ID is missing.");
            }

            try
            {
                string storedProcedure = "proc_ApproverUpdateORB1CodeB";
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@Id", model.Id),
                    new SqlParameter("@UserId", model.UserId),
                    new SqlParameter("@EntryDate", model.EntryDate),
                    new SqlParameter("@IdentityOfTanks", (object)model.IdentityOfTanks ?? DBNull.Value),
                    new SqlParameter("@PositionOfShipStart", (object)model.PositionOfShipStart ?? DBNull.Value),
                    new SqlParameter("@PositionOfShipCompletion", (object)model.PositionOfShipCompletion ?? DBNull.Value),
                    new SqlParameter("@ShipSpeedDischarge", (object)model.ShipSpeedDischarge ?? DBNull.Value),
                    new SqlParameter("@MethodOfDischarge", (object)model.MethodOfDischarge ?? DBNull.Value),
                    new SqlParameter("@ReceiptNo", (object)model.ReceiptNo ?? DBNull.Value),
                    new SqlParameter("@QuantityDischarged", model.QuantityDischarged ?? (object)DBNull.Value),
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
        [Route("ORB1/CodeB/Download")]
        public async Task<IActionResult> Download(string format)
        {
            try
            {
                // Fetch all data without filters or pagination
                var data = await GetCodeBForExport("", 1, 0); // Assuming 0 returns all records

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

        private async Task<IActionResult> ExportToExcel(IEnumerable<CodeBViewModel> data)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Code B Records");

            // Define headers
            var headers = new List<string>
            {
                "Entered By",
                "Entry Date",
                "Identity Of Tanks",
                "Position Of Ship Start",
                "Position Of Ship Completion",
                "Ship Speed Discharge",
                "Method Of Discharge",
                "Receipt No",
                "Quantity Discharged",
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
                worksheet.Cell(row, 3).Value = item.IdentityOfTanks ?? string.Empty;
                worksheet.Cell(row, 4).Value = item.PositionOfShipStart ?? string.Empty;
                worksheet.Cell(row, 5).Value = item.PositionOfShipCompletion ?? string.Empty;
                worksheet.Cell(row, 6).Value = item.ShipSpeedDischarge ?? string.Empty;
                worksheet.Cell(row, 7).Value = item.MethodOfDischarge ?? string.Empty;
                worksheet.Cell(row, 8).Value = item.ReceiptNo ?? string.Empty;
                worksheet.Cell(row, 9).Value = item.QuantityDischarged?.ToString() ?? string.Empty;
                worksheet.Cell(row, 10).Value = item.StatusName ?? string.Empty;
                worksheet.Cell(row, 11).Value = item.ApprovedBy ?? string.Empty;
                worksheet.Cell(row, 12).Value = item.Comments ?? string.Empty;
                row++;
            }

            worksheet.Columns().AdjustToContents(); // Auto-fit columns

            // Save to memory stream
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            // Set the correct headers to force download
            var fileName = $"CodeB_Records_{DateTime.Now:yyyyMMdd}.xlsx";
            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName
            );
        }

        private FileContentResult ExportToPdf(IEnumerable<CodeBViewModel> data)
        {
            byte[] pdfBytes;

            using (var stream = new MemoryStream())
            {
                var document = new Document(PageSize.A2.Rotate(), 20f, 20f, 20f, 20f);
                PdfWriter.GetInstance(document, stream);

                document.Open();

                // Title
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                var title = new Paragraph("Code B Records", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER
                };
                document.Add(title);
                document.Add(new Paragraph(" "));

                // Table setup
                var table = new PdfPTable(12)
                {
                    WidthPercentage = 100
                };
                float[] columnWidths = Enumerable.Repeat(4f, 12).ToArray();
                table.SetWidths(columnWidths);

                var headers = new[]
                {
                "Entered By",
                "Entry Date",
                "Identity Of Tanks",
                "Position Of Ship Start",
                "Position Of Ship Completion",
                "Ship Speed Discharge",
                "Method Of Discharge",
                "Receipt No",
                "Quantity Discharged",
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
                    AddCell(table, item.IdentityOfTanks ?? "", dataFont);
                    AddCell(table, item.PositionOfShipStart ?? "", dataFont);
                    AddCell(table, item.PositionOfShipCompletion ?? "", dataFont);
                    AddCell(table, item.ShipSpeedDischarge ?? "", dataFont);
                    AddCell(table, item.MethodOfDischarge ?? "", dataFont);
                    AddCell(table, item.ReceiptNo ?? "", dataFont);
                    AddCell(table, item.QuantityDischarged?.ToString() ?? "", dataFont);
                    AddCell(table, item.StatusName ?? "", dataFont);
                    AddCell(table, item.ApprovedBy ?? "", dataFont);
                    AddCell(table, item.Comments ?? "", dataFont);
                }

                document.Add(table);
                document.Close();

                // ✅ Copy to byte array BEFORE stream is disposed
                pdfBytes = stream.ToArray();
            }

            return File(pdfBytes, "application/pdf", $"CodeB_Records_{DateTime.Now:yyyyMMdd}.pdf");
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

        public async Task<List<CodeBViewModel>> GetCodeBForExport(string searchTerm = "", int pageNumber = 1, int pageSize = 10)
        {
            List<CodeBViewModel> records = new List<CodeBViewModel>();

            using (SqlConnection connection = _db.CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("proc_GetORB1_CodeB", connection))
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
                            records.Add(new CodeBViewModel
                            {
                                Id = reader.GetInt32(0),
                                UserId = reader.IsDBNull(1) ? null : reader.GetString(1),
                                EntryDate = reader.GetDateTime(2),
                                IdentityOfTanks = reader.IsDBNull(3) ? null : reader.GetString(3),
                                PositionOfShipStart = reader.IsDBNull(4) ? null : reader.GetString(4),
                                PositionOfShipCompletion = reader.IsDBNull(5) ? null : reader.GetString(5),
                                ShipSpeedDischarge = reader.IsDBNull(6) ? null : reader.GetString(6),
                                MethodOfDischarge = reader.IsDBNull(7) ? null : reader.GetString(7),
                                ReceiptNo = reader.IsDBNull(8) ? null : reader.GetString(8),
                                QuantityDischarged = reader.IsDBNull(9) ? (decimal?)null : reader.GetDecimal(9),
                                StatusName = reader.IsDBNull(10) ? null : reader.GetString(10),
                                ApprovedBy = reader.IsDBNull(11) ? null : reader.GetString(11),
                                Comments = reader.IsDBNull(12) ? null : reader.GetString(12)
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