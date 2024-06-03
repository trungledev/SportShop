namespace SportShop.Models.MessageViewModels;

public class ReviewMessageViewModel
{
    public int Success { get; set; } = 0; //1 true,-1 false, 0 not show
    public string Message { get; set; } = string.Empty;
    public bool IsSigned { get; set; } = true;
    public int ProductId { get; set; }
    public bool IsReviewed { get; set; } = false;
    public string ReturnUrl { get; set; } = string.Empty;
}