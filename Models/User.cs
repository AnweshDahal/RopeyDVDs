namespace RopeyDVDs.Models
{
    public class User: BaseModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string UserType { get; set; }
        public string Password { get; set; }
             
    }
}
