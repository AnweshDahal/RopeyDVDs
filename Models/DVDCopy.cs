using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RopeyDVDs.Models
{
    public class DVDCopy
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("DVDTitle")]
        public int DVDNumber { get; set; } //fk refrences DVDTitle
        public virtual DVDTitle? DVDTitle { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime DatePurchased { get; set; }

        public virtual ICollection<Loan>? Loans { get; set; }
    }
}
