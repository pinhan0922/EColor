using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EColor.API.Models
{
    public class ProductionOrderItem
    {
        [Key]
        public int Id { get; set; }

        public int ProductionOrderId { get; set; }

        [ForeignKey("ProductionOrderId")]
        public ProductionOrder? ProductOrder { get; set; }

        [MaxLength(20)]
        public string Category { get; set; } = string.Empty; // "order", "cut"

        [MaxLength(20)]
        public string Gender { get; set; } = string.Empty; // "male", "female"

        [MaxLength(20)]
        public string Size { get; set; } = string.Empty; // "S", "M", etc.

        public int? Quantity { get; set; }
    }
}
