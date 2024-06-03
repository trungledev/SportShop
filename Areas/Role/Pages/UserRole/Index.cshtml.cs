namespace SportShop.Areas.Role.Pages.UserRole;

 [Authorize(Roles ="Guest")]
public class IndexUserRoleModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public class InputModel
    {
        [EmailAddress(ErrorMessage = "Lỗi định dạng")]
        public string? Email { get; set; }
    }
    public class UserRoleModel
    {
        public string? UserId { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        [Display(Name = "Họ và tên")]
        public string? FullName { get; set; }
        [Display(Name = "Vai trò")]
        public List<string>? Roles { get; set; }
    }

    public List<UserRoleModel>? UserRoleModels { get; set; }

    [BindProperty]
    public InputModel? Input { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public IndexUserRoleModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }
    public async Task<IActionResult> OnGet()
    {
        var models = new List<UserRoleModel>();
        // Lay HoVaTen nguoi dung
        var allUser = _context.Users.ToList();
        //Lay role theo user
        foreach (var user in allUser)
        {
            var listModels = await AddUserRoleModelAsync((ApplicationUser)user);
            models.AddRange(listModels);
        }
        UserRoleModels = models;
        return Page();
    }
    public async Task<IActionResult> OnPostSearchUserAsync()
    {
        var models = new List<UserRoleModel>();
        if (ModelState.IsValid)
        {
            var emailInput = Input?.Email;
            if (emailInput != null)
            {
                //Tim nguoi dung co email trong tim kiem
                var userSearch = await _userManager.FindByEmailAsync(emailInput);
                if (userSearch != null)
                {
                    var listModels = await AddUserRoleModelAsync(userSearch);
                    models.AddRange(listModels);
                    UserRoleModels = models;
                    return Page();
                }
            }
            else
            {
                await this.OnGet();
            }
        }
        // else
        // {
        //     ModelState.AddModelError("", "loi me may");
        //     return Page();
        // }
        return Page();
    }
    private async Task<List<UserRoleModel>> AddUserRoleModelAsync(ApplicationUser user)
    {
        var _userRoleModels = new List<UserRoleModel>();
        List<string> roles = new List<string>(await _userManager.GetRolesAsync(user));
        _userRoleModels.Add(new UserRoleModel
        {
            UserId = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Roles = roles
        });
        return _userRoleModels;
    }
}