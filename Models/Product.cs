using System.ComponentModel.DataAnnotations;

namespace SunriseHotelApp.Models
{
    public partial class Product
    {
        [Key]
        public int ProductId { get; set; }

        [StringLength(100)]
        public string ProductName { get; set; } = null!;

        [StringLength(20)]
        public string? Unit { get; set; }

        public decimal UnitPrice { get; set; } 

        public int? CurrentStock { get; set; } 

        public int? MinStockLevel { get; set; }
        public int? SupplierID { get; set; }
        public virtual Supplier? Supplier { get; set; }

    }
}