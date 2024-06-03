using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SportShop.Areas.Role.Pages.ManageRole;

//Hiển thị các role
/*
    Add 
    Edit
    Delete
*/
 [Authorize(Roles ="Guest")]
public class IndexRoleModel : PageModel
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public List<IdentityRole> roles {set; get;} = null!;

    public IndexRoleModel(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }
    //Hàm được gọi khi gọi qua link trong @page
    
    public IActionResult OnGet()
    {
        roles = _roleManager.Roles.ToList();
        return Page();
    }
    //Hàm được thực thi khi có post form 
    public IActionResult OnPost()
    {
        return NotFound("Khong cho post");
    }
}