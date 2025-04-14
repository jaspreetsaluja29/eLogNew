namespace eLog.ViewModels.Reports
{
    public class LatestInventoryTanks
    {
        public int SNo { get; set; }
        public DateTime? EntryDate { get; set; }
        public string? IdentityOfTanks { get; set; }
        public decimal? LastRetainedOnBoard { get; set; }
    }
}
