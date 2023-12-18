using Microsoft.EntityFrameworkCore;
using MainProject.Models;
using Microsoft.AspNetCore.Identity;
using MainProject.Services;

var builder = WebApplication.CreateBuilder(args);

// Add sessions
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add MySQL
var connectionString = builder.Configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("Connection string not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {

        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        string connStr;

        if (env == "Development")
        {
            connStr = builder.Configuration.GetConnectionString("DemoConnection");


        }
        else
        {
            // Use connection string provided at runtime by Heroku.
            var connUrl = Environment.GetEnvironmentVariable("JAWSDB_URL");

            connUrl = connUrl.Replace("mysql://", string.Empty);
            var userPassSide = connUrl.Split("@")[0];
            var hostSide = connUrl.Split("@")[1];

            var connUser = userPassSide.Split(":")[0];
            var connPass = userPassSide.Split(":")[1];
            var connHost = hostSide.Split("/")[0];
            var connDb = hostSide.Split("/")[1].Split("?")[0];


            connStr = $"server={connHost};Uid={connUser};Pwd={connPass};Database={connDb}";



        }

        options.UseMySQL(connStr);

    });

builder.Services.AddDefaultIdentity<IdentityUser>() // IdentityUser is a template for a user
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>(); // This asks the application to use the db we set up and will use correct db to build migrations

builder.Services.AddTransient<DbInitializer>();

builder.Services.AddAuthentication().AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
});

builder.Services.AddScoped<CartService>(); // this is an important step that turns CartService into an injectable dependency that makes it available in the cartsController and wherever else it is used.

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);// takes the appsettings development file and
// inject it into every controller as long as we have a listener in controller (param in constructor to look for this)

var app = builder.Build();

// Enable Sessions on requests
app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using var scope = scopeFactory.CreateScope();
var initializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
await DbInitializer.Initialize(
    scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>(),
    scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>()
);

app.Run();
