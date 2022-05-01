using System.ComponentModel.DataAnnotations.Schema;

namespace RopeyDVDs.Models
{
    public class DVDCopy
    {
        public int Id { get; set; }

        [ForeignKey("DVDTitle")]
        public int DVDNumber { get; set; } //fk refrences DVDTitle
        public DVDTitle DVDTitle { get; set; }

        public DateTime DatePurchased { get; set; }

        public ICollection<Loan> Loans { get; set; }
    }
}
