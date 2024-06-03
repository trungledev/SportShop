using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SportShop.Areas.Admin.Pages;


[Authorize(Roles ="Admin")]
public class AdminIndexModel : PageModel
{
    [BindProperty]
    public IList<DataAdmin>? ListData { get; set; }

    public class DataAdmin
    {
        public string NameData { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
    }

    [HttpGet]
    public IActionResult OnGet()
    {
        ListData = GetDataAdmins();
        return Page();
    }
    [HttpPost]
    public void OnPost() { }
    private IList<DataAdmin> GetDataAdmins()
    {
        IList<DataAdmin> listData = new List<DataAdmin>();
        listData.Add(new DataAdmin()
        {
            NameData = "Loại",
            Path = "/TypeProducts/Index"
        });
        listData.Add(new DataAdmin()
        {
            NameData = "Trạng thái",
            Path = "/StatusProducts/Index"
        });
        listData.Add(new DataAdmin()
        {
            NameData = "Nhà sản xuất",
            Path = "/ProducerProducts/Index"
        });
        return listData;
    }
}
