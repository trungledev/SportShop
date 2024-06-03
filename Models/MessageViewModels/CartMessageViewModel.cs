namespace SportShop.Models.MessageViewModels;

public class CartMessageViewModel
{
    public int Success { get; set; } = 0; //1 true,-1 false, 0 not show
    public string Message { get; set; } = string.Empty;
    public string? ReturnUrl { get; set; }
    public bool IsSigned { get; set; } = true; 
}