namespace SportShop.Models;

public class SearchViewModel
{
    public string Categories { get; set; } = string.Empty;
    public string Producers { get; set; } = string.Empty;
    public string[] CategoriesArray { get; set; } = null!;
    public string[] ProducersArray { get; set; } = null!;
    public string? MinPrice { get; set; } 
    public string? MaxPrice { get; set; } 
}