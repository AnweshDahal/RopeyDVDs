using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RopeyDVDs.Models
{
    public class CastMember
    {
        [Key]
        public int ID { get; set; }
      
        [ForeignKey("DVDTitle")]
        public int DVDNumber { get; set; }
        public virtual DVDTitle? DVDTitle { get; set; }


        [ForeignKey("Actor")]
        public int ActorNumber { get; set; }
        public virtual Actor? Actor { get; set; }

    }
}
