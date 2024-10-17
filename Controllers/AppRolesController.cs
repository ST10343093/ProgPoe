using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ProgPoe.Controllers
{
    // Controller for managing application roles
    public class AppRolesController : Controller
    {
        // Declare variable to manage roles
        private readonly RoleManager<IdentityRole> _roleManager;

        // Constructor to initialize RoleManager with dependency injection
        public AppRolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        // Action method to list all roles created by users
        public IActionResult Index()
        {
            // Fetch all roles from RoleManager
            var roles = _roleManager.Roles;
            // Return the roles to the view
            return View(roles);
        }

        // GET: Display the Create role view (for adding a new role)
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Create a new role
        [HttpPost]
        public async Task<IActionResult> Create(IdentityRole model)
        {
            // Check if the role already exists to avoid duplication
            if (!_roleManager.RoleExistsAsync(model.Name).GetAwaiter().GetResult())
            {
                // If the role does not exist, create a new one
                _roleManager.CreateAsync(new IdentityRole(model.Name)).GetAwaiter().GetResult();
            }
            // Redirect back to the list of roles after creating a new role
            return RedirectToAction("Index");
        }
    }
}
