namespace ZahrawyAirFly.Domain.Enums
{
    public enum UserRole
    {
        SuperAdmin,
        AirlineAdmin,
        Staff,
        Passenger
    }

    public enum BookingStatus
    {
        Pending,
        Confirmed,
        Modified,
        Cancelled,
        Refunded,
        NoShow
    }

    public enum FlightStatus
    {
        Scheduled,
        Boarding,
        Departed,
        Arrived,
        Cancelled,
        Delayed
    }

    public enum SeatClass
    {
        Economy,
        Business,
        First
    }

    public enum SeatStatus
    {
        Available,
        Booked,
        Blocked,
        Locked,
        Maintenance
    }

    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed,
        Refunded
    }

    public enum PaymentMethod
    {
        CreditCard,
        DebitCard,
        Wallet,
        BankTransfer
    }

    public enum NotificationType
    {
        BookingConfirmation,
        BookingCancellation,
        BookingModification,
        FlightCancellation,
        FlightDelay,
        PaymentReceipt,
        Refund,
        Welcome,
        EmailVerification,
        PasswordReset
    }

    public enum TaxFeeType
    {
        Tax,
        Fee
    }
}
