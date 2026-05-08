using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EColor.API.Models
{
    public class ProductionOrder
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        public string OrderNo { get; set; } = string.Empty;
 
        [Required]
        [MaxLength(100)]
        public string Vendor { get; set; } = string.Empty;
 
        [Required]
        [MaxLength(50)]
        public string OrderDate { get; set; } = string.Empty;

        [MaxLength(100)]
        public string FabricType { get; set; } = string.Empty;

        [MaxLength(50)]
        public string MaleStyleNo { get; set; } = string.Empty;

        [MaxLength(50)]
        public string FemaleStyleNo { get; set; } = string.Empty;

        [MaxLength(50)]
        public string DetailStyleNo { get; set; } = string.Empty;

        [MaxLength(50)]
        public string DeliveryDate { get; set; } = string.Empty;

        [MaxLength(50)]
        public string ThreadCode { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Composition { get; set; } = string.Empty;

        [MaxLength(50)]
        public string CuttingDate { get; set; } = string.Empty;

        public string Notes { get; set; } = string.Empty;

        // 儲存尺寸列表順序 (JSON 格式，例如 ["S", "M", "L"])
        public string SizesJson { get; set; } = "[]";

        public List<ProductionOrderItem> Items { get; set; } = new List<ProductionOrderItem>();
    }
}
