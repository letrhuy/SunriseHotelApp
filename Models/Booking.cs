using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SunriseHotelApp.Models;

public partial class Booking
{
    [Key]
    public int BookingId { get; set; }

    public DateTime BookingDate { get; set; } = DateTime.Now;

    [Required]
    public DateTime CheckInDate { get; set; }

    [Required]
    public DateTime CheckOutDate { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TotalAmount { get; set; } = 0;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal DiscountAmount { get; set; } = 0;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal FinalAmount { get; set; } = 0;

    [StringLength(50)]
    public string? PaymentMethod { get; set; }

    [StringLength(50)]
    public string? PaymentStatus { get; set; }

    [StringLength(20)]
    public string? BookingStatus { get; set; }


    public string? Notes { get; set; } 

    public int? CustomerId { get; set; }

    public int? RoomId { get; set; }


    public virtual ICollection<BookingService> BookingServices { get; set; } = new List<BookingService>();

    [ForeignKey("CustomerId")]
    public virtual Customer? Customer { get; set; }

    [ForeignKey("RoomId")]
    public virtual Room? Room { get; set; }
}