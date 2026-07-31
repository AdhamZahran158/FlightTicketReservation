using System;
using ZahrawyAirFly.Domain.Enums;
using ZahrawyAirFly.Shared.Base;

namespace ZahrawyAirFly.Domain.Entities
{
    public class Notification : BaseEntity
    {
        public string TenantId { get; set; } = string.Empty;
        public virtual Tenant Tenant { get; set; } = null!;

        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public bool IsRead { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}