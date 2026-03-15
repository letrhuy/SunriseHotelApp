using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SunriseHotelApp.Models
{
    public partial class BookingService
    {
        [Key]
        public int BookingServiceID { get; set; }

        public int BookingId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal PriceAtOrder { get; set; }

        public DateTime OrderDate { get; set; }

        public virtual Booking Booking { get; set; }
        public virtual Product Product { get; set; }
    }
}