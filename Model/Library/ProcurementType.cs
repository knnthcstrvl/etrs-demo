using System.ComponentModel.DataAnnotations;

namespace Financial.Model.Library
{
    public class ProcurementType
    {
        [Key]
        public int TypeId { get; set; }

        [Required, MaxLength(100)]
        public string TypeName { get; set; }
    }
}
