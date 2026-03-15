using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SunriseHotelApp.Models
{
    public partial class ImportReceipt
    {
        [Key]
        public int ReceiptId { get; set; }

        public int? SupplierId { get; set; }

        public int? CreatedByUserId { get; set; } 

        public DateTime ImportDate { get; set; } = DateTime.Now;

        public decimal TotalAmount { get; set; }

        [ForeignKey("CreatedByUserId")]
        public virtual SystemUser? CreatedByUser { get; set; }

        [ForeignKey("SupplierId")]
        public virtual Supplier? Supplier { get; set; }

        public virtual ICollection<ImportReceiptDetail> ImportReceiptDetails { get; set; } = new List<ImportReceiptDetail>();
    }
}