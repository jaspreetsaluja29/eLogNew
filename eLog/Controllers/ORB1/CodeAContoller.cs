using eLog.Models.ORB1;
using eLog.ViewModels.ORB1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace eLog.Controllers.ORB1
{
    public class CodeAController : Controller
    {
        private readonly DatabaseHelper _db;

        public CodeAController(DatabaseHelper db)
        {
            _db = db;
        }

        // Fetch data using stored procedure
        public async Task<IActionResult> GetCodeAData()
        {
            List<CodeAViewModel> records = new List<CodeAViewModel>();
            using (SqlConnection connection = _db.CreateConnection()) // Ensure this method exists in DatabaseHelper
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("proc_GetORB1_CodeA", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            records.Add(new CodeAViewModel
                            {
                                Id = reader.GetInt32(0),
                                EnteredBy = reader.GetString(1),
                                EntryDate = reader.GetDateTime(2),
                                BallastingOrCleaning = reader.GetString(3),
                                LastCleaningDate = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4),
                                OilCommercialName = reader.IsDBNull(5) ? null : reader.GetString(5),
                                DensityViscosity = reader.IsDBNull(6) ? null : reader.GetString(6), // Added missing field
                                IdentityOfTanksBallasted = reader.IsDBNull(7) ? null : reader.GetString(7),
                                CleanedLastContainedOil = reader.IsDBNull(8) ? (bool?)null : reader.GetBoolean(8),
                                PreviousOilType = reader.IsDBNull(9) ? null : reader.GetString(9),
                                QuantityBallast = reader.IsDBNull(10) ? (decimal?)null : reader.GetDecimal(10),
                                StartCleaningTime = reader.IsDBNull(11) ? (TimeSpan?)null : reader.GetTimeSpan(11),
                                PositionStart = reader.IsDBNull(12) ? null : reader.GetString(12),
                                StopCleaningTime = reader.IsDBNull(13) ? (TimeSpan?)null : reader.GetTimeSpan(13),
                                PositionStop = reader.IsDBNull(14) ? null : reader.GetString(14),
                                IdentifyTanks = reader.IsDBNull(15) ? null : reader.GetString(15),
                                MethodCleaning = reader.IsDBNull(16) ? null : reader.GetString(16),
                                ChemicalType = reader.IsDBNull(17) ? null : reader.GetString(17),
                                ChemicalQuantity = reader.IsDBNull(18) ? (decimal?)null : reader.GetDecimal(18),
                                StartBallastingTime = reader.IsDBNull(19) ? (TimeSpan?)null : reader.GetTimeSpan(19),
                                BallastingPositionStart = reader.IsDBNull(20) ? null : reader.GetString(20),
                                CompletionBallastingTime = reader.IsDBNull(21) ? (TimeSpan?)null : reader.GetTimeSpan(21),
                                BallastingPositionCompletion = reader.IsDBNull(22) ? null : reader.GetString(22),
                                StatusName = reader.GetString(23),
                                ApprovedBy = reader.GetString(24),
                                Comments = reader.IsDBNull(25) ? null : reader.GetString(25) // Ensure DBNull handling for Comments
                            });
                        }
                    }
                }
            }
            return View("~/Views/ORB1/CodeA.cshtml", records);
        }


        // Data Entry Page
        public IActionResult DataEntry_CodeA()
        {
            return View("~/Views/ORB1/DataEntry_CodeA.cshtml", new CodeAModel());
        }

        [Route("ORB1/CodeA/Create")]
        [HttpPost]
        public IActionResult Create([FromBody] CodeAModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data received.");
            }

            try
            {
                string storedProcedure = "proc_InsertORB1CodeA";
                var parameters = new SqlParameter[]
                {
            new SqlParameter("@UserId", model.UserId),
            new SqlParameter("@EntryDate", model.EntryDate),
            new SqlParameter("@BallastingOrCleaning", model.BallastingOrCleaning),
            new SqlParameter("@LastCleaningDate", (object)model.LastCleaningDate ?? DBNull.Value),
            new SqlParameter("@OilCommercialName", (object)model.OilCommercialName ?? DBNull.Value),
            new SqlParameter("@DensityViscosity", (object)model.DensityViscosity ?? DBNull.Value),
            new SqlParameter("@IdentityOfTanksBallasted", (object)model.IdentityOfTanksBallasted ?? DBNull.Value),
            new SqlParameter("@CleanedLastContainedOil", model.CleanedLastContainedOil),
            new SqlParameter("@PreviousOilType", (object)model.PreviousOilType ?? DBNull.Value),
            new SqlParameter("@QuantityBallast", model.QuantityBallast ?? (object)DBNull.Value),
            new SqlParameter("@StartCleaningTime", (object)model.StartCleaningTime ?? DBNull.Value),
            new SqlParameter("@PositionStart", (object)model.PositionStart ?? DBNull.Value),
            new SqlParameter("@StopCleaningTime", (object)model.StopCleaningTime ?? DBNull.Value),
            new SqlParameter("@PositionStop", (object)model.PositionStop ?? DBNull.Value),
            new SqlParameter("@IdentifyTanks", (object)model.IdentifyTanks ?? DBNull.Value),
            new SqlParameter("@MethodCleaning", (object)model.MethodCleaning ?? DBNull.Value),
            new SqlParameter("@ChemicalType", (object)model.ChemicalType ?? DBNull.Value),
            new SqlParameter("@ChemicalQuantity", model.ChemicalQuantity ?? (object)DBNull.Value),
            new SqlParameter("@StartBallastingTime", (object)model.StartBallastingTime ?? DBNull.Value),
            new SqlParameter("@BallastingPositionStart", (object)model.BallastingPositionStart ?? DBNull.Value),
            new SqlParameter("@CompletionBallastingTime", (object)model.CompletionBallastingTime ?? DBNull.Value),
            new SqlParameter("@BallastingPositionCompletion", (object)model.BallastingPositionCompletion ?? DBNull.Value),
            new SqlParameter("@RecordEntryDateTime", DateTime.UtcNow),
            new SqlParameter("@RecordLastModifiedDateTime", DateTime.UtcNow),
                };

                _db.ExecuteNonQuery(storedProcedure, CommandType.StoredProcedure, parameters);

                return Json(new { success = true, message = "Data inserted successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}