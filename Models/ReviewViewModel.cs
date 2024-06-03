namespace SportShop.Models;

public class ReviewViewModel
{
    public int ProductId { get; set; }
    [Display(Name ="Tiêu đề")]
    public string Title { get; set; } = string.Empty;
    [Display(Name ="Nội dung")]
    public string Content { get; set; } = string.Empty;
    public double AverageStar { get; set; }
    [Display(Name ="Số sao")]
    public double NumberOfStar { get; set; }
    public IDictionary<string,int>? DetailStars {get; set; }
    public int QuantityAllReview { get; set; }
    public DateTime Time { get; set; }
    public string Author { get; set; } = string.Empty;
}