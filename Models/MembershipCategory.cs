using System.ComponentModel.DataAnnotations;

namespace RopeyDVDs.Models
{
    public class MembershipCategory
    {
        [Key]
        public int Id { get; set; }
        public string? MembershipCategoryDescription{ get; set; }
        public int? MembershipCategoryTotalLoans { get; set; }
        public virtual ICollection<Member>? Members { get; set; }
    }
}
