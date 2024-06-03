namespace SportShop.Controllers;
// [Authorize(Roles ="Admin")]
public abstract class LookupController<TModel> : CRUDGeneric<TModel, LookupViewModel, int>
    where TModel : class
{
    protected LookupController(ApplicationDbContext context) : base(context, "/Views/Lookup/")
    {
        
    }
    protected abstract Lookup ConvertToLookup(TModel model);
    protected abstract TModel ConvertToModelFromLookup(Lookup model);

    protected override int GetIdModel(TModel model) => ConvertToLookup(model).Id;
    protected override TModel BuildModel(LookupViewModel lookupViewModel) 
    {
        var generic = new Lookup()
        {
            Id = lookupViewModel.Id,
            Name = lookupViewModel.Name
        };
        TModel model = ConvertToModelFromLookup(generic);
        return model;
    }
    protected override LookupViewModel BuildViewModel(TModel? model)
    {
        if (model == null)
        {
            return new LookupViewModel();
        }
        else
        {

            var lookup = ConvertToLookup(model);
            return new LookupViewModel()
            {
                Id = lookup.Id,
                Name = lookup.Name
            };
        }
    }
    protected override IEnumerable<LookupViewModel> BuildListViewModel(IEnumerable<TModel> models)
    {
        IList<LookupViewModel> lookupViewModels = new List<LookupViewModel>();
        foreach (var model in models)
        {
            var lookup = ConvertToLookup(model);
            lookupViewModels.Add(BuildViewModel(model));
        }
        return lookupViewModels;
    }
    [HttpPost]
    public override IActionResult Create(LookupViewModel lookupViewModel,string crud)
    {
        //Check name of Lookup Exists
        string name = lookupViewModel.Name;
        string pathViewMessage = GetViewPathMessageModel();

        var messageViewModel = new CRUDMessageViewModel();
        if (ModelState.IsValid)
        {
            //Search in database 
            if(!ModelExists(name))
            {
                var modelTModel = BuildModel(lookupViewModel);
                _table.Add(modelTModel);
                _context.SaveChanges();
                messageViewModel = BuildMessageModel(true,"Đã thêm");
                messageViewModel.ReturnUrl ="/" + GetControllerName() + "/Index";
                return PartialView(pathViewMessage, messageViewModel); 
            }
            else
            {   
                string message = GetNamePage() + " đã tồn tại";
                messageViewModel= BuildMessageModel(false, message);
                return PartialView(pathViewMessage, messageViewModel); 
            }
        }
        else
        {
            return PartialView("_Create", lookupViewModel);
        }
    }
    [HttpGet]
    public override IActionResult Delete(int id)
    {
        ViewData["ControllerName"] = GetControllerName();
        var pathDelete = GetViewPathDelete();
        var model = _table.Find(id);
        if (model == null)
        {
            return NotFound();
        }
        else
        {
            var viewModel = BuildViewModel(model);
            return PartialView(pathDelete, viewModel);
        }
        
    }
    protected abstract bool ModelExists(string name);
}