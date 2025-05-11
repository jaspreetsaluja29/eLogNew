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
                                SelectType = reader.IsDBNull(3) ? null : reader.GetString(3),
                                Port = reader.IsDBNull(4) ? null : reader.GetString(4),
                                Location = reader.IsDBNull(5) ? null : reader.GetString(5),
                                StartDateTime = reader.GetDateTime(6),
                                StopDateTime = reader.GetDateTime(7),
                                Quantity = reader.GetDecimal(8),
                                Grade = reader.IsDBNull(9) ? null : reader.GetString(9),
                                SulphurContent = reader.IsDBNull(10) ? null : reader.GetString(10),
                                TankLoaded1 = reader.IsDBNull(11) ? null : reader.GetString(11),
                                TankRetained1 = reader.IsDBNull(12) ? null : reader.GetString(12),
                                TankLoaded2 = reader.IsDBNull(13) ? null : reader.GetString(13),
                                TankRetained2 = reader.IsDBNull(14) ? null : reader.GetString(14),
                                TankLoaded3 = reader.IsDBNull(15) ? null : reader.GetString(15),
                                TankRetained3 = reader.IsDBNull(16) ? null : reader.GetString(16),
                                TankLoaded4 = reader.IsDBNull(17) ? null : reader.GetString(17),
                                TankRetained4 = reader.IsDBNull(18) ? null : reader.GetString(18),
                                StatusName = reader.IsDBNull(19) ? null : reader.GetString(19),
                                ApprovedBy = reader.IsDBNull(20) ? null : reader.GetString(20),
                                Comments = reader.IsDBNull(21) ? null : reader.GetString(21)
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
                                SelectType = reader.IsDBNull(3) ? null : reader.GetValue(3).ToString(),
                                Port = reader.IsDBNull(4) ? null : reader.GetString(4).ToString(),
                                Location = reader.IsDBNull(5) ? null : reader.GetString(5).ToString(),
                                StartDateTime = reader.GetDateTime(6),
                                StopDateTime = reader.GetDateTime(7),
                                Quantity = reader.GetDecimal(8),
                                Grade = reader.IsDBNull(9) ? null : reader.GetString(9).ToString(),
                                SulphurContent = reader.IsDBNull(10) ? null : reader.GetString(10).ToString(),
                                TankLoaded1 = reader.IsDBNull(11) ? null : reader.GetString(11).ToString(),
                                TankRetained1 = reader.IsDBNull(12) ? null : reader.GetString(12).ToString(),
                                TankLoaded2 = reader.IsDBNull(13) ? null : reader.GetString(13).ToString(),
                                TankRetained2 = reader.IsDBNull(14) ? null : reader.GetString(14).ToString(),
                                TankLoaded3 = reader.IsDBNull(15) ? null : reader.GetString(15).ToString(),
                                TankRetained3 = reader.IsDBNull(16) ? null : reader.GetString(16).ToString(),
                                TankLoaded4 = reader.IsDBNull(17) ? null : reader.GetString(17).ToString(),
                                TankRetained4 = reader.IsDBNull(18) ? null : reader.GetString(18).ToString(),
                                StatusName = reader.IsDBNull(19) ? null : reader.GetValue(19).ToString(),
                                ApprovedBy = reader.IsDBNull(20) ? null : reader.GetValue(20).ToString(),
                                Comments = reader.IsDBNull(21) ? null : reader.GetValue(21).ToString()
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
            new SqlParameter("@SelectType", model.SelectType ?? (object)DBNull.Value),
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
                    new SqlParameter("@SelectType", model.SelectType ?? (object)DBNull.Value),
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
                    new SqlParameter("@SelectType", model.SelectType ?? (object)DBNull.Value),
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
                    using (SqlCommand cmd = new SqlCommand("proc_GetORB1_FirstPage_BunkerTanks", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        connection.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tanks.Add(new
                                {
                                    TankIdentification = reader["TankIdentification"].ToString()
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
        [Route("ORB1/CodeH/Download")]
        public async Task<IActionResult> Download(string format)
        {
            try
            {
                // Fetch all data without filters or pagination
                var data = await GetCodeHForExport("", 1, 0); // Assuming 0 returns all records

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

        private async Task<IActionResult> ExportToExcel(IEnumerable<CodeHViewModel> data)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Code H Records");

            // Define headers
            var headers = new List<string>
            {
                "Entered By", "Entry Date", "Operation Type", "Port", "Location",
                "Start Date Time", "Stop Date Time", "Quantity", "Grade", "Sulphur Content",
                "Tank Loaded 1", "Tank Retained 1", "Tank Loaded 2", "Tank Retained 2",
                "Tank Loaded 3", "Tank Retained 3", "Tank Loaded 4", "Tank Retained 4",
                "Status Name", "Approved By", "Comments"
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
                worksheet.Cell(row, 3).Value = item.SelectType ?? string.Empty;
                worksheet.Cell(row, 4).Value = item.Port ?? string.Empty;
                worksheet.Cell(row, 5).Value = item.Location ?? string.Empty;
                worksheet.Cell(row, 6).Value = item.StartDateTime.ToString("g"); // general date/time pattern
                worksheet.Cell(row, 7).Value = item.StopDateTime.ToString("g");
                worksheet.Cell(row, 8).Value = item.Quantity.ToString();
                worksheet.Cell(row, 9).Value = item.Grade ?? string.Empty;
                worksheet.Cell(row, 10).Value = item.SulphurContent ?? string.Empty;
                worksheet.Cell(row, 11).Value = item.TankLoaded1 ?? string.Empty;
                worksheet.Cell(row, 12).Value = item.TankRetained1 ?? string.Empty;
                worksheet.Cell(row, 13).Value = item.TankLoaded2 ?? string.Empty;
                worksheet.Cell(row, 14).Value = item.TankRetained2 ?? string.Empty;
                worksheet.Cell(row, 15).Value = item.TankLoaded3 ?? string.Empty;
                worksheet.Cell(row, 16).Value = item.TankRetained3 ?? string.Empty;
                worksheet.Cell(row, 17).Value = item.TankLoaded4 ?? string.Empty;
                worksheet.Cell(row, 18).Value = item.TankRetained4 ?? string.Empty;
                worksheet.Cell(row, 19).Value = item.StatusName ?? string.Empty;
                worksheet.Cell(row, 20).Value = item.ApprovedBy ?? string.Empty;
                worksheet.Cell(row, 21).Value = item.Comments ?? string.Empty;
                row++;
            }

            worksheet.Columns().AdjustToContents(); // Auto-fit columns

            // Save to memory stream
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            // Set the correct headers to force download
            var fileName = $"CodeH_Records_{DateTime.Now:yyyyMMdd}.xlsx";
            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName
            );
        }

        private FileContentResult ExportToPdf(IEnumerable<CodeHViewModel> data)
        {
            byte[] pdfBytes;

            using (var stream = new MemoryStream())
            {
                var document = new Document(PageSize.A1.Rotate(), 20f, 20f, 20f, 20f);
                PdfWriter.GetInstance(document, stream);

                document.Open();

                // Title
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                var title = new Paragraph("Code H Records", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER
                };
                document.Add(title);
                document.Add(new Paragraph(" "));

                // Table setup
                var table = new PdfPTable(21)
                {
                    WidthPercentage = 100
                };
                float[] columnWidths = Enumerable.Repeat(4f, 21).ToArray();
                table.SetWidths(columnWidths);

                var headers = new[]
                {
                    "Entered By", "Entry Date", "Operation Type", "Port", "Location",
                    "Start Date Time", "Stop Date Time", "Quantity", "Grade", "Sulphur Content",
                    "Tank Loaded 1", "Tank Retained 1", "Tank Loaded 2", "Tank Retained 2",
                    "Tank Loaded 3", "Tank Retained 3", "Tank Loaded 4", "Tank Retained 4",
                    "Status Name", "Approved By", "Comments"
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
                    AddCell(table, item.SelectType ?? "", dataFont);
                    AddCell(table, item.Port ?? "", dataFont);
                    AddCell(table, item.Location ?? "", dataFont);
                    AddCell(table, item.StartDateTime.ToString("g"), dataFont); // general date/time format
                    AddCell(table, item.StopDateTime.ToString("g"), dataFont);
                    AddCell(table, item.Quantity.ToString(), dataFont);
                    AddCell(table, item.Grade ?? "", dataFont);
                    AddCell(table, item.SulphurContent ?? "", dataFont);
                    AddCell(table, item.TankLoaded1 ?? "", dataFont);
                    AddCell(table, item.TankRetained1 ?? "", dataFont);
                    AddCell(table, item.TankLoaded2 ?? "", dataFont);
                    AddCell(table, item.TankRetained2 ?? "", dataFont);
                    AddCell(table, item.TankLoaded3 ?? "", dataFont);
                    AddCell(table, item.TankRetained3 ?? "", dataFont);
                    AddCell(table, item.TankLoaded4 ?? "", dataFont);
                    AddCell(table, item.TankRetained4 ?? "", dataFont);
                    AddCell(table, item.StatusName ?? "", dataFont);
                    AddCell(table, item.ApprovedBy ?? "", dataFont);
                    AddCell(table, item.Comments ?? "", dataFont);
                }

                document.Add(table);
                document.Close();

                // ✅ Copy to byte array BEFORE stream is disposed
                pdfBytes = stream.ToArray();
            }

            return File(pdfBytes, "application/pdf", $"CodeH_Records_{DateTime.Now:yyyyMMdd}.pdf");
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

        public async Task<List<CodeHViewModel>> GetCodeHForExport(string searchTerm = "", int pageNumber = 1, int pageSize = 10)
        {
            List<CodeHViewModel> records = new List<CodeHViewModel>();

            using (SqlConnection connection = _db.CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("proc_GetORB1_CodeH", connection))
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
                            records.Add(new CodeHViewModel
                            {
                                Id = reader.GetInt32(0),
                                UserId = reader.IsDBNull(1) ? null : reader.GetString(1),
                                EntryDate = reader.GetDateTime(2),
                                SelectType = reader.IsDBNull(3) ? null : reader.GetString(3),
                                Port = reader.IsDBNull(4) ? null : reader.GetString(4),
                                Location = reader.IsDBNull(5) ? null : reader.GetString(5),
                                StartDateTime = reader.GetDateTime(6),
                                StopDateTime = reader.GetDateTime(7),
                                Quantity = reader.GetDecimal(8),
                                Grade = reader.IsDBNull(9) ? null : reader.GetString(9),
                                SulphurContent = reader.IsDBNull(10) ? null : reader.GetString(10),
                                TankLoaded1 = reader.IsDBNull(11) ? null : reader.GetString(11),
                                TankRetained1 = reader.IsDBNull(12) ? null : reader.GetString(12),
                                TankLoaded2 = reader.IsDBNull(13) ? null : reader.GetString(13),
                                TankRetained2 = reader.IsDBNull(14) ? null : reader.GetString(14),
                                TankLoaded3 = reader.IsDBNull(15) ? null : reader.GetString(15),
                                TankRetained3 = reader.IsDBNull(16) ? null : reader.GetString(16),
                                TankLoaded4 = reader.IsDBNull(17) ? null : reader.GetString(17),
                                TankRetained4 = reader.IsDBNull(18) ? null : reader.GetString(18),
                                StatusName = reader.IsDBNull(19) ? null : reader.GetString(19),
                                ApprovedBy = reader.IsDBNull(20) ? null : reader.GetString(20),
                                Comments = reader.IsDBNull(21) ? null : reader.GetString(21)
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