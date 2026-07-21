using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class ProjectController : Controller
{
    [Authorize]
    public IActionResult Index()
    {
        return View();
    }
}