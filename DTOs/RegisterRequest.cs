using System.ComponentModel.DataAnnotations;

namespace SampleDotNet6App.DTOs;

/// <summary>
/// User registration request DTO
/// </summary>
public class RegisterRequest
{
    /// <summary>
    /// Username
    /// </summary>
    [Required(ErrorMessage = "Username is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 100 characters")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Email address
    /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [StringLength(200, ErrorMessage = "Email must not exceed 200 characters")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Password
    /// </summary>
    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// First name
    /// </summary>
    [StringLength(100, ErrorMessage = "First name must not exceed 100 characters")]
    public string? FirstName { get; set; }

    /// <summary>
    /// Last name
    /// </summary>
    [StringLength(100, ErrorMessage = "Last name must not exceed 100 characters")]
    public string? LastName { get; set; }
}
