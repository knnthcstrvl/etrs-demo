using System.ComponentModel.DataAnnotations;

namespace Financial.Model.Library
{
    public class ProcurementMode
    {
        [Key]
        public int ModeId { get; set; }

        [Required, MaxLength(100)]
        public string ModeName { get; set; }
    }
}
