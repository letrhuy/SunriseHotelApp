using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        public decimal UnitPrice { get; set; } // Giá bán

        public int? CurrentStock { get; set; } // Tồn kho

        public int? MinStockLevel { get; set; }
        public int? SupplierID { get; set; }
        public virtual Supplier? Supplier { get; set; }

        // --- XÓA CÁC DÒNG ICOLLECTION LIÊN QUAN ĐẾN BOOKINGSERVICE TẠI ĐÂY ---
        // Chỉ giữ lại những gì thực sự cần thiết, ví dụ như ImportReceiptDetails nếu bạn muốn thống kê nhập kho
        // Nhưng để an toàn nhất lúc này, hãy tạm ẩn hoặc xóa hết các ICollection trỏ ngược
    }
}