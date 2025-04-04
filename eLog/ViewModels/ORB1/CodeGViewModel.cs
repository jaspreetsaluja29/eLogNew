namespace eLog.ViewModels.ORB1
{
    public class CodeGViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime EntryDate { get; set; }
        public TimeSpan? TimeofOccurrence { get; set; }
        public string PositionofShip { get; set; }
        public decimal? ApproxQuantity { get; set; }
        public string TypeofOil { get; set; }
        public string Reasons { get; set; }
        public string StatusName { get; set; }
        public string ApprovedBy { get; set; }
        public string Comments { get; set; }
    }
}