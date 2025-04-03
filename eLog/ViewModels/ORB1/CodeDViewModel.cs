namespace eLog.ViewModels.ORB1
{
    public class CodeDViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime EntryDate { get; set; }
        public string MethodDischargeTransferDisposal { get; set; }
        public decimal? EquipmentQuantity { get; set; }
        public string EquipmentResidue { get; set; }
        public string EquipmentTransferredFrom { get; set; }
        public decimal? EquipmentQuantityRetained { get; set; }
        public TimeSpan? EquipmentStartTime { get; set; }
        public string EquipmentPositionStart { get; set; }
        public TimeSpan? EquipmentStopTime { get; set; }
        public string EquipmentPositionStop { get; set; }
        public decimal? ReceptionQuantity { get; set; }
        public string ReceptionResidue { get; set; }
        public string ReceptionTransferredFrom { get; set; }
        public decimal? ReceptionQuantityRetained { get; set; }
        public TimeSpan? ReceptionStartTime { get; set; }
        public TimeSpan? ReceptionStopTime { get; set; }
        public string ReceptionPortFacilities { get; set; }
        public string ReceptionReceiptNo { get; set; }
        public string SlopTransferredTo { get; set; }
        public decimal? SlopQuantity { get; set; }
        public string SlopResidue { get; set; }
        public string SlopTransferredFrom { get; set; }
        public decimal? SlopQuantityRetainedFrom { get; set; }
        public TimeSpan? SlopStartTime { get; set; }
        public TimeSpan? SlopStopTime { get; set; }
        public decimal? SlopQuantityRetainedTo { get; set; }
        public string StatusName { get; set; }
        public string ApprovedBy { get; set; }
        public string Comments { get; set; }
    }
}