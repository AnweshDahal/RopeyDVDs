namespace RopeyDVDs.Models
{
    public class DVDTitle
    {
        public int ID { get; set; }

        // Foreign key references Producer
        public int ProducerNumber { get; set; }

        // Foreign key references DVDCategory
        public int CategoryNumber { get; set; }

        // Foreign key references Studio
        public int StudioNumber { get; set; }

        public DateTime DateReleased { get; set; }
        public float StandardCharge { get; set; }
        public float PenaltyCharge { get; set; }



        // Define the relationships to other models below for foreign key
        public DVDTitle()
        {

        }
    }
}
