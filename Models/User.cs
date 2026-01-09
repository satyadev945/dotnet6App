namespace SampleDotNet6App.Models;

/// <summary>
/// Represents a user in the system
/// </summary>
public class User
{
    /// <summary>
    /// Gets or sets the user identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the username
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password hash
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the first name
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Gets or sets the last name
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Gets or sets the date when the user was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets whether the user is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the user role
    /// </summary>
    public string Role { get; set; } = "User";
}
