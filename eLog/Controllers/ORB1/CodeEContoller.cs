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

        [HttpGet]
        [Route("ORB1/CodeE/Download")]
        public async Task<IActionResult> Download(string format)
        {
            try
            {
                // Fetch all data without filters or pagination
                var data = await GetCodeEForExport("", 1, 0); // Assuming 0 returns all records

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

        private async Task<IActionResult> ExportToExcel(IEnumerable<CodeEViewModel> data)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Code E Records");

            // Define headers
            var headers = new List<string>
            {
                "Entered By",               
                "Entry Date",               
                "Automatic Discharge Type", 
                "Overboard Position Start", 
                "Overboard Time Switching", 
                "Transfer Time Switching",  
                "Transfer Tank From",       
                "Transfer Tank To",         
                "Time Back To Manual",      
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
                worksheet.Cell(row, 3).Value = item.AutomaticDischargeType ?? string.Empty;
                worksheet.Cell(row, 4).Value = item.OverboardPositionShipStart ?? string.Empty;
                worksheet.Cell(row, 5).Value = item.OverboardTimeSwitching?.ToString(@"hh\:mm") ?? string.Empty;
                worksheet.Cell(row, 6).Value = item.TransferTimeSwitching?.ToString(@"hh\:mm") ?? string.Empty;
                worksheet.Cell(row, 7).Value = item.TransferTankfrom ?? string.Empty;
                worksheet.Cell(row, 8).Value = item.TransferTankTo ?? string.Empty;
                worksheet.Cell(row, 9).Value = item.TimeBackToManual?.ToString(@"hh\:mm") ?? string.Empty;
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
            var fileName = $"CodeE_Records_{DateTime.Now:yyyyMMdd}.xlsx";
            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName
            );
        }

        private FileContentResult ExportToPdf(IEnumerable<CodeEViewModel> data)
        {
            byte[] pdfBytes;

            using (var stream = new MemoryStream())
            {
                var document = new Document(PageSize.A2.Rotate(), 20f, 20f, 20f, 20f);
                PdfWriter.GetInstance(document, stream);

                document.Open();

                // Title
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                var title = new Paragraph("Code E Records", titleFont)
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
                    "Automatic Discharge Type",
                    "Overboard Position Start",
                    "Overboard Time Switching",
                    "Transfer Time Switching",
                    "Transfer Tank From",
                    "Transfer Tank To",
                    "Time Back To Manual",
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
                    AddCell(table, item.AutomaticDischargeType ?? "", dataFont);
                    AddCell(table, item.OverboardPositionShipStart ?? "", dataFont);
                    AddCell(table, item.OverboardTimeSwitching?.ToString(@"hh\:mm") ?? "", dataFont);
                    AddCell(table, item.TransferTimeSwitching?.ToString(@"hh\:mm") ?? "", dataFont);
                    AddCell(table, item.TransferTankfrom ?? "", dataFont);
                    AddCell(table, item.TransferTankTo ?? "", dataFont);
                    AddCell(table, item.TimeBackToManual?.ToString(@"hh\:mm") ?? "", dataFont);
                    AddCell(table, item.StatusName ?? "", dataFont);
                    AddCell(table, item.ApprovedBy ?? "", dataFont);
                    AddCell(table, item.Comments ?? "", dataFont);
                }

                document.Add(table);
                document.Close();

                // ✅ Copy to byte array BEFORE stream is disposed
                pdfBytes = stream.ToArray();
            }

            return File(pdfBytes, "application/pdf", $"CodeE_Records_{DateTime.Now:yyyyMMdd}.pdf");
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

        public async Task<List<CodeEViewModel>> GetCodeEForExport(string searchTerm = "", int pageNumber = 1, int pageSize = 10)
        {
            List<CodeEViewModel> records = new List<CodeEViewModel>();

            using (SqlConnection connection = _db.CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("proc_GetORB1_CodeE", connection))
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

            // Apply pagination if needed - for exports, you might want all data
            if (pageSize > 0)
            {
                return records.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            }
            return records;
        }
    }
}