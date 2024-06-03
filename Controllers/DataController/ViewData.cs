namespace SportShop.Controllers.DataController;

public class ViewData
{
    public string ViewPath { get; private set; } = null!;
    public object? ModelData { get; private set; }
    public ErrorData? ErrorData { get; set; }

    //Khong nhat thiet phai setup trong luc khoi tao lop
    public ViewData (){}
    public ViewData(string viewPath, object modelData)
    {
        this.ViewPath = viewPath;
        this.ModelData = modelData;
    }
    public void SetViewPath (string viewPath)
    {
        this.ViewPath = viewPath;
    }
    public void SetModelData (object modelData)
    {
        this.ModelData = modelData;
    }
}