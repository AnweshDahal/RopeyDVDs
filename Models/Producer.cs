namespace RopeyDVDs.Models
{
    public class Producer: BaseModel
    {
        public int Id { get; set; }
        public string ProducerName { get; set; }
        public ICollection<DVDTitle> DVDTitles { get; set; }
    }
}
