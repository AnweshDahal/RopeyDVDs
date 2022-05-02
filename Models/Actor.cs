namespace RopeyDVDs.Models
{
    public class Actor
    {
        public int Id { get; set; }
        public string ActorSurName { get; set; }
        public string ActorFirstName { get; set; }

        public ICollection<CastMember> CastMembers { get; set; }
    }
}
