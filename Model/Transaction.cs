using Financial.Model.Library;
using Financial.Model.Modules;
using Financial.Model.Trace;
using System.ComponentModel.DataAnnotations;

namespace Financial.Model
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        [MaxLength(50)]
        public string PrNumber { get; set; }

        [MaxLength(100)]
        public string ContractNumber { get; set; }

        [MaxLength(100)]
        public string OrsNumber { get; set; }

        [MaxLength(100)]
        public string DvNumber { get; set; }

        public int? SupplierId { get; set; }
        public Supplier Supplier { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Procurement Procurement { get; set; }
        public Budget Budget { get; set; }
        public Property Property { get; set; }
        public Accounting Accounting { get; set; }
        public Cashier Cashier { get; set; }

        public ICollection<Trace.Document> Documents { get; set; }
        public ICollection<StatusHistory> StatusHistories { get; set; }
    }
}
