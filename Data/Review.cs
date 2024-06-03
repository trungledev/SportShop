namespace SportShop.Data;

public class Review
{
    public string UserId { get; set; } = null!;
    public int ProductId { get; set; }
    public double NumberOfStar { get; set; }
    public string Title { get; set; } = null!;
    public string? Content { get; set; }

    public ApplicationUser User { get; set; } = null!;
    public Product Product { get; set; } = null!;    
}
