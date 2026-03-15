using System;
using System.Collections.Generic;

namespace SunriseHotelApp.Models;

public partial class HotelInformation
{
    public int Id { get; set; }

    public string? HotelName { get; set; }

    public string? Address { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public string? Website { get; set; }

    public string? Description { get; set; }
}
