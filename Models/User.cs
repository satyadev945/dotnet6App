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
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Email address
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Password hash
        /// </summary>
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Is user active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Created date
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Last login date
        /// </summary>
        public DateTime? LastLoginDate { get; set; }
    }
}
