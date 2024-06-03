using Microsoft.AspNetCore.Mvc.Rendering;

namespace SportShop.Models;

public class ProductViewModel
{
    public int Id { get; set; }

    [Display(Name = "Tên")]
    [Required(ErrorMessage = "Can them data")]
    public string NameProduct { get; set; } = null!;
    [Required(ErrorMessage = "Them thong tin vao cha noi")]
    [Display(Name = "Thông tin")]
    public string Information { get; set; } = null!;

    [Display(Name ="Giá")]
    [DisplayFormat(DataFormatString = "{0:N0}")]
    public double Price { get; set; }
    [Display(Name = "Số lượng")]
    public int Quantity { get; set; }

    public int CategoryId { get; set; }

    [Display(Name = "Loại")]
    public string? Category { get; set; }

    [Display(Name = "Loại")]
    public IEnumerable<SelectListItem>? Categories { get; set; }
    public int ProducerId { get; set; }

    [Display(Name = "Thương hiệu")]
    public string? Producer { get; set; }
    public IEnumerable<SelectListItem>? Producers { get; set; }
    public int StatusId { get; set; }

    [Display(Name = "Tình trạng")]
    public string? Status { get; set; }
    public IEnumerable<SelectListItem>? Statuses { get; set; } 

    public double AverageStar { get; set; }
    public int QuantityAllReview { get; set; }

}