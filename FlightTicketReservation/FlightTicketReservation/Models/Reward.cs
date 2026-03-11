namespace FlightTicketReservation.Models
{
    public class Reward
    {
        public int Id { get; set; }
        public int Points { get; set; }
        public DateTime TransactionTime {  get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
