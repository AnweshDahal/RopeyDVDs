namespace RopeyDVDs.Models
{
    public class Loan
    {
        public int Id { get; set; }
        public int LoanTypeNumber { get; set; } //fk refrences LoanType
        public int CopyNumber { get; set; } //fk refrences DVDCopy
        public int MemberNumber { get; set; } //fk refrences Member
        public DateTime DateOut { get; set; }
        public DateTime DateDue { get; set; }
        public DateTime DateReturned { get; set; }
    }
}
