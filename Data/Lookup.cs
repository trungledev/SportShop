namespace SportShop.Data;


public class Lookup
{
    [Key]
    public int Id { get; set; }
    [Display(Name = "Tên")]
    public string Name { get; set; } = null!;

    /*
        Reference navigation property
        Relational 1-Many Product
    */
    public int ProductId { get; set; }
    public List<Product> Products { get; set; } = null!;
}
