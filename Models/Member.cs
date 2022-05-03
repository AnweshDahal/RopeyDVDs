using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RopeyDVDs.Models
{
    public class Member
    {
        [Key]
        public int Id { get; set; }
        public string? MemberLastName { get; set; }
        public string? MemberFirstName { get; set; }
        public string? MemberAddress { get; set; }
        public DateTime MemberDOB { get; set; }


        //foreign key references MemberCategory
        [ForeignKey("MembershipCategory")]
        public int MembershipCategoryNumber { get; set; }
        public virtual MembershipCategory? MembershipCategory { get; set; }

        public virtual ICollection<Loan>? Loans { get; set; }
    }
}
