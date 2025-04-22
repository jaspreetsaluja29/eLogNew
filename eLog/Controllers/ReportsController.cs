using eLog.ViewModels.ORB1;
using eLog.ViewModels.Reports;
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
    }
}