using System.ComponentModel.DataAnnotations;

namespace RopeyDVDs.Models
{
    public class Studio
    {
        [Key]
        public int ID { get; set; }
        public string? StudioName { get; set; }

        public virtual ICollection<DVDTitle>? DVDTitles { get; set; }
    }
}
