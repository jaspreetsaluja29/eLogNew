using eLog.ViewModels.ORB1;
using eLog.ViewModels.Reports;
using Microsoft.AspNetCore.Mvc;
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
            List<LatestInventoryTanks> records = new List<LatestInventoryTanks>();
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
                            records.Add(new LatestInventoryTanks
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

    }
}
