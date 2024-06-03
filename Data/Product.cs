namespace SportShop.Data;

public class Product 
{
    [Key]
    public int ProductId { get; set; }
    public string Name { get; set; } = null!;
    public string Information { get; set; } = null!;
    public double Price { get; set; }
    public int Quantity { get; set; }
    public string? ImagePath { get; set; } = null!;
    
    
    //
    //  Relational of Product
    //
    public int ProducerId { get; set; }
    public ProducerProduct Producer { get; set; } = null!;
    //Connect to Category
    public int CategoryId {get; set;}
    public Category Category { get; set; } = null!;
    //Connect to StatusData
    public int StatusId { get; set; }
    public StatusProduct Status { get; set; } = null!;
}