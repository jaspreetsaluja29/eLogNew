using eLog.CompleteORBReportModels.ORB1;
using eLog.Controllers.ORB1;
using eLog.Models.ORB1;
using eLog.ViewModels.ORB1;
using eLog.ViewModels.Reports;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace eLog.Controllers
{
    public class CompleteORBEntriesController : Controller
    {
        private readonly DatabaseHelper _db;
        private readonly IConfiguration _configuration;

        public CompleteORBEntriesController(DatabaseHelper db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("CompleteORBEntries/CompleteORBReports")]
        [HttpGet]
        public async Task<IActionResult> CompleteORBReports(string codeType, string dateRange, DateTime? startDate, DateTime? endDate)
        {
            // Store filter parameters in ViewBag for the view
            ViewBag.SelectedCodeType = codeType;
            ViewBag.SelectedDateRange = dateRange;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            switch (codeType)
            {
                case "CodeH":
                    return await DownloadReportCodeH(dateRange, startDate, endDate);
                    break;
                    // Add other cases for different codes as needed
            }

            return View("~/Views/CompleteORBEntries/CompleteORBReports.cshtml");
        }

        public async Task<IActionResult> DownloadReportCodeH(string dateRange, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                // Fetch all data for the report with date filtering
                var data = await GetCodeHWithDateRange(dateRange, startDate, endDate);
                return ExportToPdfReport(data, dateRange, startDate, endDate);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        public async Task<List<CodeHReportModel>> GetCodeHWithDateRange(string dateRange, DateTime? startDate, DateTime? endDate)
        {
            List<CodeHReportModel> records = new List<CodeHReportModel>();

            // Calculate date range based on selection
            var (calculatedStartDate, calculatedEndDate) = CalculateDateRange(dateRange, startDate, endDate);

            using (SqlConnection connection = _db.CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("proc_GetORB1_CodeH_WithDateRange", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add date range parameters
                    command.Parameters.AddWithValue("@StartDate", calculatedStartDate);
                    command.Parameters.AddWithValue("@EndDate", calculatedEndDate);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            records.Add(new CodeHReportModel
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
                                Comments = reader.IsDBNull(21) ? null : reader.GetString(21),
                                RecordLastModifiedDateTime = reader.GetDateTime(22),
                                JobRank = reader.IsDBNull(23) ? null : reader.GetString(23)
                            });
                        }
                    }
                }
            }

            return records;
        }

        private (DateTime startDate, DateTime endDate) CalculateDateRange(string dateRange, DateTime? customStartDate, DateTime? customEndDate)
        {
            var today = DateTime.Today;
            var endDate = today.AddDays(1).AddSeconds(-1); // End of today

            switch (dateRange?.ToLower())
            {
                case "today":
                    return (today, endDate);

                case "yesterday":
                    var yesterday = today.AddDays(-1);
                    return (yesterday, yesterday.AddDays(1).AddSeconds(-1));

                case "last7days":
                    return (today.AddDays(-7), endDate);

                case "1month":
                    return (today.AddMonths(-1), endDate);

                case "3months":
                    return (today.AddMonths(-3), endDate);

                case "6months":
                    return (today.AddMonths(-6), endDate);

                case "1year":
                    return (today.AddYears(-1), endDate);

                case "3years":
                    return (today.AddYears(-3), endDate);

                case "custom":
                    if (customStartDate.HasValue && customEndDate.HasValue)
                    {
                        var startDate = customStartDate.Value.Date;
                        var endDateCustom = customEndDate.Value.Date.AddDays(1).AddSeconds(-1);
                        return (startDate, endDateCustom);
                    }
                    break;
            }

            // Default to last 30 days if no valid range specified
            return (today.AddDays(-30), endDate);
        }

        private FileContentResult ExportToPdfReport(IEnumerable<CodeHReportModel> data, string dateRange, DateTime? startDate, DateTime? endDate)
        {
            byte[] pdfBytes;
            var (calculatedStartDate, calculatedEndDate) = CalculateDateRange(dateRange, startDate, endDate);

            using (var stream = new MemoryStream())
            {
                var document = new Document(PageSize.A4, 30f, 30f, 50f, 40f);
                var writer = PdfWriter.GetInstance(document, stream);

                document.Open();

                // Add header with logo and company info
                AddReportHeader(document, dateRange, calculatedStartDate, calculatedEndDate);

                // Add report title and date range
                AddReportTitleSection(document, dateRange, calculatedStartDate, calculatedEndDate);

                // Add separator line
                AddSeparatorLine(document);

                if (!data.Any())
                {
                    AddNoDataSection(document);
                }
                else
                {
                    AddDataSection(document, data);
                }

                // Add footer
                AddReportFooter(document);

                document.Close();
                pdfBytes = stream.ToArray();
            }

            var fileName = $"CodeH_Report_{GetDateRangeFilenameSuffix(dateRange, calculatedStartDate, calculatedEndDate)}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }

        private void AddReportHeader(Document document, string dateRange, DateTime calculatedStartDate, DateTime calculatedEndDate)
        {
            try
            {
                // Create main header table with 2 columns for better layout control
                var headerTable = new PdfPTable(2);
                headerTable.WidthPercentage = 100;
                headerTable.SetWidths(new float[] { 70f, 30f }); // Left content, Right logo section
                headerTable.SpacingAfter = 15f;

                // Left cell with report title (extreme left and top)
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20, BaseColor.BLACK);
                var titlePhrase = new Phrase("Bunkering Of Fuel Oil or Lube Oil Report", titleFont);
                var leftCell = new PdfPCell(titlePhrase)
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_TOP,
                    Border = Rectangle.NO_BORDER,
                    PaddingTop = 0f,
                    PaddingBottom = 10f,
                    PaddingLeft = 0f
                };

                // Right cell with logo and company name (extreme right and top)
                var rightCell = new PdfPCell()
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    VerticalAlignment = Element.ALIGN_TOP,
                    Border = Rectangle.NO_BORDER,
                    PaddingTop = 0f,
                    PaddingBottom = 10f,
                    PaddingRight = 0f
                };

                // Create a nested table for logo and company name alignment
                var logoCompanyTable = new PdfPTable(1);
                logoCompanyTable.WidthPercentage = 100;

                try
                {
                    // Try to load logo
                    var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "image", "Logo.png");
                    if (System.IO.File.Exists(logoPath))
                    {
                        var logo = Image.GetInstance(logoPath);
                        logo.ScaleToFit(60f, 60f);
                        logo.Alignment = Element.ALIGN_RIGHT;

                        var logoCell = new PdfPCell()
                        {
                            HorizontalAlignment = Element.ALIGN_RIGHT,
                            VerticalAlignment = Element.ALIGN_TOP,
                            Border = Rectangle.NO_BORDER,
                            PaddingBottom = 5f
                        };
                        logoCell.AddElement(logo);
                        logoCompanyTable.AddCell(logoCell);
                    }
                    else
                    {
                        // Fallback if logo not found - create a colored rectangle as placeholder
                        var logoPlaceholder = new PdfPTable(1);
                        logoPlaceholder.SetWidths(new float[] { 1f });
                        var placeholderCell = new PdfPCell(new Phrase("LOGO", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8, BaseColor.WHITE)))
                        {
                            BackgroundColor = new BaseColor(180, 60, 60), // Red background similar to screenshot
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            FixedHeight = 50f,
                            Border = Rectangle.NO_BORDER
                        };
                        logoPlaceholder.AddCell(placeholderCell);

                        var logoCell = new PdfPCell()
                        {
                            HorizontalAlignment = Element.ALIGN_RIGHT,
                            VerticalAlignment = Element.ALIGN_TOP,
                            Border = Rectangle.NO_BORDER,
                            PaddingBottom = 5f
                        };
                        logoCell.AddElement(logoPlaceholder);
                        logoCompanyTable.AddCell(logoCell);
                    }
                }
                catch
                {
                    // Fallback if logo loading fails
                    var logoPlaceholder = new PdfPTable(1);
                    logoPlaceholder.SetWidths(new float[] { 1f });
                    var placeholderCell = new PdfPCell(new Phrase("LOGO", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8, BaseColor.WHITE)))
                    {
                        BackgroundColor = new BaseColor(180, 60, 60), // Red background
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        FixedHeight = 50f,
                        Border = Rectangle.NO_BORDER
                    };
                    logoPlaceholder.AddCell(placeholderCell);

                    var logoCell = new PdfPCell()
                    {
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        VerticalAlignment = Element.ALIGN_TOP,
                        Border = Rectangle.NO_BORDER,
                        PaddingBottom = 5f
                    };
                    logoCell.AddElement(logoPlaceholder);
                    logoCompanyTable.AddCell(logoCell);
                }

                // Add company name below logo (DigitalLog)
                var companyFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.DARK_GRAY);
                var companyCell = new PdfPCell(new Phrase(_configuration["CompanyName"], companyFont))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    VerticalAlignment = Element.ALIGN_TOP,
                    Border = Rectangle.NO_BORDER,
                    PaddingTop = 0f
                };
                logoCompanyTable.AddCell(companyCell);

                // Add the logo and company table to the right cell
                rightCell.AddElement(logoCompanyTable);

                // Add cells to header table
                headerTable.AddCell(leftCell);
                headerTable.AddCell(rightCell);
                document.Add(headerTable);

                // Add vessel details section
                var detailsTable = new PdfPTable(4);
                detailsTable.WidthPercentage = 100;
                detailsTable.SetWidths(new float[] { 15f, 35f, 15f, 35f }); // Label, Value, Label, Value
                detailsTable.SpacingAfter = 10f;

                var labelFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.BLACK);
                var valueFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.BLACK);

                // Vessel row
                AddDetailCell(detailsTable, "Vessel", labelFont, Element.ALIGN_LEFT);
                AddDetailCell(detailsTable, $": {_configuration["VesselName"] ?? "No Data"}", valueFont, Element.ALIGN_LEFT);
                AddDetailCell(detailsTable, "IMO Number", labelFont, Element.ALIGN_LEFT);
                AddDetailCell(detailsTable, $": {_configuration["IMONo"] ?? "No Data"}", valueFont, Element.ALIGN_LEFT);

                // Date and Status row
                var dateRangeText = GetDateRangeDisplayText(dateRange, calculatedStartDate, calculatedEndDate);
                AddDetailCell(detailsTable, "Date", labelFont, Element.ALIGN_LEFT);
                AddDetailCell(detailsTable, $": {dateRangeText}", valueFont, Element.ALIGN_LEFT);
                AddDetailCell(detailsTable, "Printed", labelFont, Element.ALIGN_LEFT);
                AddDetailCell(detailsTable, $": {DateTime.Now:dd. MMM yyyy HH:mm}", valueFont, Element.ALIGN_LEFT);

                // Status row
                AddDetailCell(detailsTable, "Status", labelFont, Element.ALIGN_LEFT);
                AddDetailCell(detailsTable, ": Signed by Demo User", valueFont, Element.ALIGN_LEFT);
                AddDetailCell(detailsTable, "", labelFont, Element.ALIGN_LEFT); // Empty cell
                AddDetailCell(detailsTable, "", valueFont, Element.ALIGN_LEFT); // Empty cell

                document.Add(detailsTable);
            }
            catch (Exception ex)
            {
                // Fallback header if anything goes wrong
                var fallbackFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                var fallbackHeader = new Paragraph("Bunkering Of Fuel Oil or Lube Oil Report", fallbackFont);
                fallbackHeader.Alignment = Element.ALIGN_CENTER;
                fallbackHeader.SpacingAfter = 20f;
                document.Add(fallbackHeader);
            }
        }

        // Helper method to add detail cells
        private void AddDetailCell(PdfPTable table, string text, iTextSharp.text.Font font, int alignment)
        {
            var cell = new PdfPCell(new Phrase(text, font))
            {
                HorizontalAlignment = alignment,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Border = Rectangle.NO_BORDER,
                PaddingTop = 2f,
                PaddingBottom = 2f,
                PaddingLeft = 5f
            };
            table.AddCell(cell);
        }

        private void AddReportTitleSection(Document document, string dateRange, DateTime calculatedStartDate, DateTime calculatedEndDate)
        {
            // Report title - left aligned, smaller font
            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, BaseColor.BLACK);
            var title = new Paragraph("4.1 Bunkering of Fuel", titleFont);
            title.Alignment = Element.ALIGN_LEFT;
            title.SpacingAfter = 8f;
            document.Add(title);

            // Date range - left aligned with bold label
            //var subtitleFont = FontFactory.GetFont(FontFactory.HELVETICA, 11, BaseColor.DARK_GRAY);
            //var boldSubtitleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11, BaseColor.DARK_GRAY);
            //var dateRangeText = GetDateRangeDisplayText(dateRange, calculatedStartDate, calculatedEndDate);

            //var subtitleParagraph = new Paragraph();
            //subtitleParagraph.Add(new Phrase("Report Period: ", boldSubtitleFont));
            //subtitleParagraph.Add(new Phrase(dateRangeText, subtitleFont));
            //subtitleParagraph.Alignment = Element.ALIGN_LEFT;
            //subtitleParagraph.SpacingAfter = 15f;
            //document.Add(subtitleParagraph);
        }

        private void AddSeparatorLine(Document document)
        {
            var line = new LineSeparator(1f, 100f, BaseColor.LIGHT_GRAY, Element.ALIGN_CENTER, -2);
            document.Add(new Chunk(line));
            document.Add(new Paragraph(" ") { SpacingAfter = 10f });
        }

        private void AddNoDataSection(Document document)
        {
            // Add spacing before the no-data section
            document.Add(new Paragraph(" ") { SpacingAfter = 30f });

            var noDataFont = FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.DARK_GRAY);
            var noDataParagraph = new Paragraph("No records found for the selected date range.", noDataFont);
            noDataParagraph.Alignment = Element.ALIGN_CENTER;

            // Add a border around the no-data message
            var noDataTable = new PdfPTable(1);
            noDataTable.WidthPercentage = 60;
            noDataTable.HorizontalAlignment = Element.ALIGN_CENTER;
            noDataTable.SpacingBefore = 20f;
            noDataTable.SpacingAfter = 30f;

            var noDataCell = new PdfPCell(noDataParagraph)
            {
                Padding = 20f,
                BorderColor = BaseColor.LIGHT_GRAY,
                BorderWidth = 1f,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                MinimumHeight = 60f
            };

            noDataTable.AddCell(noDataCell);
            document.Add(noDataTable);

            // Add spacing after the no-data section
            document.Add(new Paragraph(" ") { SpacingAfter = 20f });
        }

        private void AddDataSection(Document document, IEnumerable<CodeHReportModel> data)
        {
            // Group data by date for better organization
            var groupedData = data.GroupBy(x => x.EntryDate.Date).OrderBy(g => g.Key);

            foreach (var dateGroup in groupedData)
            {
                // Create main data table
                var table = new PdfPTable(4);
                table.WidthPercentage = 100;
                table.SpacingAfter = 15f;

                // Set column widths: Date (12%), Code (8%), Item No. (12%), Record (68%)
                float[] columnWidths = { 12f, 8f, 12f, 68f };
                table.SetWidths(columnWidths);

                // Add headers with styling
                AddStyledHeaderRow(table);

                var records = dateGroup.OrderBy(x => x.StartDateTime).ToList();

                for (int i = 0; i < records.Count; i++)
                {
                    var record = records[i];
                    AddStyledDataRowsInTableFormat(table, record, i, dateGroup.Key);
                }

                document.Add(table);
            }
        }

        private void AddStyledHeaderRow(PdfPTable table)
        {
            var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE);
            var headerColor = new BaseColor(60, 60, 60); // Dark gray

            AddStyledReportCell(table, "Date", headerFont, headerColor, Element.ALIGN_CENTER, true);
            AddStyledReportCell(table, "Code", headerFont, headerColor, Element.ALIGN_CENTER, true);
            AddStyledReportCell(table, "Item No.", headerFont, headerColor, Element.ALIGN_CENTER, true);
            AddStyledReportCell(table, "Record of operations/signature of officer in charge", headerFont, headerColor, Element.ALIGN_CENTER, true);
        }

        private void AddStyledDataRowsInTableFormat(PdfPTable table, CodeHReportModel record, int recordIndex, DateTime groupDate)
        {
            var cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 9);
            var lightGray = new BaseColor(248, 248, 248);
            var white = BaseColor.WHITE;

            // Get all operation lines
            var operationLines = BuildOperationRecordLines(record);

            // Add first row with date, code, item number, and first operation line
            var backgroundColor = recordIndex % 2 == 0 ? white : lightGray;

            // Date cell (only show for first row of each record)
            AddStyledReportCell(table, groupDate.ToString("dd-MMM-yyyy").ToUpper(), cellFont, backgroundColor, Element.ALIGN_CENTER, false);

            // Code cell
            AddStyledReportCell(table, "H", cellFont, backgroundColor, Element.ALIGN_CENTER, false);

            // Item number cell
            AddStyledReportCell(table, $"26.{recordIndex + 1}", cellFont, backgroundColor, Element.ALIGN_CENTER, false);

            // First operation line
            if (operationLines.Count > 0)
            {
                AddStyledReportCell(table, operationLines[0], cellFont, backgroundColor, Element.ALIGN_LEFT, false);
            }
            else
            {
                AddStyledReportCell(table, "", cellFont, backgroundColor, Element.ALIGN_LEFT, false);
            }

            // Add remaining operation lines as separate rows
            for (int i = 1; i < operationLines.Count; i++)
            {
                // Empty cells for date, code, and item number
                AddStyledReportCell(table, "", cellFont, backgroundColor, Element.ALIGN_CENTER, false);
                AddStyledReportCell(table, "", cellFont, backgroundColor, Element.ALIGN_CENTER, false);
                AddStyledReportCell(table, "", cellFont, backgroundColor, Element.ALIGN_CENTER, false);

                // Operation line
                AddStyledReportCell(table, operationLines[i], cellFont, backgroundColor, Element.ALIGN_LEFT, false);
            }
        }

        private void AddReportFooter(Document document)
        {
            // Add footer with page numbers and generation info
            var footerFont = FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.GRAY);
            var footer = new Paragraph($"Generated by {_configuration["CompanyName"]} | {DateTime.Now:dd-MMM-yyyy HH:mm}", footerFont);
            footer.Alignment = Element.ALIGN_CENTER;
            footer.SpacingBefore = 20f;

            // Add separator line before footer
            var line = new LineSeparator(0.5f, 100f, BaseColor.LIGHT_GRAY, Element.ALIGN_CENTER, -2);
            document.Add(new Chunk(line));
            document.Add(footer);
        }

        private void AddStyledReportCell(PdfPTable table, string text, iTextSharp.text.Font font, BaseColor backgroundColor, int alignment, bool isHeader)
        {
            var cell = new PdfPCell(new Phrase(text, font))
            {
                HorizontalAlignment = alignment,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Padding = isHeader ? 8f : 6f,
                MinimumHeight = isHeader ? 25f : 20f,
                BackgroundColor = backgroundColor,
                BorderColor = BaseColor.LIGHT_GRAY,
                BorderWidth = 0.5f
            };

            table.AddCell(cell);
        }

        private string GetDateRangeDisplayText(string dateRange, DateTime startDate, DateTime endDate)
        {
            switch (dateRange?.ToLower())
            {
                case "today":
                    return $"{DateTime.Today:dd-MMM-yyyy}";
                case "yesterday":
                    return $"{DateTime.Today.AddDays(-1):dd-MMM-yyyy}";
                case "last7days":
                    return $"Last 7 Days ({DateTime.Today.AddDays(-7):dd-MMM-yyyy} to {DateTime.Today:dd-MMM-yyyy})";
                case "1month":
                    return $"Last 1 Month ({DateTime.Today.AddMonths(-1):dd-MMM-yyyy} to {DateTime.Today:dd-MMM-yyyy})";
                case "3months":
                    return $"Last 3 Months ({DateTime.Today.AddMonths(-3):dd-MMM-yyyy} to {DateTime.Today:dd-MMM-yyyy})";
                case "6months":
                    return $"Last 6 Months ({DateTime.Today.AddMonths(-6):dd-MMM-yyyy} to {DateTime.Today:dd-MMM-yyyy})";
                case "1year":
                    return $"Last 1 Year ({DateTime.Today.AddYears(-1):dd-MMM-yyyy} to {DateTime.Today:dd-MMM-yyyy})";
                case "3years":
                    return $"Last 3 Years ({DateTime.Today.AddYears(-3):dd-MMM-yyyy} to {DateTime.Today:dd-MMM-yyyy})";
                case "custom":
                    return $"{startDate:dd-MMM-yyyy} to {endDate.Date:dd-MMM-yyyy}";
                default:
                    return $"{startDate:dd-MMM-yyyy} to {endDate.Date:dd-MMM-yyyy}";
            }
        }

        private string GetDateRangeFilenameSuffix(string dateRange, DateTime startDate, DateTime endDate)
        {
            switch (dateRange?.ToLower())
            {
                case "today":
                    return "Today";
                case "yesterday":
                    return "Yesterday";
                case "last7days":
                    return "Last7Days";
                case "1month":
                    return "Last1Month";
                case "3months":
                    return "Last3Months";
                case "6months":
                    return "Last6Months";
                case "1year":
                    return "Last1Year";
                case "3years":
                    return "Last3Years";
                case "custom":
                    return $"{startDate:yyyyMMdd}_to_{endDate.Date:yyyyMMdd}";
                default:
                    return $"{startDate:yyyyMMdd}_to_{endDate.Date:yyyyMMdd}";
            }
        }

        private List<string> BuildOperationRecordLines(CodeHReportModel record)
        {
            var lines = new List<string>();

            // Location/Port information
            if (!string.IsNullOrEmpty(record.Port))
            {
                lines.Add($"{record.Port.ToUpper()}");
            }

            // Operation time details
            if (record.StartDateTime != default && record.StopDateTime != default)
            {
                lines.Add($"START: {record.StartDateTime:dd-MMM-yyyy} - {record.StartDateTime:HH:mm} HRS STOP: {record.StopDateTime:dd-MMM-yyyy} - {record.StopDateTime:HH:mm} HRS".ToUpper());
            }

            // Quantity and fuel details
            if (record.Quantity > 0)
            {
                var quantityText = $"{record.Quantity:F0} MT of {(record.Grade ?? "FUEL").ToUpper()}";
                if (!string.IsNullOrEmpty(record.SulphurContent))
                {
                    quantityText += $" {record.SulphurContent.ToUpper()}";
                }
                quantityText += " BUNKERED IN TANKS";
                lines.Add(quantityText.ToUpper());
            }

            // Tank loading details
            AddTankDetailsToLines(lines, record.TankLoaded1, record.TankRetained1);
            AddTankDetailsToLines(lines, record.TankLoaded2, record.TankRetained2);
            AddTankDetailsToLines(lines, record.TankLoaded3, record.TankRetained3);
            AddTankDetailsToLines(lines, record.TankLoaded4, record.TankRetained4);

            // Comments if any
            if (!string.IsNullOrEmpty(record.Comments))
            {
                lines.Add($"COMMENTS: {record.Comments.ToUpper()}");
            }

            // Signature line
            if (!string.IsNullOrEmpty(record.ApprovedBy))
            {
                lines.Add($"SIGNED: (NAME: {record.ApprovedBy.ToUpper()} & RANK: {record.JobRank.ToUpper()}) RECORD APPROVED DATE: {record.RecordLastModifiedDateTime:dd-MMM-yyyy}".ToUpper());
            }
            else
            {
                lines.Add($"SIGNED: (NOT APPROVED BY OFFICER-IN-CHARGE) RECORD LAST MODIFIED DATE: {record.RecordLastModifiedDateTime:dd-MMM-yyyy}".ToUpper());
            }

            return lines;
        }

        private void AddTankDetailsToLines(List<string> lines, string tankLoaded, string tankRetained)
        {
            if (!string.IsNullOrEmpty(tankLoaded))
            {
                var tankQuantity = GetTankQuantity(tankLoaded);
                var tankInfo = $"{tankQuantity} MT ADDED TO {tankLoaded}";

                if (!string.IsNullOrEmpty(tankRetained))
                {
                    tankInfo += $" NOW CONTAINING {GetTankQuantity(tankRetained)} MT";
                }
                lines.Add(tankInfo);
            }
        }

        private string GetTankQuantity(string tankInfo)
        {
            if (string.IsNullOrEmpty(tankInfo))
                return "0";

            var parts = tankInfo.Split(' ');
            foreach (var part in parts)
            {
                if (decimal.TryParse(part, out decimal qty))
                {
                    return qty.ToString("F0");
                }
            }

            return "900";
        }
    }
}