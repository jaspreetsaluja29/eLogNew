    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

namespace eLog.Models.ORB1
{
    public class CodeBModel
    {
        public int UserId { get; set; }
        public DateTime EntryDate { get; set; }
        public string IdentityOfTanks { get; set; }
        public string PositionOfShipStart { get; set; }
        public string PositionOfShipCompletion { get; set; }
        public string ShipSpeedDischarge { get; set; }
        public string MethodOfDischarge { get; set; }
        public string ReceiptNo { get; set; }
        public decimal? QuantityDischarged { get; set; }
    }
}
