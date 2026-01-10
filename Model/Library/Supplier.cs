using System.ComponentModel.DataAnnotations;

namespace Financial.Model.Library
{
    public class Supplier
    {
        [Key]
        public int SupplierId { get; set; }


        [Required, MaxLength(200)]
        public string SupplierName { get; set; }

        public ICollection<Transaction> Transactions { get; set; }
    }
}
