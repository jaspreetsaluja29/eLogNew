using ClosedXML.Excel;
using eLog.ViewModels.ORB1;
using eLog.ViewModels.Reports;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System.Data;

namespace eLog.Controllers
{
    public class ReportsController : Controller
    {
        private readonly DatabaseHelper _db;

        public ReportsController(DatabaseHelper db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("Reports/LatestInventoryTanks")]
        [HttpGet]
        public async Task<IActionResult> LatestInventoryTanks(int pageNumber = 1, int pageSize = 10)
        {
            List<LatestInventoryTanksModel> records = new List<LatestInventoryTanksModel>();
            int totalRecords = 0;
            int count = 1;

            using (SqlConnection connection = _db.CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("proc_GetReports_LatestInventoryTanks", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            records.Add(new LatestInventoryTanksModel
                            {
                                SNo = count++,
                                EntryDate = reader.IsDBNull(0) ? null : reader.GetDateTime(0),
                                IdentityOfTanks = reader.IsDBNull(1) ? null : reader.GetString(1),
                                LastRetainedOnBoard = reader.IsDBNull(2) ? null : reader.GetDecimal(2)
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

            return View("~/Views/Reports/LatestInventoryTanks.cshtml", paginatedRecords);
        }

        [Route("Reports/HistoryForTanks")]
        [HttpGet]
        public async Task<IActionResult> HistoryForTanks(string tankId = "", string dateRange = "1month",
            string startDate = null, string endDate = null, int pageNumber = 1, int pageSize = 10)
        {
            string tankName = null;
            // Load tanks for dropdown
            await LoadTanksDropdown();

            if (tankId != "")
            {
                // Get tank name by ID
                tankName = await GetTankNameById(tankId);
                ViewBag.TankName = tankName;
            }
            else
            {
                ViewBag.TankName = "All Tanks";
            }
            // Set ViewBag values for maintaining form state
            ViewBag.SelectedTankId = tankId;
            ViewBag.SelectedDateRange = dateRange;
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            // Calculate date range based on selection
            DateTime endDateTime = DateTime.Now;
            DateTime startDateTime = DateTime.Now;

            switch (dateRange)
            {
                case "1month":
                    startDateTime = endDateTime.AddMonths(-1);
                    break;
                case "3months":
                    startDateTime = endDateTime.AddMonths(-3);
                    break;
                case "6months":
                    startDateTime = endDateTime.AddMonths(-6);
                    break;
                case "1year":
                    startDateTime = endDateTime.AddYears(-1);
                    break;
                case "3years":
                    startDateTime = endDateTime.AddYears(-3);
                    break;
                case "custom":
                    if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                    {
                        startDateTime = DateTime.Parse(startDate);
                        endDateTime = DateTime.Parse(endDate);
                    }
                    break;
            }

            List<HistoryForTanksModel> records = new List<HistoryForTanksModel>();
            int totalRecords = 0;
            int count = 1;

            using (SqlConnection connection = _db.CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("proc_GetReports_HistoryForTanks", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TankId", string.IsNullOrEmpty(tankName) ? DBNull.Value : (object)tankName);
                    command.Parameters.AddWithValue("@StartDate", startDateTime);
                    command.Parameters.AddWithValue("@EndDate", endDateTime);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            records.Add(new HistoryForTanksModel
                            {
                                SNo = count++,
                                EntryDate = reader.IsDBNull(0) ? null : reader.GetDateTime(0),
                                IdentityOfTanks = reader.IsDBNull(1) ? null : reader.GetString(1),
                                LastRetainedOnBoard = reader.IsDBNull(2) ? null : reader.GetDecimal(2),
                                ModAppRejDate = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                                Status = reader.IsDBNull(4) ? null : reader.GetString(4)
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

            return View("~/Views/Reports/HistoryForTanks.cshtml", paginatedRecords);
        }

        private async Task LoadTanksDropdown()
        {
            List<SelectListItem> tanks = new List<SelectListItem>();

            using (SqlConnection connection = _db.CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("proc_GetReports_HFT_Tanks", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string tankId = reader.GetInt64(0).ToString();
                            string tankName = reader.GetString(1);
                            tanks.Add(new SelectListItem { Value = tankId, Text = tankName });
                        }
                    }
                }
            }

            ViewBag.Tanks = tanks;
        }

        private async Task<string> GetTankNameById(string tankId)
        {
            string tankName = null;

            using (SqlConnection connection = _db.CreateConnection())
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("proc_GetTankNameById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TankId", tankId);

                    var result = await command.ExecuteScalarAsync();
                    if (result != null && result != DBNull.Value)
                    {
                        tankName = result.ToString();
                    }
                }
            }

            return tankName;
        }

        [Route("Reports/BunkeringData")]
        [HttpGet]
        public async Task<IActionResult> BunkeringData(string tankId = "", string dateRange = "1month",
           string startDate = null, string endDate = null, int pageNumber = 1, int pageSize = 10)
        {
            string tankName = null;
            // Load tanks for dropdown
            await BDLoadTanksDropdown();

            if (tankId != "")
            {
                // Get tank name by ID
                tankName = await GetBDTankNameById(tankId);
                ViewBag.TankName = tankName;
            }
            else
            {
                ViewBag.TankName = "All Tanks";
            }
            // Set ViewBag values for maintaining form state
            ViewBag.SelectedTankId = tankId;
            ViewBag.SelectedDateRange = dateRange;
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            // Calculate date range based on selection
            DateTime endDateTime = DateTime.Now;
            DateTime startDateTime = DateTime.Now;

            switch (dateRange)
            {
                case "1month":
                    startDateTime = endDateTime.AddMonths(-1);
                    break;
                case "3months":
                    startDateTime = endDateTime.AddMonths(-3);
                    break;
                case "6months":
                    startDateTime = endDateTime.AddMonths(-6);
                    break;
                case "1year":
                    startDateTime = endDateTime.AddYears(-1);
                    break;
                case "3years":
                    startDateTime = endDateTime.AddYears(-3);
                    break;
                case "custom":
                    if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                    {
                        startDateTime = DateTime.Parse(startDate);
                        endDateTime = DateTime.Parse(endDate);
                    }
                    break;
            }

            List<BunkeringDataModel> records = new List<BunkeringDataModel>();
            int totalRecords = 0;
            int count = 1;

            using (SqlConnection connection = _db.CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("proc_GetReports_BunkeringData", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TankId", string.IsNullOrEmpty(tankName) ? DBNull.Value : (object)tankName);
                    command.Parameters.AddWithValue("@StartDate", startDateTime);
                    command.Parameters.AddWithValue("@EndDate", endDateTime);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            records.Add(new BunkeringDataModel
                            {
                                SNo = count++,
                                Tanks = reader.IsDBNull(0) ? null : reader.GetString(0),
                                Capacity = reader.IsDBNull(1) ? null : reader.GetDecimal(1),
                                EntryDate = reader.IsDBNull(2) ? null : reader.GetDateTime(2),
                                Quantity = reader.IsDBNull(3) ? null : reader.GetDecimal(3),
                                Grade = reader.IsDBNull(4) ? null : reader.GetString(4),
                                SulphurContent = reader.IsDBNull(5) ? null : reader.GetString(5),
                                Port = reader.IsDBNull(6) ? null : reader.GetString(6)
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

            return View("~/Views/Reports/BunkeringData.cshtml", paginatedRecords);
        }

        private async Task BDLoadTanksDropdown()
        {
            List<SelectListItem> tanks = new List<SelectListItem>();

            using (SqlConnection connection = _db.CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("proc_GetReports_BD_Tanks", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string tankId = reader.GetInt32(0).ToString();
                            string tankName = reader.GetString(1);
                            tanks.Add(new SelectListItem { Value = tankId, Text = tankName });
                        }
                    }
                }
            }

            ViewBag.Tanks = tanks;
        }

        private async Task<string> GetBDTankNameById(string tankId)
        {
            string tankName = null;

            using (SqlConnection connection = _db.CreateConnection())
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("proc_GetBDTankNameById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TankId", tankId);

                    var result = await command.ExecuteScalarAsync();
                    if (result != null && result != DBNull.Value)
                    {
                        tankName = result.ToString();
                    }
                }
            }
            return tankName;
        }

        [HttpGet]
        [Route("Reports/DownloadBunkeringData")]
        public async Task<IActionResult> DownloadBunkeringData(string format)
        {
            try
            {
                // Fetch all data without filters or pagination
                var data = await GetBunkeringDataForExport("", 1, 0); // Assuming 0 returns all records

                if (format?.ToLower() == "excel")
                {
                    return await ExportToExcelBunkeringData(data);
                }
                else if (format?.ToLower() == "pdf")
                {
                    return ExportToPdfBukeringData(data); // Implement this method if not already
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

        private async Task<IActionResult> ExportToExcelBunkeringData(IEnumerable<BunkeringDataModel> data)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Bunkering Data Records");

            // Define headers
            var headers = new List<string>
            {
                "Tanks",
                "Capacity",
                "Entry Date",
                "Quantity",
                "Grade",
                "Sulphur Content",
                "Port"
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
                worksheet.Cell(row, 1).Value = item.Tanks ?? string.Empty;
                worksheet.Cell(row, 2).Value = item.Capacity?.ToString() ?? string.Empty;
                worksheet.Cell(row, 3).Value = item.EntryDate?.ToShortDateString() ?? string.Empty;
                worksheet.Cell(row, 4).Value = item.Quantity?.ToString() ?? string.Empty;
                worksheet.Cell(row, 5).Value = item.Grade ?? string.Empty;
                worksheet.Cell(row, 6).Value = item.SulphurContent ?? string.Empty;
                worksheet.Cell(row, 7).Value = item.Port ?? string.Empty;
                row++;
            }

            worksheet.Columns().AdjustToContents(); // Auto-fit columns

            // Save to memory stream
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            // Set the correct headers to force download
            var fileName = $"BunkeringData_Records_{DateTime.Now:yyyyMMdd}.xlsx";
            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName
            );
        }

        private FileContentResult ExportToPdfBukeringData(IEnumerable<BunkeringDataModel> data)
        {
            byte[] pdfBytes;

            using (var stream = new MemoryStream())
            {
                var document = new Document(PageSize.A4.Rotate(), 20f, 20f, 20f, 20f);
                PdfWriter.GetInstance(document, stream);

                document.Open();

                // Title
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                var title = new Paragraph("Bunkering Data Records", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER
                };
                document.Add(title);
                document.Add(new Paragraph(" "));

                // Table setup
                var table = new PdfPTable(7)
                {
                    WidthPercentage = 100
                };
                float[] columnWidths = Enumerable.Repeat(4f, 7).ToArray();
                table.SetWidths(columnWidths);

                var headers = new[]
                {
                "Tanks",
                "Capacity",
                "Entry Date",
                "Quantity",
                "Grade",
                "Sulphur Content",
                "Port"
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
                    AddCell(table, item.Tanks ?? "", dataFont);
                    AddCell(table, item.Capacity?.ToString() ?? "", dataFont);
                    AddCell(table, item.EntryDate?.ToShortDateString() ?? "", dataFont);
                    AddCell(table, item.Quantity?.ToString() ?? "", dataFont);
                    AddCell(table, item.Grade ?? "", dataFont);
                    AddCell(table, item.SulphurContent ?? "", dataFont);
                    AddCell(table, item.Port ?? "", dataFont);
                }

                document.Add(table);
                document.Close();

                // ✅ Copy to byte array BEFORE stream is disposed
                pdfBytes = stream.ToArray();
            }

            return File(pdfBytes, "application/pdf", $"BunkeringData_Records_{DateTime.Now:yyyyMMdd}.pdf");
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

        public async Task<List<BunkeringDataModel>> GetBunkeringDataForExport(string searchTerm = "", int pageNumber = 1, int pageSize = 10)
        {
            List<BunkeringDataModel> records = new List<BunkeringDataModel>();

            using (SqlConnection connection = _db.CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("proc_GetReports_BunkeringData", connection))
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
                            records.Add(new BunkeringDataModel
                            {
                                Tanks = reader.IsDBNull(0) ? null : reader.GetString(0),
                                Capacity = reader.IsDBNull(1) ? null : reader.GetDecimal(1),
                                EntryDate = reader.IsDBNull(2) ? null : reader.GetDateTime(2),
                                Quantity = reader.IsDBNull(3) ? null : reader.GetDecimal(3),
                                Grade = reader.IsDBNull(4) ? null : reader.GetString(4),
                                SulphurContent = reader.IsDBNull(5) ? null : reader.GetString(5),
                                Port = reader.IsDBNull(6) ? null : reader.GetString(6)
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

        [HttpGet]
        [Route("Reports/DownloadHistoryForTanks")]
        public async Task<IActionResult> DownloadHistoryForTanks(string format)
        {
            try
            {
                // Fetch all data without filters or pagination
                var data = await GetHistoryForTanksForExport("", 1, 0); // Assuming 0 returns all records

                if (format?.ToLower() == "excel")
                {
                    return await ExportToExcelHistoryForTanks(data);
                }
                else if (format?.ToLower() == "pdf")
                {
                    return ExportToPdfHistoryForTanks(data); // Implement this method if not already
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

        private async Task<IActionResult> ExportToExcelHistoryForTanks(IEnumerable<HistoryForTanksModel> data)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("History For Tanks Records");

            // Define headers
            var headers = new List<string>
            {
                "Entry Date",
                "Identity of Tanks",
                "Last Retained On Board",
                "Modified/Approved/Rejected Date",
                "Status"
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
                worksheet.Cell(row, 1).Value = item.EntryDate?.ToShortDateString() ?? string.Empty;
                worksheet.Cell(row, 2).Value = item.IdentityOfTanks ?? string.Empty;
                worksheet.Cell(row, 3).Value = item.LastRetainedOnBoard?.ToString() ?? string.Empty;
                worksheet.Cell(row, 4).Value = item.ModAppRejDate?.ToShortDateString() ?? string.Empty;
                worksheet.Cell(row, 5).Value = item.Status ?? string.Empty;
                row++;
            }

            worksheet.Columns().AdjustToContents(); // Auto-fit columns

            // Save to memory stream
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            // Set the correct headers to force download
            var fileName = $"HistoryForTanks_Records_{DateTime.Now:yyyyMMdd}.xlsx";
            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName
            );
        }

        private FileContentResult ExportToPdfHistoryForTanks(IEnumerable<HistoryForTanksModel> data)
        {
            byte[] pdfBytes;

            using (var stream = new MemoryStream())
            {
                var document = new Document(PageSize.A4.Rotate(), 20f, 20f, 20f, 20f);
                PdfWriter.GetInstance(document, stream);

                document.Open();

                // Title
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                var title = new Paragraph("History For Tanks Records", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER
                };
                document.Add(title);
                document.Add(new Paragraph(" "));

                // Table setup
                var table = new PdfPTable(5)
                {
                    WidthPercentage = 100
                };
                float[] columnWidths = Enumerable.Repeat(4f, 5).ToArray();
                table.SetWidths(columnWidths);

                var headers = new[]
                {
                "Entry Date",
                "Identity of Tanks",
                "Last Retained On Board",
                "Modified/Approved/Rejected Date",
                "Status"
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
                    AddCell(table, item.EntryDate?.ToShortDateString() ?? "", dataFont);
                    AddCell(table, item.IdentityOfTanks ?? "", dataFont);
                    AddCell(table, item.LastRetainedOnBoard?.ToString() ?? "", dataFont);
                    AddCell(table, item.ModAppRejDate?.ToShortDateString() ?? "", dataFont);
                    AddCell(table, item.Status ?? "", dataFont);
                }

                document.Add(table);
                document.Close();

                // ✅ Copy to byte array BEFORE stream is disposed
                pdfBytes = stream.ToArray();
            }

            return File(pdfBytes, "application/pdf", $"HistoryForTanks_Records_{DateTime.Now:yyyyMMdd}.pdf");
        }

        public async Task<List<HistoryForTanksModel>> GetHistoryForTanksForExport(string searchTerm = "", int pageNumber = 1, int pageSize = 10)
        {
            List<HistoryForTanksModel> records = new List<HistoryForTanksModel>();

            using (SqlConnection connection = _db.CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("proc_GetReports_HistoryForTanks", connection))
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
                            records.Add(new HistoryForTanksModel
                            {
                                EntryDate = reader.IsDBNull(0) ? null : reader.GetDateTime(0),
                                IdentityOfTanks = reader.IsDBNull(1) ? null : reader.GetString(1),
                                LastRetainedOnBoard = reader.IsDBNull(2) ? null : reader.GetDecimal(2),
                                ModAppRejDate = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                                Status = reader.IsDBNull(4) ? null : reader.GetString(4)
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

        [HttpGet]
        [Route("Reports/DownloadLatestInventory")]
        public async Task<IActionResult> DownloadLatestInventory(string format)
        {
            try
            {
                // Fetch all data without filters or pagination
                var data = await GetLatestInventoryForExport("", 1, 0); // Assuming 0 returns all records

                if (format?.ToLower() == "excel")
                {
                    return await ExportToExcelLatestInventory(data);
                }
                else if (format?.ToLower() == "pdf")
                {
                    return ExportToPdfLatestInventory(data); // Implement this method if not already
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

        private async Task<IActionResult> ExportToExcelLatestInventory(IEnumerable<LatestInventoryTanksModel> data)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Latest Inventory Records");

            // Define headers
            var headers = new List<string>
            {
                "Entry Date",
                "Identity of Tanks",
                "Last Retained On Board"
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
                worksheet.Cell(row, 1).Value = item.EntryDate?.ToShortDateString() ?? string.Empty;
                worksheet.Cell(row, 2).Value = item.IdentityOfTanks ?? string.Empty;
                worksheet.Cell(row, 3).Value = item.LastRetainedOnBoard?.ToString() ?? string.Empty;
                row++;
            }

            worksheet.Columns().AdjustToContents(); // Auto-fit columns

            // Save to memory stream
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            // Set the correct headers to force download
            var fileName = $"LatestInventory_Records_{DateTime.Now:yyyyMMdd}.xlsx";
            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName
            );
        }

        private FileContentResult ExportToPdfLatestInventory(IEnumerable<LatestInventoryTanksModel> data)
        {
            byte[] pdfBytes;

            using (var stream = new MemoryStream())
            {
                var document = new Document(PageSize.A4.Rotate(), 20f, 20f, 20f, 20f);
                PdfWriter.GetInstance(document, stream);

                document.Open();

                // Title
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                var title = new Paragraph("Latest Inventory Records", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER
                };
                document.Add(title);
                document.Add(new Paragraph(" "));

                // Table setup
                var table = new PdfPTable(3)
                {
                    WidthPercentage = 100
                };
                float[] columnWidths = Enumerable.Repeat(4f, 3).ToArray();
                table.SetWidths(columnWidths);

                var headers = new[]
                {
                    "Entry Date",
                    "Identity of Tanks",
                    "Last Retained On Board"
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
                    AddCell(table, item.EntryDate?.ToShortDateString() ?? "", dataFont);
                    AddCell(table, item.IdentityOfTanks ?? "", dataFont);
                    AddCell(table, item.LastRetainedOnBoard?.ToString() ?? "", dataFont);
                }

                document.Add(table);
                document.Close();

                // ✅ Copy to byte array BEFORE stream is disposed
                pdfBytes = stream.ToArray();
            }

            return File(pdfBytes, "application/pdf", $"LatestInventory_Records_{DateTime.Now:yyyyMMdd}.pdf");
        }

        public async Task<List<LatestInventoryTanksModel>> GetLatestInventoryForExport(string searchTerm = "", int pageNumber = 1, int pageSize = 10)
        {
            List<LatestInventoryTanksModel> records = new List<LatestInventoryTanksModel>();

            using (SqlConnection connection = _db.CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("proc_GetReports_LatestInventoryTanks", connection))
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
                            records.Add(new LatestInventoryTanksModel
                            {
                                EntryDate = reader.IsDBNull(0) ? null : reader.GetDateTime(0),
                                IdentityOfTanks = reader.IsDBNull(1) ? null : reader.GetString(1),
                                LastRetainedOnBoard = reader.IsDBNull(2) ? null : reader.GetDecimal(2)
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