using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace eLog.Models.ORB1
{
    public class CodeCModel
    {
        public int UserId { get; set; }
        public DateTime EntryDate { get; set; }
        public string CollectionType { get; set; }
        public string WeeklyIdentityOfTanks { get; set; }
        public decimal? WeeklyCapacityOfTanks { get; set; }
        public decimal? WeeklyTotalQuantityOfRetention { get; set; }
        public string CollectionIdentityOfTanks { get; set; }
        public decimal? CollectionCapacityOfTanks { get; set; }
        public decimal? CollectionTotalQuantityOfRetention { get; set; }
        public decimal? CollectionManualResidueQuantity { get; set; }
        public string CollectionCollectedFromTank { get; set; }
        public string TransferOperationType { get; set; }
        public decimal? TransferQuantity { get; set; }
        public string TransferTanksFrom { get; set; }
        public string TransferRetainedIn { get; set; }
        public string TransferTanksTo { get; set; }
        public string IncineratorOperationType { get; set; }
        public decimal? IncineratorQuantity { get; set; }
        public string IncineratorTanksFrom { get; set; }
        public decimal? IncineratorTotalRetainedContent { get; set; }
        public TimeSpan? IncineratorStartTime { get; set; }
        public TimeSpan? IncineratorStopTime { get; set; }
        public decimal? IncineratorTotalOperationTime { get; set; }
        public decimal? DisposalShipQuantity { get; set; }
        public string DisposalShipTanksFrom { get; set; }
        public string DisposalShipRetainedIn { get; set; }
        public string DisposalShipTanksTo { get; set; }
        public string DisposalShipRetainedTo { get; set; }
        public decimal? DisposalShoreQuantity { get; set; }
        public string DisposalShoreTanksFrom { get; set; }
        public string DisposalShoreRetainedInDischargeTanks { get; set; }
        public string DisposalShoreBargeName { get; set; }
        public string DisposalShoreReceptionFacility { get; set; }
        public string DisposalShoreReceiptNo { get; set; }
    }
}
