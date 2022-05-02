namespace RopeyDVDs.Models
{
    public class DVDCategory: BaseModel
    {
        public int Id { get; set; }
        public string CategoryDescription { get; set; }
        public bool AgeRestricted { get; set; }
        public ICollection<DVDTitle> DVDTitles { get; set; }
    }
}
