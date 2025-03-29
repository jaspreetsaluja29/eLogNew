using eLog.Models;
using eLog.Models.ORB1;
using eLog.ViewModels;
using eLog.ViewModels.ORB1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace eLog.Controllers
{
    public class ISMCompanyDetailsController : Controller
    {
        private readonly DatabaseHelper _db;

        public ISMCompanyDetailsController(DatabaseHelper db)
        {
            _db = db;
        }

        public async Task<IActionResult> GetISMCompanyDetails(int pageNumber = 1, int pageSize = 10)
        {
            List<ISMCompanyDetailsViewModel> records = new List<ISMCompanyDetailsViewModel>();
            int totalRecords = 0;

            using (SqlConnection connection = _db.CreateConnection()) // Ensure this method exists in DatabaseHelper
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("proc_GetISMCompanyDetails", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            records.Add(new ISMCompanyDetailsViewModel
                            {
                                CompanyId = reader.GetInt32(reader.GetOrdinal("CompanyId")),
                                CompanyName = reader.GetString(reader.GetOrdinal("CompanyName")), // Ensure correct column name
                                VesselName = reader.IsDBNull(reader.GetOrdinal("VesselName")) ? null : reader.GetString(reader.GetOrdinal("VesselName")),
                                IMONumber = reader.IsDBNull(reader.GetOrdinal("IMONumber")) ? 0 : reader.GetInt32(reader.GetOrdinal("IMONumber")),
                                ActiveId = reader.IsDBNull(reader.GetOrdinal("ActiveId")) ? 0 : reader.GetInt32(reader.GetOrdinal("ActiveId")),
                                Flag = reader.GetBoolean(reader.GetOrdinal("Flag")), // Ensure correct column type
                                LastEntryDate = reader.IsDBNull(reader.GetOrdinal("LastEntryDate")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("LastEntryDate")),
                                LastApprovedDate = reader.IsDBNull(reader.GetOrdinal("LastApprovedDate")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("LastApprovedDate")),
                                SubscriptionStartDate = reader.IsDBNull(reader.GetOrdinal("SubscriptionStartDate")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("SubscriptionStartDate")),
                                TotalActiveeLog = reader.IsDBNull(reader.GetOrdinal("TotalActiveeLog")) ? 0 : reader.GetInt32(reader.GetOrdinal("TotalActiveeLog"))
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

            return View("~/Views/SuperAdmin/ISMCompanyDetails.cshtml", paginatedRecords);
        }

        // Data Entry Page
        public IActionResult DataEntry_ISMCompanyDetails(string userId, string userName, string userRoleName)
        {
            ViewBag.UserID = userId;
            ViewBag.UserName = userName;
            ViewBag.UserRoleName = userRoleName;
            return View("~/Views/SuperAdmin/DataEntry_ISMCompanyDetails.cshtml", new CodeAModel());
        }

        [Route("ISMCompanyDetails/Create")]
        [HttpPost]
        public IActionResult Create([FromBody] ISMCompanyDetails model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data received.");
            }

            try
            {
                string storedProcedure = "proc_InsertISMCompanyDetails";
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@CompanyName", model.CompanyName),
                    new SqlParameter("@ManagerName", model.ManagerName),
                    new SqlParameter("@OwnerName", model.OwnerName),
                    new SqlParameter("@Address", model.Address),
                    new SqlParameter("@Email", model.Email),
                    new SqlParameter("@ContactNumber", model.ContactNumber),
                    new SqlParameter("@PICDetails", model.PICDetails),
                    new SqlParameter("@PilotProjectStartDate", model.PilotProjectStartDate),
                    new SqlParameter("@VesselName", (object)model.VesselName ?? DBNull.Value),
                    new SqlParameter("@IMONumber", (object)model.IMONumber ?? DBNull.Value),
                    new SqlParameter("@ActiveId", (object)model.ActiveId ?? DBNull.Value),
                    new SqlParameter("@Flag", (object)model.Flag ?? DBNull.Value),
                    new SqlParameter("@LastEntryDate", (object)model.LastEntryDate ?? DBNull.Value),
                    new SqlParameter("@LastApprovedDate", (object)model.LastApprovedDate ?? DBNull.Value),
                    new SqlParameter("@SubscriptionStartDate", (object)model.SubscriptionStartDate ?? DBNull.Value),
                    new SqlParameter("@TotalActiveeLog", (object)model.TotalActiveeLog ?? DBNull.Value),
                };

                int insertedId = _db.ExecuteInsertStoredProcedure(storedProcedure, parameters);

                if (insertedId > 0)
                {
                    return Json(new { success = true, message = "Company data inserted successfully!", insertedId = insertedId });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to insert company data." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
