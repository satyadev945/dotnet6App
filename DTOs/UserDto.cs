namespace SampleDotNet6App.DTOs
{
    /// <summary>
    /// Data transfer object for User responses.
    /// </summary>
    public class UserDto
    {
        /// <summary>Gets or sets the user ID.</summary>
        public int Id { get; set; }

        /// <summary>Gets or sets the username.</summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>Gets or sets the email address.</summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>Gets or sets the first name.</summary>
        public string? FirstName { get; set; }

        /// <summary>Gets or sets the last name.</summary>
        public string? LastName { get; set; }

        /// <summary>Gets or sets the user role.</summary>
        public string Role { get; set; } = "User";

        /// <summary>Gets or sets whether the user is active.</summary>
        public bool IsActive { get; set; }

        /// <summary>Gets or sets the creation date.</summary>
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Data transfer object for user login request.
    /// </summary>
    public class LoginRequestDto
    {
        /// <summary>Gets or sets the username.</summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>Gets or sets the password.</summary>
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// Data transfer object for login response containing JWT token.
    /// </summary>
    public class LoginResponseDto
    {
        /// <summary>Gets or sets the JWT token.</summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>Gets or sets the token expiry date.</summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>Gets or sets the user information.</summary>
        public UserDto? User { get; set; }
    }

    /// <summary>
    /// Data transfer object for creating a user.
    /// </summary>
    public class CreateUserDto
    {
        /// <summary>Gets or sets the username.</summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>Gets or sets the email address.</summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>Gets or sets the password.</summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>Gets or sets the first name.</summary>
        public string? FirstName { get; set; }

        /// <summary>Gets or sets the last name.</summary>
        public string? LastName { get; set; }
    }
}
