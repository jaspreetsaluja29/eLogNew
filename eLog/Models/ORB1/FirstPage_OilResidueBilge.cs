namespace eLog.Models.ORB1
{
    public class FirstPage_OilResidueBilge
    {
        public int Id { get; set; }
        public string TankIdentification { get; set; }
        public string TankLocation_Frames_From_To { get; set; }
        public string TankLocation_LateralPosition { get; set; }
        public decimal Volume_m3 { get; set; }
    }
}
