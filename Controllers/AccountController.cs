using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Data;
using ProjectManagement.Models;
using ProjectManagement.Models.ViewModels;

public class AccountController : Controller
{
    private readonly AppDbContext _context;

    public AccountController(AppDbContext context)
    {
        _context = context;
    }

    #region Register

    // Register GET action method returns the registration view to the user. It is responsible for displaying the registration form where users can input their details to create a new account.
    public IActionResult Register()
    {
        return View();
    }

    // Register POST action method handles the registration of a new user. It validates the input model, checks for existing users with the same email, hashes the password, and saves the new user to the database.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);


        var existingUser = await _context.Users
            .AnyAsync(u => u.Email == model.Email);


        if (existingUser)
        {
            ModelState.AddModelError("", "Email already exists");
            return View(model);
        }


        var user = new User
        {
            Username = model.Username,
            Email = model.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password)
        };


        _context.Users.Add(user);

        await _context.SaveChangesAsync();


        return RedirectToAction("Login");
    }

    #endregion
    #region Login

    // Login GET action method returns the login view to the user. It is responsible for displaying the login form where users can input their credentials to access their account.
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }


        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == model.Email);

        if (user == null)
        {
            ModelState.AddModelError("", "Invalid email or password");
            return View(model);
        }


        bool passwordValid = BCrypt.Net.BCrypt.Verify(
            model.Password,
            user.PasswordHash
        );


        if (!passwordValid)
        {
            ModelState.AddModelError("", "Invalid email or password");
            return View(model);
        }


        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Email, user.Email)
    };


        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme
        );


        var principal = new ClaimsPrincipal(identity);


        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal
        );


        return RedirectToAction("Index", "Home");
    }

    #endregion
    #region Logout

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        return RedirectToAction("Login");
    }

    #endregion
    #region Profile

    [Authorize]
    public IActionResult Profile()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var usernameClaim = User.FindFirstValue(ClaimTypes.Name);
        var emailClaim = User.FindFirstValue(ClaimTypes.Email);


        if (idClaim == null || usernameClaim == null || emailClaim == null)
        {
            return RedirectToAction("Login");
        }


        var model = new ProfileViewModel
        {
            Id = int.Parse(idClaim),
            Username = usernameClaim,
            Email = emailClaim
        };


        return View(model);
    }

    #endregion
    #region EditProfile

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> EditProfile()
    {
        // Retrieve the user ID from the claims of the currently authenticated user

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // Get the user from the database using the retrieved user ID
        // Note: claims only update when the user logs in, so if the user changes their username or email, the claims won't reflect those changes until they log in again. To get the most up-to-date information, we fetch the user from the database.

        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            return NotFound();
        }

        var model = new EditProfileViewModel
        {
            Username = user.Username,
            Email = user.Email
        };

        return View(model);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProfile(EditProfileViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var user = await _context.Users.FindAsync(userId);

        if (user == null)
            return NotFound();

        bool emailExists = await _context.Users.AnyAsync(u =>
            u.Email == model.Email &&
            u.Id != userId);

        if (emailExists)
        {
            ModelState.AddModelError(nameof(model.Email), "Email already exists.");
            return View(model);
        }

        user.Username = model.Username;
        user.Email = model.Email;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Profile));
    }

    #endregion
    #region Settings

    [Authorize]
    public IActionResult Settings()
    {
        return View();
    }

    #endregion
}