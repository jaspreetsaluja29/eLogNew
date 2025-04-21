namespace eLog.ViewModels.Reports
{
    public class HistoryForTanksModel
    {
        public int SNo { get; set; }
        public DateTime? EntryDate { get; set; }
        public string? IdentityOfTanks { get; set; }
        public decimal? LastRetainedOnBoard { get; set; }
        public DateTime? ModAppRejDate { get; set; }
        public string? Status { get; set; }
    }
}
