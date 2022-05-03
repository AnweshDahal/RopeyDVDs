namespace RopeyDVDs.Models
{
    public class Actor
    {
        public Actor()
        {
            this.DVDTitles = new HashSet<DVDTitle>();
        }

        public int Id { get; set; }
        public string ActorSurName { get; set; }
        public string ActorFirstName { get; set; }

        public virtual ICollection<DVDTitle> DVDTitles { get; set; }
    }
}
