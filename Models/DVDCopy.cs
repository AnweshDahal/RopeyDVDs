namespace RopeyDVDs.Models
{
    public class DVDCopy
    {
        public int Id { get; set; }
        public int DVDNumber { get; set; } //fk refrences DVDTitle
        public DateTime DatePurchased { get; set; }
    }
}
