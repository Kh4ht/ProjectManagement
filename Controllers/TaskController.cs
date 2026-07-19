using Microsoft.AspNetCore.Mvc;

public class TaskController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}