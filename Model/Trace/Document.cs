using System.ComponentModel.DataAnnotations;

namespace Financial.Model.Trace
{
    public class Document
    {
        [Key]
        public int DocumentId { get; set; }

        [Required]
        public int TransactionId { get; set; }
        public Transaction Transaction { get; set; }

        public string DocumentType { get; set; }
        public string FilePath { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
