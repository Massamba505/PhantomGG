﻿using System.ComponentModel.DataAnnotations;

namespace PhantomGG.API.DTOs.Auth;

/// <summary>
/// Request model for user login
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// User's email address
    /// </summary>
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's password
    /// </summary>
    [Required]
    public string Password { get; set; } = string.Empty;
}
