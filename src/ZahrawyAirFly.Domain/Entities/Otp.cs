using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZahrawyAirFly.Shared.Base;

namespace ZahrawyAirFly.Domain.Entities
{
    public class Otp : BaseEntity
    {
        public string OTP {  get; set; }
        public string TenantId { get; set; }
        public Tenant Tenant { get; set; }  
        public DateTime ValidTo { get; set; } = DateTime.UtcNow.AddHours(2);
        public bool IsValid { get; set; }
    }
}
