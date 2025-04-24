namespace eLog.ViewModels.Reports
{
    public class BunkeringDataModel
    {
        public int SNo { get; set; }
        public string? Tanks { get; set; }
        public decimal? Capacity { get; set; }
        public DateTime? EntryDate { get; set; }
        public decimal? Quantity { get; set; }
        public string? Grade { get; set; }
        public string? SulphurContent { get; set; }
        public string? Port { get; set; }
    }
}
