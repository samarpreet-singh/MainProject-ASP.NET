using Microsoft.AspNetCore.Identity;

namespace MainProject.Models
{
    public class DbInitializer
    {
        public static async Task Initialize(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            string[] roleNames = { "Admin", "Artisan", "Member" };
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);

                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                    }
            }

            var user = new IdentityUser
            {
                UserName = "admin@admin.com",
                Email = "admin@admin.com",
            };

            string userPWD = "Password@123";

            var createUser = await userManager.CreateAsync(user, userPWD);

            if (createUser.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }
}