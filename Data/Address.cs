namespace SportShop.Data;

public class Address
{
    [Key]
    public int AddressId { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public string ApplicationUserId { get; set; } = null!;
    public ApplicationUser ApplicationUser { get; set; } = null!;
    public string ProducerId { get; set; } = null!;
    public ProducerProduct ProducerProduct { get; set; } = null!;
}