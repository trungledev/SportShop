namespace SportShop.Models;

public class CartViewModel
{
    public int Id { get; set; }
    [Display(Name = "Tên sản phẩm")]
    public string? Name { get; set; }

    public string? Infomation { get; set; }
    [Display(Name = "Số lượng")]
    public int Quantity { get; set; }
    [DisplayFormat(DataFormatString = "{0:N0}")]
    public double Price { get; set; }
    [DisplayFormat(DataFormatString = "{0:N0}")]
    [Display(Name = "Thành tiền")]
    public double Total { get; set; }
}