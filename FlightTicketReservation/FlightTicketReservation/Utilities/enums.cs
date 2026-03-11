namespace FlightTicketReservation.Utilities
{
    public enum PaymentMethod
    {
        CreditCard = 1,
        DebitCard = 2,
        PayPal = 3,
        BankTransfer = 4,
        DigitalWallet = 5,
        LoyaltyPoints = 6
    }
    public enum PaymentStatus
    {
        Pending = 1,
        Completed = 2,
        Failed = 3,
        Refunded = 4,
        Cancelled = 5
    }
    public enum TripType
    {
        OneWay = 1,
        RoundTrip = 2,
        MultiCity = 3
    }
    public enum FlightStatus
    {
        Scheduled = 1,   // Flight is planned and not started yet
        Boarding = 2,    // Passengers are boarding the aircraft
        Departed = 3,    // Flight has taken off
        InAir = 4,       // Flight is currently flying
        Landed = 5,      // Flight has arrived at the destination
        Delayed = 6,     // Flight departure is delayed
        Cancelled = 7    // Flight has been cancelled
    }
    public enum SeatClass
    {
        Economy = 1,
        Business = 2,
        First = 3
    }
}
