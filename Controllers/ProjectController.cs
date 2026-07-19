using Microsoft.AspNetCore.Mvc;

public class ProjectController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}