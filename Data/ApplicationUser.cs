namespace SportShop.Data;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = null!;
    //Connect to AddressData
    public int AddressId { get; set; }
    public List<Address> Address { get; set; } = null!;
}