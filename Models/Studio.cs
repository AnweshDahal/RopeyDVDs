namespace RopeyDVDs.Models
{
    public class Studio
    {
        public int ID { get; set; }
        public string StudioName { get; set; }

        public ICollection<DVDTitle> DVDTitles { get; set; }
    }
}
