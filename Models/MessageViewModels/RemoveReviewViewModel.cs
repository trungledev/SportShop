namespace SportShop.Models.MessageViewModels;

public class RemoveReviewViewModel 
{
    public int ProductId { get; set; }
    public string Message { get; set; } = string.Empty;
    public string ReturnUrl { get; set; } = string.Empty;
}