using System.ComponentModel.DataAnnotations.Schema;

namespace RopeyDVDs.Models
{
    public class Loan
    {
        public int Id { get; set; }

        //fk refrences LoanType
        [ForeignKey("LoanType")]
        public int LoanTypeNumber { get; set; } 
        public virtual LoanType? LoanType { get; set; }

        //fk refrences DVDCopy
        [ForeignKey("DVDCopy")]
        public int CopyNumber { get; set; }
        public virtual DVDCopy? DVDCopy { get; set; }

        //fk refrences Member
        [ForeignKey("Member")]
        public int MemberNumber { get; set; }
        public virtual Member? Member { get; set; }

        public DateTime DateOut { get; set; }
        public DateTime DateDue { get; set; }
        public DateTime DateReturned { get; set; }


        

    }
}
