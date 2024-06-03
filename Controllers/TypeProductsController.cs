namespace SportShop.Controllers;

public class TypeProductsController : LookupController<Category>
{
    public TypeProductsController(ApplicationDbContext context) : base(context)
    {

    }
    protected override bool ModelExists(string name)
    {
        var model = _table.Where(x => x.Name == name).FirstOrDefault();
        return model != null;
    }
    protected override string GetControllerName()
    {
        return "TypeProducts";
    }
    protected override string GetNamePage()
    {
        return "Loại sản phẩm";
    }
    protected override Category ConvertToModelFromLookup(Lookup lookup)
    {
        return new Category()
        {
            Id = lookup.Id,
            Name = lookup.Name
        };
    }
    protected override Lookup ConvertToLookup(Category model)
    {
        return new Lookup()
        {
            Id = model.Id,
            Name = model.Name
        };
    }
}