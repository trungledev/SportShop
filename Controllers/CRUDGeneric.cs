using Microsoft.EntityFrameworkCore;

namespace SportShop.Controllers;

public abstract class CRUDGeneric<TModel, TViewModel, TTypeId> : Controller
    where TModel : class
    where TViewModel : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<TModel> _table;
    protected string _viewPath = String.Empty;
    protected CRUDGeneric(ApplicationDbContext context, string viewPath)
    {
        _context = context;
        _table = _context.Set<TModel>();
        if (_viewPath == String.Empty)
            _viewPath = viewPath;
    }
    protected virtual string GetViewPathIndex() => _viewPath + "Index.cshtml";
    protected virtual string GetViewPathCreate() => _viewPath + "_CreateOrUpdate.cshtml";
    protected virtual string GetViewPathRead() => _viewPath + "Detail.cshtml";
    protected virtual string GetViewPathUpdate() => _viewPath + "_CreateOrUpdate.cshtml";
    protected virtual string GetViewPathDelete() => _viewPath + "_Delete.cshtml";
    protected virtual string GetViewPathMessageModel() => _viewPath + "_Message.cshtml";

    protected abstract string GetControllerName();
    protected abstract string GetNamePage();
    protected abstract TTypeId GetIdModel(TModel model);
    protected abstract TModel BuildModel(TViewModel viewModel);
    //Get data only 1 ViewModel from 1 DataModel
    protected abstract TViewModel BuildViewModel(TModel? model);

    //Get data for ListViewModel from ListDataModel
    protected abstract IEnumerable<TViewModel> BuildListViewModel(IEnumerable<TModel> models);
    protected CRUDMessageViewModel BuildMessageModel(bool isSuccess, string message)
    {
        var model = new CRUDMessageViewModel();
        model.Success = isSuccess == true ? 1 : 0;
        model.Message = message;
        return model;
    }
    public virtual IActionResult Index()
    {
        string pageName = GetNamePage();
        string controllerName = GetControllerName();
        ViewData["PageName"] = pageName;
        ViewData["ControllerName"] = controllerName;
        var allData = _table.ToList();
        var viewPathIndex = GetViewPathIndex();
        //Get data for ViewModel 
        var viewModel = BuildListViewModel(allData);
        if (viewPathIndex != String.Empty)
            return View(viewPathIndex, viewModel);
        else
        {
            //Loi Path to view
            return View();
        }
    }
    public virtual IActionResult Read(TTypeId id)
    {
       
        var pathViewRead = GetViewPathRead();
        if (pathViewRead != null)
            return View(pathViewRead);
        else
            return View("~/");
    }
    [HttpGet]
    public virtual IActionResult Create(string crud)
    {
        //Code
        ViewData["Method"] = crud;
        ViewData["ControllerName"] = GetControllerName();
        var pathViewCreate = GetViewPathCreate();
        var viewData = BuildViewModel(null);
        if (pathViewCreate != String.Empty)
        {
            return PartialView(pathViewCreate, viewData);
        }
        else
        {
            return PartialView(viewData);
        }
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual IActionResult Create(TViewModel viewModel, string crud)
    {
        
        ViewData["Method"] = crud;
        ViewData["ControllerName"] = GetControllerName();
        var pathViewCreate = GetViewPathCreate();
        var pathViewMessage = GetViewPathMessageModel();
        CRUDMessageViewModel message = new CRUDMessageViewModel();
        var model = BuildModel(viewModel);
        viewModel = BuildViewModel(model);
        if (ModelState.IsValid)
        {
            _table.Add(model);
            _context.SaveChanges();
            message = BuildMessageModel(true, "Da them " + GetControllerName());
            message.ReturnUrl = "/" + GetControllerName() + "/Index";
            return PartialView(pathViewMessage, message);
        }
        else
        {
            //Show error model
            return PartialView(pathViewCreate, viewModel);
        }
    }
    [AllowAnonymous]
    [HttpGet]
    public virtual IActionResult Detail(TTypeId id)
    {
        var pathRead = GetViewPathRead();
        ViewData["IsSigned"] = User.Identity!.IsAuthenticated;
        var dataModel = _table.Find(id);
        if (dataModel == null)
        {
            return NotFound();
        }
        else
        {
            var viewData = BuildViewModel(dataModel);
            if (pathRead != String.Empty && viewData != null)
            {
                return View(pathRead, viewData);
            }
            return PartialView(pathRead);
        }
    }
    [HttpGet]
    public virtual IActionResult Update(TTypeId id, string crud)
    {
        ViewData["Method"] = crud;
        ViewData["ControllerName"] = GetControllerName();
        var pathUpdate = GetViewPathUpdate();
        var dataModel = _table.Find(id);
        if (dataModel == null)
        {
            return NotFound();
        }
        else
        {
            var viewData = BuildViewModel(dataModel);
            return PartialView(pathUpdate, viewData);
        }

    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual IActionResult Update(TViewModel viewModel,string crud)
    {
        
        ViewData["Method"] = crud;
        ViewData["ControllerName"] = GetControllerName();
        var pathUpdate = GetViewPathUpdate();
        string pathViewMessage = GetViewPathMessageModel();
        var messageViewModel = new CRUDMessageViewModel();
        var message = String.Empty;
        if (ModelState.IsValid)
        {
            var model = BuildModel(viewModel);
            if (model != null)
            {
                _table.Update(model);
                _context.SaveChanges();
                message = "Thao tác thành công";
                messageViewModel = BuildMessageModel(true, message);
                messageViewModel.ReturnUrl = "/" + GetControllerName() + "/Index";
                return PartialView(pathViewMessage, messageViewModel);
            }
            else
            {
                return NotFound();
            }

        }
        else
        {
            //Show error model
            return PartialView(pathUpdate, viewModel);
        }

    }
    [HttpGet]
    public virtual IActionResult Delete(TTypeId id)
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
    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual IActionResult Delete(int? id, TViewModel? viewModel)
    {
        var pathDelete = GetViewPathDelete();
        var messageModel = new CRUDMessageViewModel();


        if (id != 0)
        {
            var model = _table.Find(id);
            if (model == null)
            {
                return NotFound();
            }
            else
            {
                _table.Remove(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
        }
        else if (viewModel != null)
        {
            var model = BuildModel(viewModel);
            model = _table.Find(GetIdModel(model));
            if (model != null)
            {
                _table.Remove(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
        }
        return BadRequest();
    }
    [HttpGet]
    public virtual IActionResult DeleteAll()
    {
        //Code
        var pathDelete = GetViewPathDelete();
        return PartialView(pathDelete);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual IActionResult DeleteAll(TTypeId id)
    {
        _table.RemoveRange(_table.ToArray());
        _context.SaveChanges();
        return Ok();
    }
}