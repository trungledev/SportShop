
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SportShop.Areas.Role.Pages.ManageRole;
public class AddRoleModel : PageModel
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public AddRoleModel(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }
    [TempData]
    public string? ErrorMessage { get; set; }
    public class InputModel
    {
        [Required(ErrorMessage = "Phải điền tên role")]
        [Display(Name = "Tên role")]
        public string? Name { get; set; }
    }
    [BindProperty]
    public InputModel? Input { get; set; }
    public IActionResult OnGet() => Page();
   
    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            //Kiểm tra tên Role đã tồn tại 
            var nameRoleInput = Input?.Name;
            if (nameRoleInput != null)
            {
                bool roleExist = await _roleManager.RoleExistsAsync(nameRoleInput);
                
                if (roleExist)
                {
                    ModelState.AddModelError(String.Empty, "Role đã tồn tại");
                    return Page();
                }
                else
                {
                    //Tạo role mới vào IdentityRole
                    var roleAdd = new IdentityRole(nameRoleInput);
                    var resultAddRole = await _roleManager.CreateAsync(roleAdd);
                    if (resultAddRole.Succeeded)
                    {
                        //Thêm role thành công
                        return RedirectToPage("./Index");
                    }
                    else
                    {
                        //Lỗi khi thêm role vào IdentityUser
                        ErrorMessage = " Lỗi khi thêm role vào IdentityUser";
                        return RedirectToPage("./Index");
                    }
                }
            }
            else
            {
                ModelState.AddModelError(String.Empty, "Tên rỗng");
                return Page();
            }
        }
        else
        {
            return Page();
        }
    }
    
}