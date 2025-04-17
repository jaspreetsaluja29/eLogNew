namespace eLog.ViewModels.ORB1
{
    public class CodeIViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime EntryDate { get; set; }
        public string SelectType { get; set; }

        //Weekly Inventory
        public string WeeklyInventoryTanks { get; set; }
        public decimal? WeeklyInventoryCapacity { get; set; }
        public decimal? WeeklyInventoryRetained { get; set; }

        //Debunkering
        public decimal? DebunkeringQuantity { get; set; }
        public string DebunkeringGrade { get; set; }
        public string DebunkeringSulphurContent { get; set; }
        public string DebunkeringFrom { get; set; }
        public decimal? DebunkeringQuantityRetained { get; set; }
        public string DebunkeringTo { get; set; }
        public string DebunkeringPortFacility { get; set; }
        public DateTime? DebunkeringStartDateTime { get; set; }
        public DateTime? DebunkeringStopDateTime { get; set; }

        //Sealing of Valve
        public string ValveName { get; set; }
        public string ValveNo { get; set; }
        public string ValveAssociatedEquipment { get; set; }
        public string ValveSealNo { get; set; }

        //Breaking of Seal
        public string BreakingValveName { get; set; }
        public string BreakingValveNo { get; set; }
        public string BreakingAssociatedEquipment { get; set; }
        public string BreakingReason { get; set; }
        public string BreakingSealNo { get; set; }
        public string StatusName { get; set; }
        public string ApprovedBy { get; set; }
        public string Comments { get; set; }
    }
}