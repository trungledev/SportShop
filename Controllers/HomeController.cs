namespace SportShop.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    public IActionResult CategoriesLayout()
    {
        var Categories = _context.Categories.ToList();
        IList<string> CategoriesNames = new List<string>();
        foreach (var Category in Categories)
        {
            CategoriesNames.Add(Category.Name);
        }
        return PartialView("~/Views/Shared/_CategoriesProduct.cshtml", CategoriesNames);
    }
    public IActionResult Index()
    {
        var Categories = _context.Categories.ToList();
        IList<string> CategoriesNames = new List<string>();
        foreach (var Category in Categories)
        {
            CategoriesNames.Add(Category.Name);
        }
        ViewData["CategoriesNames"] = CategoriesNames;
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new HomeMessageViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
