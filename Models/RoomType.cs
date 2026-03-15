using System.ComponentModel.DataAnnotations;

namespace SunriseHotelApp.Models
{
    public class RoomType
    {
        [Key]
        public int RoomTypeID { get; set; }

        public string RoomTypeName { get; set; }

        public decimal PricePerNight { get; set; }

        public int MaxGuests { get; set; }

        public string? Description { get; set; }

        public string? Image { get; set; }

        public virtual ICollection<Room> Rooms { get; set; }
    }
}