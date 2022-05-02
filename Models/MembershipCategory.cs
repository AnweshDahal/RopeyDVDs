namespace RopeyDVDs.Models
{
    public class MembershipCategory: BaseModel
    {
        public int Id { get; set; }
        public string? MembersgipCategoryDescription{ get; set; }
        public int? MembershipCategoryTotalLoans { get; set; }
        public ICollection<Member> Members { get; set; }
    }
}
