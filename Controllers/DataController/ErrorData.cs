namespace SportShop.Controllers.DataController;

public class ErrorData
{
    public string? Message { get; set; } = null!;
    public bool Success { get; set; } = true;
    public ErrorData(string? message, bool success)
    {
        this.Message = message;
        this.Success = success;
    }
    public ErrorData()
    {

    } 
}