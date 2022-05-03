using System.ComponentModel.DataAnnotations;

namespace RopeyDVDs.Models
{
    public class DVDCategory
    {
        [Key]
        public int Id { get; set; }
        public string? CategoryDescription { get; set; }
        public bool AgeRestricted { get; set; }
        public virtual ICollection<DVDTitle>? DVDTitles { get; set; }
    }
}
