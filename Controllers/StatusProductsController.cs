namespace SportShop.Controllers;

public class StatusProductsController : LookupController<StatusProduct>
{
    public StatusProductsController(ApplicationDbContext context) : base(context)
    {

    }
    protected override bool ModelExists(string name)
    {
        var model = _table.Where(x => x.Name == name).FirstOrDefault();
        return model!= null;
    }
    protected override string GetControllerName()
    {
        return "StatusProducts";
    }
    protected override string GetNamePage()
    {
        return "Trạng thái sản phẩm";
    }
    protected override StatusProduct ConvertToModelFromLookup(Lookup lookup)
    {
        return new StatusProduct()
        {
            Id = lookup.Id,
            Name = lookup.Name
        };
    }
    protected override Lookup ConvertToLookup(StatusProduct model)
    {
        return new Lookup()
        {
            Id = model.Id,
            Name = model.Name
        };
    }
}