namespace eLog.ViewModels
{
    public class ISMCompanyDetailsViewModel
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string VesselName { get; set; }
        public int IMONumber { get; set; }
        public int ActiveId { get; set; }
        public string Flag { get; set; }
        public DateTime LastEntryDate { get; set; }
        public DateTime LastApprovedDate { get; set; }
        public DateTime SubscriptionStartDate { get; set; }
        public int TotalActiveeLog { get; set; }
    }
}
