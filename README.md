# ASP.NET MVC Project Setup in VSCode

Follow this guide to create a new ASP.NET MVC project using VSCode. We'll also use the ASPNET CodeGenerator package and configure the Entity Framework to use MySQL.

## Prerequisites

- [VSCode](https://code.visualstudio.com/)
- [.NET SDK](https://dotnet.microsoft.com/download)
- MySQL Server installed and running

## Steps

### 1. Initialize a New Project

Open your terminal and create a new directory for your project. Navigate into it and run the following command to create a new ASP.NET MVC project:

```bash
dotnet new mvc -o NameOfMyProject
dotnet dev-certs https --trust
```

---
### 2. Add MySQL Entity Framework Package

Add the NuGet package for Entity Framework Core MySQL provider.

```bash
dotnet tool uninstall --global dotnet-ef
dotnet tool install --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.SQLite
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package MySql.EntityFrameworkCore --version 7.0.2
```

---
### 3. Add ASPNET CodeGenerator

Install the ASPNET CodeGenerator package:

```bash
dotnet tool uninstall --global dotnet-aspnet-codegenerator
dotnet tool install --global dotnet-aspnet-codegenerator

```

---
### 4. Create Models

Create your model classes in the `Models` folder, using the code generator:
```bash
dotnet aspnet-codegenerator model -n Department -o Models
```

---
### 5. Add DbContext

Create a new folder called **Data**. In the folder create a new file called **ApplicationDbContext.cs**. Add the following to the file:
```csharp
using Microsoft.EntityFrameworkCore;

namespace WorldDominion.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        
        // Change to be your model(s) and table(s)
        public DbSet<ModelName> TableName { get; set; }
    }
}
```

In the Program.cs file, add the following line, after the **"// Add services to the container."** comment:
```csharp
// Add MySQL
var connectionString = builder.Configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("Connection string not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseMySQL(connectionString));
```

---
### 6. Scaffold Controllers and Views

Using the terminal, run the following command to scaffold controllers and views:

```bash
dotnet aspnet-codegenerator controller -name [ControllerName] -m [ModelName] -dc [DbContextName] --relativeFolderPath Controllers --useDefaultLayout --referenceScriptLibraries
```

Replace `[ControllerName]`, `[ModelName]`, and `[DbContextName]` with appropriate names.

---
### 7. Run Migrations

Initialize a new migration:

```bash
dotnet ef migrations add InitialMigration
```

Apply the migration to the database:

```bash
dotnet ef database update
```

---
### 8. Run the Project

Run your project to make sure everything works:

```bash
dotnet run
```

---
### 9. Adding Identity

Adding Authentication and Authorization to an existing MVC project in ASP.NET Core involves several steps. Below is a simplified step-by-step guide to achieve this, assuming you're using ASP.NET Core Identity for authentication and authorization:

1. **Install Necessary Packages**:
   - Ensure you have the necessary NuGet packages installed for ASP.NET Core Identity.

```bash
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add package Microsoft.AspNetCore.Identity.UI
```

2. **Update Program.cs**:
   - Register the identity services and the `ApplicationDbContext` in the `ConfigureServices` method.
   - Ensure authentication and authorization middleware are added in the `Configure` method.

```csharp
// Adding identity service and roles
builder.Services.AddDefaultIdentity<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
```

```csharp
// Setup authentication and authorization
app.UseAuthentication();
app.UseAuthorization();
```

3. **Update DbContext**:
   - Ensure your `ApplicationDbContext` class extends `IdentityDbContext`.

```csharp
public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    public DbSet<Department> Departments { get; set; }
    public DbSet<Product> Products { get; set; }
}
```

4. **Scaffold the Views**

```bash
dotnet aspnet-codegenerator identity -dc WorldDominion.Models.ApplicationDbContext --files "Account.Register;Account.Login;Account.Logout"
```

5. **Register Razor Pages Middleware**

```csharp
app.MapRazorPages();
```

6. **Run Migrations**:
   - Create a migration to apply the identity schema to your database.

```bash
dotnet ef migrations add CreateIdentitySchema
dotnet ef database update
```

---
### 10. Adding a Seeding File for a Default Admin Role

Create a new file in the Data folder called **DbInitializer.cs**

```csharp
// Data/DbInitializer.cs
using Microsoft.AspNetCore.Identity;

namespace WorldDominion.Models
{
    public class DbInitializer
    {
        public static async Task Initialize(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Admin", "Customer" };
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                    await roleManager.CreateAsync(new IdentityRole(roleName));
            }

            // Create an Admin user
            var user = new IdentityUser
            {
                UserName = "admin@admin.com",
                Email = "admin@admin.com",
            };

            string userPWD = "Password@123";
            var createUser = await userManager.CreateAsync(user, userPWD);

            if (createUser.Succeeded)
                await userManager.AddToRoleAsync(user, "Admin");
        }
    }
}
```

Add the DbInitializer as a service to our application
```csharp
// Program.cs
// Registering the DbInitializer seeder
builder.Services.AddTransient<DbInitializer>();
```

Seed the database
```csharp
// Program.cs
// Seed roles
var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using var scope = scopeFactory.CreateScope();
var initializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
await DbInitializer.Initialize(
    scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>(),
    scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>()
);
```