    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

namespace eLog.Models.ORB1
{
    public class CodeAModel
    {
        public int UserId { get; set; }
        public DateTime EntryDate { get; set; }
        public string BallastingOrCleaning { get; set; }
        public DateTime? LastCleaningDate { get; set; }
        public string OilCommercialName { get; set; }
        public string DensityViscosity { get; set; }
        public string IdentityOfTanksBallasted { get; set; }
        public bool CleanedLastContainedOil { get; set; }
        public string PreviousOilType { get; set; }
        public decimal? QuantityBallast { get; set; }
        public TimeSpan? StartCleaningTime { get; set; }
        public string PositionStart { get; set; }
        public TimeSpan? StopCleaningTime { get; set; }
        public string PositionStop { get; set; }
        public string IdentifyTanks { get; set; }
        public string MethodCleaning { get; set; }
        public string ChemicalType { get; set; }
        public decimal? ChemicalQuantity { get; set; }
        public TimeSpan? StartBallastingTime { get; set; }
        public string BallastingPositionStart { get; set; }
        public TimeSpan? CompletionBallastingTime { get; set; }
        public string BallastingPositionCompletion { get; set; }
        // Add other properties
    }
}
