using System.ComponentModel.DataAnnotations.Schema;

namespace RopeyDVDs.Models
{
    public class DVDCopy
    {
        public int Id { get; set; }

        [ForeignKey("DVDTitle")]
        public int DVDNumber { get; set; } //fk refrences DVDTitle
<<<<<<< Updated upstream
        public DVDTitle DVDTitle { get; set; }

        public DateTime DatePurchased { get; set; }

        public ICollection<Loan> Loans { get; set; }
=======
        public virtual DVDTitle? DVDTitle { get; set; }
        public DateTime DatePurchased { get; set; }

        public bool IsDeleted { get; set; }
        public virtual ICollection<Loan>? Loans { get; set; }
>>>>>>> Stashed changes
    }
}
