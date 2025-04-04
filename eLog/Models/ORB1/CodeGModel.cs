    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

namespace eLog.Models.ORB1
{
    public class CodeGModel
    {
        public int UserId { get; set; }
        public DateTime EntryDate { get; set; }
        public TimeSpan? TimeofOccurrence { get; set; }
        public string PositionofShip { get; set; }
        public decimal? ApproxQuantity{ get; set; }
        public string TypeofOil { get; set; }
        public string Reasons { get; set; }
    }
}
