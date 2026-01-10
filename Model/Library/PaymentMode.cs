using System.ComponentModel.DataAnnotations;

namespace Financial.Model.Library
{
    public class PaymentMode
    {
        [Key]
        public int PaymentModeId { get; set; }

        [Required, MaxLength(50)]
        public string PaymentModeName { get; set; }
    }
}
