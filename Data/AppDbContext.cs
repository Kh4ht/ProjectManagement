using Microsoft.EntityFrameworkCore;
using ProjectManagement.Models;

namespace ProjectManagement.Data;

// Represents the Entity Framework database context for the application.
// This class is responsible for configuring the database model and exposing
// the entity sets used by the application.
public class AppDbContext : DbContext
{
    // Initializes a new instance of the "AppDbContext" class.
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // A DbSet<T> represents a table in the database, Can add more DbSet properties for other entities as needed.
    public DbSet<User> Users => Set<User>();
}

// To apply the changes to the database, you can use the following command in the terminal:
/*
dotnet ef migrations add ChangeDesiredNameForYourMigration
dotnet ef database update
*/

// To verify the changes in the database, you can use the following command in the terminal:
/*
/Library/PostgreSQL/18/bin/psql -U postgres -d ProjectManagementDB
\d "Users"
*/