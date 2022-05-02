namespace RopeyDVDs.Models
{
    public class LoanType: BaseModel
    {
        public int Id { get; set; }
        public string? Type { get; set; }
        public int? LoanDuration { get; set; }

        public ICollection<Loan> Loans { get; set; }
    }
}
