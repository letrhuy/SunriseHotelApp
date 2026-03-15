using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SunriseHotelApp.Models
{
    public partial class ImportReceipt
    {
        [Key]
        public int ReceiptId { get; set; }

        public int? SupplierId { get; set; }

        public int? CreatedByUserId { get; set; } // ID của nhân viên tạo phiếu

        public DateTime ImportDate { get; set; } = DateTime.Now;

        public decimal TotalAmount { get; set; }

        // --- PHẦN BỔ SUNG ĐỂ SỬA LỖI ---

        // 1. Thuộc tính điều hướng đến nhân viên (Sửa lỗi CreatedByUser)
        [ForeignKey("CreatedByUserId")]
        public virtual SystemUser? CreatedByUser { get; set; }

        // 2. Thuộc tính điều hướng đến nhà cung cấp (Phòng hờ lỗi Supplier)
        [ForeignKey("SupplierId")]
        public virtual Supplier? Supplier { get; set; }

        // 3. Danh sách chi tiết nhập kho
        public virtual ICollection<ImportReceiptDetail> ImportReceiptDetails { get; set; } = new List<ImportReceiptDetail>();
    }
}