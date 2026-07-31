using ZahrawyAirFly.Domain.Enums;
using ZahrawyAirFly.Shared.Base;

namespace ZahrawyAirFly.Domain.Entities
{
    public class TaxFee : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public TaxFeeType Type { get; set; }
        public decimal Value { get; set; }
        public bool IsPercentage { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
