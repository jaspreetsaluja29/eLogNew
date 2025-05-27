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
        public IActionResult DataEntry_CodeG(string userId, string userName, string userRoleName, string jobRank)
        {
            ViewBag.UserID = userId;
            ViewBag.UserName = userName;
            ViewBag.UserRoleName = userRoleName;
            ViewBag.JobRank = jobRank;
            return View("~/Views/ORB1/DataEntry_CodeG.cshtml", new CodeGModel());
        }

        // Edit data entry page (fetch record by Id)
        public async Task<IActionResult> Edit(int id, string userId, string userName, string userRoleName, string jobRank)
        {
            ViewBag.UserID = userId;
            ViewBag.UserName = userName;
            ViewBag.UserRoleName = userRoleName;
            ViewBag.JobRank = jobRank;

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

        [HttpGet]
        [Route("ORB1/CodeG/Download")]
        public async Task<IActionResult> Download(string format)
        {
            try
            {
                // Fetch all data without filters or pagination
                var data = await GetCodeGForExport("", 1, 0); // Assuming 0 returns all records

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

        private async Task<IActionResult> ExportToExcel(IEnumerable<CodeGViewModel> data)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Code A Records");

            // Define headers
            var headers = new List<string>
            {
                "Entered By",         
                "Entry Date",         
                "Time of Occurrence", 
                "Position of Ship",   
                "Approx Quantity",    
                "Type of Oil",        
                "Reasons",            
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
                worksheet.Cell(row, 3).Value = item.TimeofOccurrence?.ToString(@"hh\:mm") ?? string.Empty;
                worksheet.Cell(row, 4).Value = item.PositionofShip ?? string.Empty;
                worksheet.Cell(row, 5).Value = item.ApproxQuantity?.ToString() ?? string.Empty;
                worksheet.Cell(row, 6).Value = item.TypeofOil ?? string.Empty;
                worksheet.Cell(row, 7).Value = item.Reasons ?? string.Empty;
                worksheet.Cell(row, 8).Value = item.StatusName ?? string.Empty;
                worksheet.Cell(row, 9).Value = item.ApprovedBy ?? string.Empty;
                worksheet.Cell(row, 10).Value = item.Comments ?? string.Empty;
                row++;
            }

            worksheet.Columns().AdjustToContents(); // Auto-fit columns

            // Save to memory stream
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            // Set the correct headers to force download
            var fileName = $"CodeG_Records_{DateTime.Now:yyyyMMdd}.xlsx";
            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName
            );
        }

        private FileContentResult ExportToPdf(IEnumerable<CodeGViewModel> data)
        {
            byte[] pdfBytes;

            using (var stream = new MemoryStream())
            {
                var document = new Document(PageSize.A2.Rotate(), 20f, 20f, 20f, 20f);
                PdfWriter.GetInstance(document, stream);

                document.Open();

                // Title
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                var title = new Paragraph("Code G Records", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER
                };
                document.Add(title);
                document.Add(new Paragraph(" "));

                // Table setup
                var table = new PdfPTable(10)
                {
                    WidthPercentage = 100
                };
                float[] columnWidths = Enumerable.Repeat(4f, 10).ToArray();
                table.SetWidths(columnWidths);

                var headers = new[]
                {
                    "Entered By",
                    "Entry Date",
                    "Time of Occurrence",
                    "Position of Ship",
                    "Approx Quantity",
                    "Type of Oil",
                    "Reasons",
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
                    AddCell(table, item.TimeofOccurrence?.ToString(@"hh\:mm") ?? "", dataFont);
                    AddCell(table, item.PositionofShip ?? "", dataFont);
                    AddCell(table, item.ApproxQuantity?.ToString() ?? "", dataFont);
                    AddCell(table, item.TypeofOil ?? "", dataFont);
                    AddCell(table, item.Reasons ?? "", dataFont);
                    AddCell(table, item.StatusName ?? "", dataFont);
                    AddCell(table, item.ApprovedBy ?? "", dataFont);
                    AddCell(table, item.Comments ?? "", dataFont);
                }

                document.Add(table);
                document.Close();

                // ✅ Copy to byte array BEFORE stream is disposed
                pdfBytes = stream.ToArray();
            }

            return File(pdfBytes, "application/pdf", $"CodeG_Records_{DateTime.Now:yyyyMMdd}.pdf");
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

        public async Task<List<CodeGViewModel>> GetCodeGForExport(string searchTerm = "", int pageNumber = 1, int pageSize = 10)
        {
            List<CodeGViewModel> records = new List<CodeGViewModel>();

            using (SqlConnection connection = _db.CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("proc_GetORB1_CodeG", connection))
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

            // Apply pagination if needed - for exports, you might want all data
            if (pageSize > 0)
            {
                return records.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            }
            return records;
        }
    }
}