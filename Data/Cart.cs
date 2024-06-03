namespace SportShop.Data;

public class Cart
{
    public string UserId { get; set; } = null!;
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public double Total { get; set; }

    public ApplicationUser User { get; set; } = null!;
    public Product Product { get; set; } = null!;    
}