using System.ComponentModel.DataAnnotations;

namespace SunriseHotelApp.Models
{
    public class RoomType
    {
        [Key]
        // SỬA LỖI 1: Viết đúng RoomTypeID (ID viết hoa) để khớp với Code cũ
        public int RoomTypeID { get; set; }

        public string RoomTypeName { get; set; }

        public decimal PricePerNight { get; set; }

        // Các cột mới thêm
        public int MaxGuests { get; set; }
        public string? Description { get; set; }

        // SỬA LỖI 2: Thêm thuộc tính Image (Tên phải giống hệt cột trong SQL)
        public string? Image { get; set; }

        public virtual ICollection<Room> Rooms { get; set; }
    }
}