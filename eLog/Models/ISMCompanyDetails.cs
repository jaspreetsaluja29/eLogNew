namespace eLog.Models
{
    public class ISMCompanyDetails
    {
        public int Companyid { get; set; }
        public string CompanyName { get; set; }
        public string ManagerName { get; set; }
        public string OwnerName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public string PICDetails { get; set; }
        public DateTime PilotProjectStartDate { get; set; }
        public string VesselName { get; set; }
        public int IMONumber { get; set; }
        public int ActiveId { get; set; }
        public bool Flag { get; set; }
        public DateTime LastEntryDate { get; set; }
        public DateTime LastApprovedDate { get; set; }
        public DateTime SubscriptionStartDate { get; set; }
        public int TotalActiveeLog { get; set; }
    }
}
