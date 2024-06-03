namespace SportShop.Models.MessageViewModels;

public class CRUDMessageViewModel
{
    public int Success { get; set; } = 0; //1 true,-1 false, 0 not show
    public string Message { get; set; } = string.Empty;
    public string? ReturnUrl { get; set; }
}