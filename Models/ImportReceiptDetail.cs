using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SunriseHotelApp.Models
{
    public partial class ImportReceiptDetail
    {
        [Key]
        public int Id { get; set; }

        public int ReceiptId { get; set; } 

        public int ProductId { get; set; } 

        public decimal ImportPrice { get; set; }

        public int Quantity { get; set; }

        public decimal SubTotal { get; set; }

        [ForeignKey("ReceiptId")]
        public virtual ImportReceipt? ImportReceipt { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }
    }
}