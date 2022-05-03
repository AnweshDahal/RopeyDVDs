using System.ComponentModel.DataAnnotations;

namespace RopeyDVDs.Models
{
    public class Producer
    {
        [Key]
        public int Id { get; set; }
        public string? ProducerName { get; set; }
        public virtual ICollection<DVDTitle>? DVDTitles { get; set; }
    }
}
