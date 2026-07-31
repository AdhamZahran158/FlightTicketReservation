using System;

namespace ZahrawyAirFly.Shared.Base
{
    public abstract class BaseDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
