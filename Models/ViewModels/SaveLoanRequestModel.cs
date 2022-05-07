using System.ComponentModel.DataAnnotations;

namespace RopeyDVDs.Models.ViewModels
{
    public class SaveLoanRequestModel
    {
        [Key]
        public int? ID { get; set; }
        public int? DVDCopyNumber { get; set; }

        public int? MemberNumber { get; set; }

        public int? LoanTypeNumber { get; set; }
    }
}
