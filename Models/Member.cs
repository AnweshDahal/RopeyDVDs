namespace RopeyDVDs.Models
{
    public class Member
    {
        public int Id { get; set; }
        public string? MemberLastName { get; set; }
        public string? MemberFirstName { get; set; }
        public string? MemberAddress { get; set; }
        public DateTime MemberDOB { get; set; }
        //foreign key references MemberCategory
        public int MembershipCategoryNumber { get; set; }
    }
}
