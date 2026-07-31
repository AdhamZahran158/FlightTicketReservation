using ZahrawyAirFly.Shared.Base;

namespace ZahrawyAirFly.Domain.Entities
{
    public class Policy : BaseEntity
    {
        public bool AllowCancellation { get; set; }
        public int CancelDeadlineHours { get; set; }
        public decimal CancelPenaltyPercent { get; set; }
        public bool AllowModification { get; set; }
        public int ModifyDeadlineHours { get; set; }
        public decimal ModifyFee { get; set; }
        public string TermsAndConditions { get; set; } = string.Empty;
    }
}
