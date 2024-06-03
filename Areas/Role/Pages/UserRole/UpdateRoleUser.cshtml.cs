namespace SportShop.Areas.Role.Pages.UserRole;

public class UpdateRoleUserModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    public UpdateRoleUserModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager)
    {
        _context = context;
        _roleManager = roleManager;
        _userManager = userManager;
        _signInManager = signInManager;
    }
    public class InputModel
    {
        public string UserId { get; set; } = String.Empty;
        public List<string> RoleNames { get; set; } = new List<string>();
        public List<string> RoleExistNames { get; set; } = new List<string>();
    }
    [BindProperty]
    public InputModel? Input { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public IActionResult OnGet() => NotFound("Không cho get  <3");
    //Goi page qua form
    public async Task<IActionResult> OnPostAsync()
    {
        //Lay toan bo ten roles
        var roleNamesAll = _roleManager.Roles.Select(c => c.Name).ToList();
        Console.WriteLine("inputIdUser: " + Input?.UserId);
        var idApplicationUser = Input?.UserId;
        var user = await _userManager.FindByIdAsync(idApplicationUser);

        var rolesExistName = new List<string>(await _userManager.GetRolesAsync(user));
        //So sanh 2 list roleNameAlls vs rolesExist
        //roleNames.Intersect(rolesExist);

        var rolesNameAbout = new List<string>(roleNamesAll.Except(rolesExistName));
        Input = new InputModel
        {
            UserId = idApplicationUser!,
            RoleNames = rolesNameAbout,
            RoleExistNames = rolesExistName
        };
        return Page();
    }
    public async Task<IActionResult> OnPostUpdateRoleOfUserAsync()
    {
        //them roleInput va xoa di nhung role con lai
        var rolesNameAll = _roleManager.Roles.Select(x => x.Name).ToList();
        var roleNameInputs = Input!.RoleNames;
        var rolesNameAbout = rolesNameAll.Except(roleNameInputs);
        var IdApplicationUser = Input.UserId;
        var roles = new List<IdentityRole>();
        if (IdApplicationUser != null)
        {
            var user = await _userManager.FindByIdAsync(IdApplicationUser);
            Console.WriteLine("idApplicationUser" + IdApplicationUser);
            if (user != null)
            {
                if (roleNameInputs.Count() > 0)
                {
                    foreach (var item in roleNameInputs)
                    {
                        var resultAddRole = await _userManager.AddToRoleAsync(user, item);
                        if (resultAddRole.Succeeded)
                        {
                            ErrorMessage = "Đã cập nhật role thành công ";
                        }
                        else
                        {

                            ErrorMessage = " Error add: " + resultAddRole.ToString();
                        }
                    }
                }

                if (rolesNameAbout.Count() > 0)
                {
                    foreach (var item in rolesNameAbout)
                    {
                        var resultRemoveRole = await _userManager.RemoveFromRoleAsync(user, item);
                        if (resultRemoveRole.Succeeded)
                        {
                            ErrorMessage = "Đã cập nhật role thành công ";
                        }
                        else
                        {

                            ErrorMessage = " Error remove: " + resultRemoveRole.ToString();
                        }
                    }
                }
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToPage("./Index");
            }
            else
            {
                ErrorMessage = "Lỗi không tìm thấy User";
                return RedirectToPage("./Index");
            }
        }
        else
        {
            ErrorMessage = "Lỗi UserId";
            return RedirectToPage("./Index");
        }

    }
}
