using System.ComponentModel.DataAnnotations.Schema;

namespace RopeyDVDs.Models
{
    public class DVDTitle
    {
        public int ID { get; set; }

        // Foreign key references Producer
        [ForeignKey("Producer")]
        public int ProducerNumber { get; set; }
        public Producer Producer { get; set; }

        // Foreign key references DVDCategory
        [ForeignKey("DVDCategory")]
        public int CategoryNumber { get; set; }
        public DVDCategory DVDCategory { get; set; }

        // Foreign key references Studio
        [ForeignKey("Studio")]
        public int StudioNumber { get; set; }
        public Studio Studio { get; set; }

        public DateTime DateReleased { get; set; }
        public float StandardCharge { get; set; }
        public float PenaltyCharge { get; set; }


        public ICollection<CastMember> CastMembers { get; set; }
        public ICollection<DVDCopy> DVDCopies { get; set; }

    }
}
