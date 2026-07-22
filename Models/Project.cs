using System.ComponentModel.DataAnnotations;
using ProjectManagement.Models;

namespace ProjectManagement.Models;

public class Project
{
    public int Id { get; set; }

    [StringLength(maximumLength: 30)]
    public string Name { get; set; } = string.Empty;

    [StringLength(maximumLength: 500)]
    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? DueDate { get; set; }

    public ProjectStatus Status { get; set; }

    // Foreign Key
    public int OwnerId { get; set; }

    public User Owner { get; set; } = null!;
}

public enum ProjectStatus
{
    NotStarted,
    InProgress,
    Completed,
    OnHold
}