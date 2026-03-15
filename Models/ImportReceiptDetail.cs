using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SunriseHotelApp.Models
{
    public partial class ImportReceiptDetail
    {
        [Key]
        public int Id { get; set; }

        public int ReceiptId { get; set; } // Khóa ngoại tới Phiếu nhập

        public int ProductId { get; set; } // Khóa ngoại tới Sản phẩm

        public decimal ImportPrice { get; set; }

        public int Quantity { get; set; }

        public decimal SubTotal { get; set; }

        // --- CÁC THUỘC TÍNH ĐIỀU HƯỚNG (NAVIGATION PROPERTIES) ---
        // Thêm dòng này để sửa lỗi "does not contain a definition for ImportReceipt"
        [ForeignKey("ReceiptId")]
        public virtual ImportReceipt? ImportReceipt { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }
    }
}