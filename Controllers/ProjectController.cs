using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Data;
using ProjectManagement.Models;

public class ProjectController : Controller
{
    private readonly AppDbContext _context;

    public ProjectController(AppDbContext context)
    {
        _context = context;
    }

    #region Index

    [Authorize]
    public async Task<IActionResult> Index()
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );

        var projects = await _context.Projects
            .Where(p => p.OwnerId == userId)
            .ToListAsync();

        return View(projects);
    }

    #endregion
    #region Create

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Project project)
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );

        project.OwnerId = userId;
        project.CreatedAt = DateTime.UtcNow;
        project.Status = ProjectStatus.NotStarted;

        _context.Projects.Add(project);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    #endregion
}