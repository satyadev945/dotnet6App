using System.ComponentModel.DataAnnotations;

namespace SampleDotNet6App.Models
{
    /// <summary>
    /// User entity
    /// </summary>
    public class User
    {
        /// <summary>
        /// User ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Email address
        /// </summary>
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Password hash
        /// </summary>
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// First name
        /// </summary>
        [StringLength(50)]
        public string? FirstName { get; set; }

        /// <summary>
        /// Last name
        /// </summary>
        [StringLength(50)]
        public string? LastName { get; set; }

        /// <summary>
        /// User role
        /// </summary>
        [Required]
        [StringLength(20)]
        public string Role { get; set; } = "User";

        /// <summary>
        /// Is user active
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Created date
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Last login date
        /// </summary>
        public DateTime? LastLoginAt { get; set; }
    }
}
