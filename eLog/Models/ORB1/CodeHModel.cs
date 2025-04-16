    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

namespace eLog.Models.ORB1
{
    public class CodeHModel
    {
        public int UserId { get; set; }
        public DateTime EntryDate { get; set; }
        public string SelectType { get; set; }
        public string Port { get; set; }
        public string Location { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime StopDateTime { get; set; }
        public decimal Quantity { get; set; }
        public string Grade { get; set; }
        public string SulphurContent { get; set; }
        public string TankLoaded1 { get; set; }
        public string TankRetained1 { get; set; }
        public string TankLoaded2 { get; set; }
        public string TankRetained2 { get; set; }
        public string TankLoaded3 { get; set; }
        public string TankRetained3 { get; set; }
        public string TankLoaded4 { get; set; }
        public string TankRetained4 { get; set; }
        // Add other properties
    }
}
