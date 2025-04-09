namespace eLog.ViewModels.ORB1
{
    public class CodeBViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime EntryDate { get; set; }
        public string IdentityOfTanks { get; set; }
        public string PositionOfShipStart { get; set; }
        public string PositionOfShipCompletion { get; set; }
        public string ShipSpeedDischarge { get; set; }
        public string MethodOfDischarge { get; set; }
        public string ReceiptNo { get; set; }
        public decimal? QuantityDischarged { get; set; }
        public string StatusName { get; set; }
        public string ApprovedBy { get; set; }
        public string Comments { get; set; }
    }
}