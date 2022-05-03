using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RopeyDVDs.Models
{
    public class DVDTitle
    {
        public DVDTitle()
        {
            this.Actor = new HashSet<Actor>();
        }
        [Key]
        public int ID { get; set; }

        // Foreign key references Producer
        [ForeignKey("Producer")]
        public int ProducerNumber { get; set; }
        public virtual Producer? Producer { get; set; }

        // Foreign key references DVDCategory
        [ForeignKey("DVDCategory")]
        public int CategoryNumber { get; set; }
        public virtual DVDCategory? DVDCategory { get; set; }

        // Foreign key references Studio
        [ForeignKey("Studio")]
        public int StudioNumber { get; set; }
        public virtual Studio? Studio { get; set; }

        public DateTime DateReleased { get; set; }
        public float StandardCharge { get; set; }
        public float PenaltyCharge { get; set; }


        public virtual ICollection<Actor>? Actor { get; set; }
        public virtual ICollection<DVDCopy>? DVDCopies { get; set; }

    }
}
