using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SportShop.Areas.Role.Pages.ManageRole;
public class EditRoleModel : PageModel
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public EditRoleModel(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }
    [TempData]
    public string? ErrorMessage { get; set; }
    public class InputModel
    {
        public string? Id { get; set; }
        [Required(ErrorMessage = "Phải điền tên role")]
        [Display(Name = "Tên role")]
        public string? Name { get; set; }
    }
    [BindProperty]
    public InputModel? Input { get; set; }

    public async Task<IActionResult> OnGetAsync(string idRole)
    {
        //Lay role theo id
        if (idRole != null)
        {
            var role = await _roleManager.FindByIdAsync(idRole);
            Input = new InputModel
            {
                Id = role.Id,
                Name = role.Name
            };
        }
        return Page();
    }
    public async Task<IActionResult> OnPostAsync()
    {
        //Xác thực Input
        if (ModelState.IsValid)
        {
            //Lấy role trong data
            var nameInput = Input?.Name;
            var idRole = Input?.Id;
            if (nameInput != String.Empty || idRole != String.Empty)
            {
                var role = await _roleManager.FindByIdAsync(idRole);
                if (role != null)
                {
                    //Tim thay role thoa man
                    if (role.Name != Input?.Name && Input?.Id != null)
                        role.Name = Input?.Name;
                    var resultUpdate = await _roleManager.UpdateAsync(role);
                    ModelState.Clear();
                    if (resultUpdate.Succeeded)
                    {
                        ErrorMessage = "Role đã được sửa";
                    }
                    else
                    {
                        foreach (var error in resultUpdate.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                else
                {
                    ModelState.Clear();
                    ErrorMessage = "Không tìm thấy role ";
                }
            }
            else
            {
                ModelState.AddModelError(String.Empty, "name : null");
                return Page();
            }
        }
        return RedirectToPage("./Index");
    }
}