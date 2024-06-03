using System.Text.RegularExpressions;

using System.Text.Encodings.Web;

namespace SportShop.Controllers;

public class SearchController : Controller
{
    private readonly ApplicationDbContext _context;
    private const string PATH_VIEW = "~/Views/Search/_ProductsFound.cshtml";

    public SearchController(ApplicationDbContext context)
    {
        _context = context;
    }
    [HttpGet]
    public IActionResult Index()
    {
        return Search(String.Empty);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index(string wordSearch)
    {
        return Search(wordSearch);
    }

    private IActionResult Search(string wordSearch)
    {
        IList<string> categoryNames = GetCategoriesName();
        ViewData["CategoryNames"] = categoryNames;

        IList<string> producerNames = GetProducersName();
        ViewData["ProducerNames"] = producerNames;

        var data = GetData();
        ViewData["WordSearch"] = wordSearch;

        //Show all san pham
        if (String.IsNullOrEmpty(wordSearch))
        {
            var allProducts = _context.Products.ToList();
            var allProductsViewModel = DataToViewModel(allProducts);
            return View(allProductsViewModel);
        }
        else
        {
            Regex pattern = new Regex("[^a-zA-Z0-9]");
            wordSearch = pattern.Replace(wordSearch, "\n");

        }
        //So sanh Cum
        string[] keywords = SplitInput(wordSearch);
        string[] characters = DecayInput(wordSearch);
        List<int> listProductId = ProductFound(keywords, data);

        //Test
        if (keywords == null || keywords.Length == 0)
            ViewData["Message"] = "Loi Tach Chuoi";
        else if (data == null)
            ViewData["Message"] = "Loi Data";
        else if (listProductId == null || listProductId.Count == 0)
        {
            listProductId = ProductFound(characters, data);
            ViewData["Message"] = listProductId;
        }
        else
            ViewData["Message"] = listProductId;

        //Theo Tu khoa tim kiem
        IEnumerable<Product> productFound = GetProductById(listProductId);
        IEnumerable<ProductViewModel> productViewModel = new List<ProductViewModel>();
        if (productFound != null)
        {
            if (productFound.Count() == 0)
                ViewData["messageEmpty"] = "Không tìm thấy sản phẩm";
            else
                productViewModel = DataToViewModel(productFound);
        }
        return View(productViewModel);
    }

    private IList<string> GetProducersName()
    {
        IList<string> result = new List<string>();
        foreach (var producer in _context.Producers.ToArray())
        {
            result.Add(producer.Name);
        }
        return result;
    }

    private IList<string> GetCategoriesName()
    {
        IList<string> result = new List<string>();
        foreach (var category in _context.Categories.ToArray())
        {
            result.Add(category.Name);
        }
        return result;
    }

    [HttpGet]
    public IActionResult AscendingPrice()
    {
        var products = GetProductsAscendingPrice(true);
        if (products != null)
        {
            var ProductViewModel = DataToViewModel(products);
            return PartialView(PATH_VIEW, ProductViewModel);
        }
        else
            return PartialView(PATH_VIEW, null);
    }
    [HttpGet]
    public IActionResult DescendingPrice()
    {
        var products = GetProductsAscendingPrice(false);
        if (products != null)
        {
            var ProductViewModel = DataToViewModel(products);
            return PartialView(PATH_VIEW, ProductViewModel);
        }
        else
            return PartialView(PATH_VIEW, null);

    }

    [HttpPost]
    public IActionResult SearchFilters([Bind(include: "Categories,Producers,MinPrice,MaxPrice")] SearchViewModel filters)
    {
        var allProducts = _context.Products.ToList();
        var allProductsViewModel = DataToViewModel(allProducts);
        if (string.IsNullOrEmpty(filters.MinPrice))
        {
            filters.MinPrice = "0";
        }
        if (string.IsNullOrEmpty(filters.MaxPrice))
        {
            filters.MaxPrice = "0";
        }

        if (filters.Categories == null && filters.Producers == null && filters.MinPrice == "0" && filters.MaxPrice == "0")
            return PartialView(PATH_VIEW, allProductsViewModel);
        filters.Categories ??= string.Empty;
        filters.Producers ??= string.Empty;

        filters.CategoriesArray = filters.Categories.Split(",");
        filters.ProducersArray = filters.Producers.Split(",");

        var products = SearchByProperty(filters);
        if (products.Count() == 0)
        {
            return PartialView(PATH_VIEW, new List<ProductViewModel>());
        }
        else
        {
            var ProductViewModel = DataToViewModel(products);
            return PartialView(PATH_VIEW, ProductViewModel);
        }

    }
    //Loc san pham theo thuoc tinh
    private IEnumerable<Product> SearchByProperty(SearchViewModel searchFilters)
    {
        var minStr = searchFilters.MinPrice;
        var maxStr = searchFilters.MaxPrice;
        if (minStr != null && maxStr != null)
        {
            minStr = minStr.Replace(",", "");
            maxStr = maxStr.Replace(",", "");
        }
        //Convert tp double
        double min = 0;
        double max = 0;
        var isSuccessParseMin = double.TryParse(minStr, out min);
        var isSuccessParseMax = double.TryParse(maxStr, out max);

        Console.WriteLine("Min: " + min + ", Max:" + max);

        if (!isSuccessParseMin || !isSuccessParseMax)
        {
            //Thông báo lỗi không thể chuyển sang dạng double
            return new List<Product>();
        }
        //List
        var products = new List<Product>();
        var productsByCategory = new List<Product>() { };
        var productsByProducer = new List<Product>() { };
        var productsByPrice = new List<Product>();

        //Loc Theo Loai
        if (searchFilters.CategoriesArray.Length > 0)
        {
            var categories = searchFilters.CategoriesArray;
            IList<Product> productsFoundByCategory = GetProductByCategoryNames(categories);
            productsByCategory.AddRange(productsFoundByCategory);

        }
        //Loc theo thuong hieu
        if (searchFilters.ProducersArray.Length > 0)
        {
            var producers = searchFilters.ProducersArray;
            IList<Product> productsFoundByProducer = GetProductByProducerNames(producers);
            Console.WriteLine("Length array: " + searchFilters.ProducersArray.Length + " Count : " + productsFoundByProducer.Count);
            productsByProducer.AddRange(productsFoundByProducer);
        }
        //Loc theo gia ca 
        if (min >= 0 && max >= 0 && max > min)
        {

            var productByMinMax = from product in _context.Products
                                  where product.Price > min && product.Price <= max
                                  select product;

            if (productByMinMax != null)
                productsByPrice.AddRange(productByMinMax);

        }

        //Giao 2 list 
        if (productsByCategory.Count > 0 && productsByProducer.Count > 0 && productsByPrice.Count > 0)
        {
            var m = productsByCategory.Intersect(productsByProducer);
            m = m.Intersect(productsByPrice);
            products.AddRange(m);
        }
        else if (productsByCategory.Count == 0 && productsByProducer.Count > 0 && productsByPrice.Count > 0)
        {
            var m = productsByPrice.Intersect(productsByProducer);
            products.AddRange(m);
        }
        else if (productsByProducer.Count == 0 && productsByCategory.Count > 0 && productsByPrice.Count > 0)
        {

            var m = productsByCategory.Intersect(productsByPrice);
            products.AddRange(m);
        }
        else if (productsByPrice.Count == 0 && productsByCategory.Count > 0 && productsByProducer.Count > 0)
        {
            var m = productsByCategory.Intersect(productsByProducer);
            products.AddRange(m);
        }
        else
        {
            products.AddRange(productsByPrice);
            products.AddRange(productsByCategory);
            products.AddRange(productsByProducer);
        }
        return products;
    }

    private IList<Product> GetProductByCategoryNames(string[] categories)
    {
        var products = new List<Product>();
        foreach (string category in categories)
        {
            int categoryId = GetCategoryIdByName(category);
            products.AddRange(from product in _context.Products
                              where product.CategoryId == categoryId
                              select product);
        }
        return products;
    }

    private int GetCategoryIdByName(string categoryName)
    {
        var categoryFound = _context.Categories.FirstOrDefault(c => c.Name == categoryName);
        if (categoryFound != null)
            return categoryFound.Id;
        else
            return 0;
    }

    private IList<Product> GetProductByProducerNames(string[] producers)
    {
        var products = new List<Product>();
        foreach (string producer in producers)
        {
            int producerId = GetProducerIdByName(producer);
            Console.WriteLine("Name: " + producer + " Id: " + producerId);
            products.AddRange(from product in _context.Products
                              where product.ProducerId == producerId
                              select product);
        }
        return products;
    }

    private int GetProducerIdByName(string producerName)
    {
        var producerFound = _context.Producers.FirstOrDefault(c => c.Name == producerName);
        if (producerFound != null)
            return producerFound.Id;
        else
            return 0;
    }

    // private IList<Product> GetProductByProperty(string[] propertyNameArr)
    // {
    //     var productsFound = new List<Product>();
    //     foreach (var item in propertyNameArr)
    //     {
    //         string nameOfCategory = nameof(propertyNameArr);
    //         System.Reflection.PropertyInfo? prop = typeof(Product).GetProperty(nameOfCategory);
    //         if (prop != null)
    //         {
    //             foreach (var product in _context.Products)
    //             {
    //                 if (prop.GetValue(product)!.ToString() == item)
    //                     productsFound.Add(product);
    //             }
    //         }
    //     }
    //     return productsFound;
    // }

    private IEnumerable<Product>? GetProductsAscendingPrice(bool isAcsending)
    {
        IEnumerable<Product> productsFound;
        if (isAcsending)
        {
            productsFound = from product in _context.Products
                            orderby product.Price ascending
                            select product;
            if (productsFound != null)
                return productsFound;
            else
                return null;
        }
        else
        {
            productsFound = from product in _context.Products
                            orderby product.Price descending
                            select product;
            if (productsFound != null)
                return productsFound;
            else
                return null;
        }

    }
    private IEnumerable<Product> GetProductById(List<int> productId)
    {
        var productsFound = new List<Product>();
        if (productId != null)
        {
            foreach (var id in productId)
            {
                var product = _context.Products.Where(x => x.ProductId == id).FirstOrDefault();
                if (product != null)
                {
                    productsFound.Add(product);
                }
            }
        }
        return productsFound;
    }

    private IEnumerable<ProductViewModel> DataToViewModel(IEnumerable<Product> products)
    {

        var productsFound = new List<ProductViewModel>();
        foreach (var product in products)
        {
            if (product != null)
            {
                productsFound.Add(new ProductViewModel
                {
                    Id = product.ProductId,
                    NameProduct = product.Name,
                    Category = GetNameCategoryOfProduct(product.CategoryId),
                    Producer = GetNameProducerOfProduct(product.ProducerId),
                    Information = product.Information,
                    Status = GetNameStatusOfProduct(product.StatusId),
                    Price = product.Price,
                    AverageStar = GetAverangeStarReview(product.ProductId),
                    QuantityAllReview = GetQuantityReview(product.ProductId)
                });
            }
        }
        return productsFound;
    }

    private int GetQuantityReview(int productId)
    {
        var allReviews = _context.Reviews.Where(r => r.ProductId == productId).ToArray();
        return allReviews.Length;
    }

    private double GetAverangeStarReview(int productId)
    {
        var allReview = _context.Reviews.Where(r => r.ProductId == productId);
        if (allReview.Count() > 0)
        {
            return allReview.Average(x => x.NumberOfStar);
        }
        else
        {
            return 0;
        }
    }

    private string GetNameStatusOfProduct(int statusId)
    {
        var statusFound = _context.Statuses.Find(statusId);
        return statusFound != null ? statusFound.Name : string.Empty;
    }

    private string GetNameCategoryOfProduct(int categoryId)
    {
        var categoryFound = _context.Categories.Find(categoryId);
        return categoryFound != null ? categoryFound.Name : string.Empty;
    }

    private string GetNameProducerOfProduct(int producerId)
    {
        var producerFound = _context.Producers.Find(producerId);
        return producerFound != null ? producerFound.Name : string.Empty;
    }

    private List<int> ProductFound(string[] inputString, IDictionary<string, IDictionary<string, int>> dataProducts)
    {
        List<int> result = new List<int> { };
        var nameProducts = dataProducts["NameProduct"];
        //Tim kiem trong Ten
        if (inputString != null && nameProducts != null)
        {
            foreach (KeyValuePair<string, int> nameProduct in nameProducts)
            {
                int quantityCharInNameProduct = 0;
                //Tra cuu theo cum 
                foreach (var str in inputString)
                {
                    if (str.Length == 1)
                    {

                        if ((nameProduct.Key).Contains(str))
                        {
                            quantityCharInNameProduct++;
                        }
                    }
                    else if ((nameProduct.Key).Contains(str))
                    {
                        result.Add(nameProduct.Value);
                        break;
                    }
                }
                if (quantityCharInNameProduct > 2)
                {
                    result.Add(nameProduct.Value);
                }
            }
        }
        return result;
    }
    //Quy het dinh dang le lower de so sanh
    private string[] SplitInput(string input)
    {
        //Chia Nho chuoi duoc phan cach bang space va chuyen het ve lower
        input = input.ToLower();
        input = input.Trim();
        var result = new string[] { };
        if (input != null)
        {
            result = input.Split(" ");
        }
        return result;
    }
    private string[] DecayInput(string input)
    {
        var characters = new char[] { };
        input = input.ToLower().Trim();
        IEnumerable<char> x = input.TakeWhile(x => x != ' ');
        characters = x.ToArray();
        var characterArray = characters.Select(c => c.ToString()).ToArray();
        return characterArray;
    }

    private IDictionary<string, IDictionary<string, int>> GetData()
    {
        var products = _context.Products.ToArray();
        IDictionary<string, int> nameOfProducts = new Dictionary<string, int>();
        IDictionary<string, int> inforOfProducts = new Dictionary<string, int>();
        IDictionary<string, IDictionary<string, int>>? informationOfProducts = new Dictionary<string, IDictionary<string, int>>();
        if (products == null)
            informationOfProducts = null;
        else
        {
            foreach (var product in products)
            {
                var nameOfProduct = product.Name;
                if (nameOfProduct != null)
                    nameOfProducts.Add(nameOfProduct.ToLower(), product.ProductId);
                var inforProduct = product.Information;
                if (inforProduct != null)
                    inforOfProducts.Add(inforProduct.ToLower(), product.ProductId);

            }
            if (nameOfProducts.Count > 0 && inforOfProducts.Count > 0)
            {
                informationOfProducts.Add("NameProduct", nameOfProducts);
                informationOfProducts.Add("Information", inforOfProducts);
            }
        }
        return informationOfProducts!;
    }
}