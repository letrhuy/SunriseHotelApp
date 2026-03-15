    using System.ComponentModel.DataAnnotations;

    namespace SunriseHotelApp.Models
    {
        public class SystemUser
        {
            [Key]
            public int UserId { get; set; }

            [Required(ErrorMessage = "Vui lòng nhập tài khoản")]
            public string Username { get; set; }

            [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
            public string PasswordHash { get; set; } // Nên mã hóa MD5/BCrypt

            [Required(ErrorMessage = "Vui lòng nhập họ tên")]
            public string FullName { get; set; }

            public string? PhoneNumber { get; set; }

            public string UserRole { get; set; } // Admin, Manager, Receptionist

            public bool IsActive { get; set; } = true;

            public virtual ICollection<ImportReceipt>? ImportReceipts { get; set; }
        }
    }