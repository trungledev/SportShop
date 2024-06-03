namespace SportShop.Controllers;

public class ProducerProductsController : CRUDGeneric<ProducerProduct, ProducerViewModel, int>
{
    public ProducerProductsController(ApplicationDbContext context) : base(context, "/Views/Producer/")
    {

    }

    protected override string GetControllerName()
    {
        return "ProducerProducts";
    }
    protected override string GetNamePage()
    {
        return "Producer";
    }
    protected override int GetIdModel(ProducerProduct model)
    {
        return model.Id;
    }
    protected override ProducerProduct BuildModel(ProducerViewModel viewModel)
    {
        return new ProducerProduct()
        {
            Id = viewModel.Id,
            Name = viewModel.Name,
            // AddressId = GetAddressId(viewModel.Address),
            PhoneNumber = viewModel.PhoneNumber,
        };
    }

    private int GetAddressId(string addressStr)
    {
        var model = _context.Producers.Where(x => x.Name == addressStr).FirstOrDefault();
        if (model != null)
        {
            return model.AddressId;
        }
        else
        {
            return 0;
        }
    }

    //Get data only 1 ViewModel from 1 DataModel
    protected override ProducerViewModel BuildViewModel(ProducerProduct? model)
    {
        if (model == null)
        {
            //Not model in parameter for method Create() 
            // Data property default or empty or null
            return new ProducerViewModel();
        }
        else
        {
            return new ProducerViewModel()
            {
                Id = model.Id,
                Name = model.Name,
                // Address = GetNameAddress(model.AddressId),
                PhoneNumber = model.PhoneNumber
            };
        }
    }

    private string GetNameAddress(int id)
    {
        var model = _context.Addresses.Find(id);
        if (model != null)
        {
            return " Street: " + model.Street +", Province: "+ model.Province +", City: "+ model.City;
        }
        else
        {
            return string.Empty;
        }
    }

    //Get data for ListViewModel from ListDataModel
    protected override IEnumerable<ProducerViewModel> BuildListViewModel(IEnumerable<ProducerProduct> models)
    {
        var list = new List<ProducerViewModel>();
        foreach (var item in models)
        {
            list.Add(BuildViewModel(item));
        }
        return list;
    }
}