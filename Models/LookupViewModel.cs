namespace SportShop.Models;

public class LookupViewModel
{
    public int Id { get; set; }
    [Required]
    [Display(Name = "Tên")]
    public string Name { get; set; } = string.Empty;
}