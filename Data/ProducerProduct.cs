namespace SportShop.Data;

public class ProducerProduct : Lookup
{
    public string PhoneNumber { get; set; } = string.Empty;
    public int AddressId { get; set; }
    public List<Address> Address { get; set; } = null!;
}