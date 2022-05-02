using System.ComponentModel.DataAnnotations.Schema;

namespace RopeyDVDs.Models
{
    public class CastMember
    {

        public int ID { get; set; }

        [ForeignKey("DVDTitle")]
        public int DVDNumber { get; set; }
        public DVDTitle DVDTitle { get; set; }


        [ForeignKey("Actor")]
        public int ActorNumber { get; set; }
        public Actor Actor { get; set; }

    }
}
