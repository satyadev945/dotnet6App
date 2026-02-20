namespace SampleDotNet6App.DTOs;

/// <summary>
/// Login response DTO
/// </summary>
public class LoginResponse
{
    /// <summary>
    /// JWT token
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Username
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// User role
    /// </summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// Token expiration date
    /// </summary>
    public DateTime ExpiresAt { get; set; }
}
