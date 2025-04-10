    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

namespace eLog.Models.ORB1
{
    public class CodeEModel
    {
        public int UserId { get; set; }
        public DateTime EntryDate { get; set; }
        public string AutomaticDischargeType { get; set; }
        public string OverboardPositionShipStart { get; set; }
        public TimeSpan? OverboardTimeSwitching { get; set; }
        public TimeSpan? TransferTimeSwitching { get; set; }
        public string TransferTankfrom { get; set; }
        public string TransferTankTo { get; set; }
        public TimeSpan? TimeBackToManual { get; set; }
    }
}
