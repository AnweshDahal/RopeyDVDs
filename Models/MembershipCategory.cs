namespace RopeyDVDs.Models
{
    public class MembershipCategory
    {
        public int Id { get; set; }
        public string? MembershipCategoryDescription{ get; set; }
        public int? MembershipCategoryTotalLoans { get; set; }
        public ICollection<Member> Members { get; set; }
    }
}
