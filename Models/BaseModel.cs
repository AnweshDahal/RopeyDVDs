using System.ComponentModel.DataAnnotations.Schema;

namespace RopeyDVDs.Models
{
    public class BaseModel: IBaseModel
    {
        // Is Deleted
        [Column(Order = 999)]
        public bool IsDeleted { get; set; }


    }
}
