using System.Collections;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SportShop.Controllers;

/*
    Sumary: 
       
*/
[Authorize(Roles = "Guest")]
public class ProductsController : CRUDGeneric<Product, ProductViewModel, int>
{
    
    private ViewData? _viewData;
    public ProductsController(ApplicationDbContext context) : base(context, "/Views/Products/")
    {
        
    }
    protected override string GetNamePage() => "Product";
    protected override string GetControllerName() => "Products";
    protected override int GetIdModel(Product model) => model.ProductId;

    protected override Product BuildModel(ProductViewModel viewModel)
    {
        if (viewModel != null)
        {
            Product product = new Product()
            {
                ProductId = viewModel.Id,
                Name = viewModel.NameProduct!,
                Information = viewModel.Information!,
                Price = viewModel.Price,
                Quantity = viewModel.Quantity,

                CategoryId = viewModel.CategoryId,
                ProducerId = viewModel.ProducerId,
                StatusId = viewModel.StatusId
            };
            return product;
        }
        else
        {
            return null!;
        }
    }
    protected override ProductViewModel BuildViewModel(Product? model)
    {

        ProductViewModel viewModel = new ProductViewModel();
        if (model != null)
        {
            viewModel.Id = model.ProductId;
            viewModel.NameProduct = model.Name;
            viewModel.Information = model.Information;
            viewModel.Price = model.Price;
            viewModel.Quantity = model.Quantity;
            viewModel.AverageStar = GetAverangeStarReview(model.ProductId);
            viewModel.QuantityAllReview = GetQuantityReview(model.ProductId);

            viewModel.CategoryId = model.CategoryId;
            viewModel.ProducerId = model.ProducerId;
            viewModel.StatusId = model.StatusId;

            viewModel.Category = GetNameTypeById(model.CategoryId);
            viewModel.Status = GetNameStatusById(model.StatusId);
            viewModel.Producer = GetNameProducerById(model.ProducerId);

            //Get all data Type
            viewModel.Categories = GetTypeProducts();
            //Get all data Status
            viewModel.Statuses = GetStatusProducts();
            //Get all data Producer
            viewModel.Producers = GetProducerProducts();
        }
        else
        {
            viewModel.Categories = GetTypeProducts();
            //Get all data Status
            viewModel.Statuses = GetStatusProducts();
            //Get all data Producer
            viewModel.Producers = GetProducerProducts();
        }
        return viewModel;
    }
    private string GetNameTypeById(int id)
    {
        return _context.Categories.Find(id)!.Name;
    }
    private string GetNameStatusById(int id)
    {
        return _context.Statuses.Find(id)!.Name;
    }
    private string GetNameProducerById(int id)
    {
        return _context.Producers.Find(id)!.Name;
    }
    private IEnumerable<SelectListItem> GetProducerProducts()
    {
        return _context.Producers.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = x.Name
        }).ToList();
    }

    private IEnumerable<SelectListItem> GetStatusProducts()
    {
        return _context.Statuses.Select(x => new SelectListItem()
        {
            Value = x.Id.ToString(),
            Text = x.Name
        }).ToList();

    }

    private IEnumerable<SelectListItem> GetTypeProducts()
    {
        return _context.Categories.Select(x => new SelectListItem()
        {
            Value = x.Id.ToString(),
            Text = x.Name
        }).ToList();
    }

    protected override IEnumerable<ProductViewModel> BuildListViewModel(IEnumerable<Product> models)
    {
        IList<ProductViewModel> viewData = new List<ProductViewModel>();
        foreach (var product in models)
        {
            viewData.Add(new ProductViewModel()
            {
                Id = product.ProductId,
                NameProduct = product.Name,
                Information = product.Information,
                Price = product.Price,
                Quantity = product.Quantity,
                QuantityAllReview = GetQuantityReview(product.ProductId),
                AverageStar = GetAverangeStarReview(product.ProductId),

                CategoryId = product.CategoryId,
                ProducerId = product.ProducerId,
                StatusId = product.StatusId,

                Category = GetNameTypeById(product.CategoryId),
                Status = GetNameStatusById(product.StatusId),
                Producer = GetNameProducerById(product.ProducerId),

                Categories = GetTypeProducts(),
                Statuses = GetStatusProducts(),
                Producers = GetProducerProducts()
            });
        }
        return viewData;
    }

    private double GetAverangeStarReview(int productId)
    {
        var allReview = _context.Reviews.Where(r => r.ProductId == productId);
        if(allReview.Count() > 0)
        {
            return allReview.Average(x=> x.NumberOfStar);
        }
        else 
        {
            return 0;
        }
    }

    private int GetQuantityReview(int productId)
    {
        var allReviews = _context.Reviews.Where(r => r.ProductId == productId).ToArray();
        return allReviews.Length;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult ShowProductsByCategory(int quantity)
    {
        string? viewPathShowProduct = _viewPath + "ShowProducts.cshtml";

        var Categories = _context.Categories.ToList();
        List<string> CategoriesNames = new List<string>();
        List<Product> products  = new List<Product>();
        foreach (var Category in Categories)
        {
            CategoriesNames.Add(Category.Name);
            products.AddRange (_context.Products.Where(p => p.CategoryId == Category.Id).ToList().Take(quantity));
        }
        ViewData["CategoriesNames"] = CategoriesNames;
     
        IEnumerable<ProductViewModel> viewData = BuildListViewModel(products);

        return PartialView(viewPathShowProduct, viewData);
    }
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Category(string categoryName)
    {
        ViewData["Title"] = categoryName;
        ViewData["PathImage"] = "/images/" + categoryName + ".png";
        var categoryFound = _context.Categories.Where(c => c.Name == categoryName).FirstOrDefault();
        if(categoryFound == null)
            return NotFound();
        var products = _context.Products.Where(p => p.CategoryId == categoryFound.Id).ToList();
        var productsViewModel = BuildListViewModel(products);
        return View(productsViewModel);

    }
}