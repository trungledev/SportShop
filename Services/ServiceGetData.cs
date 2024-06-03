namespace SportShop.Services;

public class ServiceGetData<TModel> : IServiceGetData<TModel> where TModel : class
{
    private readonly ApplicationDbContext _context;
    public ServiceGetData(ApplicationDbContext context)
    {
        _context = context;
    }
    public List<TModel> GetAllModels()
    {
        var list = _context.Set<TModel>().ToList();
        return list;
    }
    public List<TModel> GetAnyModels(int quantity)
    {
        var allData = GetAllModels();
        var anyModel = allData.GetRange(0, quantity) ;
        return anyModel;
    }
   
}