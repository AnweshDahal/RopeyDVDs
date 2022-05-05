using System.ComponentModel.DataAnnotations;

namespace RopeyDVDs.Models
{
    public class Actor
    {
        public Actor()
        {
            this.DVDTitle = new HashSet<DVDTitle>();
        }
        [Key]
        public int Id { get; set; }
        public string ActorSurName { get; set; }
        public string ActorFirstName { get; set; }

        public virtual ICollection<DVDTitle>? DVDTitle { get; set; }
    }
}
