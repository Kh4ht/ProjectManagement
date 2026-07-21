using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Models.ViewModels;

public class EditProfileViewModel
{
    [Required]
    [StringLength(30)]
    public required string Username { get; set; }

    [Required]
    [EmailAddress]
    public required string Email { get; set; }
}