using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using ZahrawyAirFly.Domain.Entities;

public class Tenant : IdentityUser
{
    public string Name { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string PassportNumber { get; set; } = string.Empty;
    public string Subdomain { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public string Currency { get; set; } = "USD";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}