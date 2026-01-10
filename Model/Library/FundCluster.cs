using System.ComponentModel.DataAnnotations;

namespace Financial.Model.Library
{
    public class FundCluster
    {
        [Key]
        public int FundClusterId { get; set; }

        [Required, MaxLength(50)]
        public string FundClusterName { get; set; }
    }
}
