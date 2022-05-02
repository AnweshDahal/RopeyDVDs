using System.ComponentModel.DataAnnotations.Schema;

namespace RopeyDVDs.Models
{
    public class Member: BaseModel
    {
        public int Id { get; set; }
        public string? MemberLastName { get; set; }
        public string? MemberFirstName { get; set; }
        public string? MemberAddress { get; set; }
        public DateTime MemberDOB { get; set; }


        //foreign key references MemberCategory
        [ForeignKey("MembershipCategory")]
        public int MembershipCategoryNumber { get; set; }
        public MembershipCategory MembershipCategory { get; set; }

        public ICollection<Loan> Loans { get; set; }
    }
}
