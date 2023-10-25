using Microsoft.AspNetCore.Mvc;
using MainProject.Models;

namespace MainProject.Components.ViewComponents
{
    public class NavigationMenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var menuItems = new List<MenuItem>
            {
                new MenuItem { Controller = "Home", Action = "Index", Label = "Home" },
                new MenuItem { Controller = "Categories", Action = "Index", Label = "Categories" },
                new MenuItem { Controller = "Products", Action = "Index", Label = "Products" },
                new MenuItem { Controller = "Carts", Action = "Index", Label = "View My Cart" },
                new MenuItem { Controller = "Home", Action = "Privacy", Label = "Privacy" },
                new MenuItem { Controller = "Home", Action = "Brief", Label = "Brief" },
            };
            return View(menuItems);
        }
    }
}