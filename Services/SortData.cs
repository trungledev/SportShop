using System.Reflection;

namespace SportShop.Services;

/*
    Sumary
    Get data with request from List<Model>
    Request include: IDictionary<PropetyOfData,Condition> 
    Return List<Model>;
*/

public class SortData<TModel> : ISortData<TModel> where TModel : class
{
    private IEnumerable<TModel> ListAllModel { get; }
    private IEnumerable<TModel> ListModelRuntime { get; set; }

    public SortData(IList<TModel> ListData)
    {
        this.ListAllModel = ListData;
        ListModelRuntime = ListData;
    }
    public class ConditionModel
    {
        public ConditionModel(string nameOfProperty, double number, Compare compare)
        {
            this.NameOfProperty = nameOfProperty;
            this.Number = number;
            this.Compare = compare;
        }
        public string NameOfProperty { get; set; } = null!;
        public double Number { get; set; }
        public Compare Compare { get; set; }

    }
    public IEnumerable<TModel> GetModelsCompareNumber(string nameOfProperty, double number, Compare compare)
    {
        List<TModel> result = new List<TModel>();
        ConditionModel condition = new ConditionModel(nameOfProperty, number, compare);
        var list = this.ListAllModel;
        try
        {
            System.Type type = list.GetType();
            PropertyInfo? propertyInfo = type.GetProperty(condition.NameOfProperty);
            if (propertyInfo != null)
                foreach (var item in list)
                {
                    var value = propertyInfo.GetValue(item, null);
                    double _number = 0;
                    if (value != null)
                    {
                        _number = (double)value;
                    }
                    //Goi ham so sanh IsGreaterOrLess
                    var isGreater = IsGreater(condition.Compare, _number, condition.Number);
                    if (isGreater)
                    {
                        result.Add(item);
                    }
                }
        }
        catch (NullReferenceException e)
        {
            //Write Error
            Console.WriteLine("The property does not exist in MyClass." + e.Message);
            throw new ArgumentNullException("Ten thuoc tinh khong phu hop");
        }
        this.ListModelRuntime = result;
        return result;
    }
    public IEnumerable<TModel> GetNumbericModels(int numberic)
    {
        IEnumerable<TModel> result = new List<TModel>();
        var list = this.ListAllModel;
        if (numberic >= list.Count())
        {
            //So luong yeu cau lon hon so luong danh sach => giu nguyen danh sach
            result = list;
        }
        else
        {
            //Lay phan dau cua danh sach voi so luong la numberic
            list.Take(numberic);
        }
        this.ListModelRuntime = result;
        return result;
    }
    private bool IsGreater(Compare compare, double number1, double number2)
    {
        bool result = (number1 > number2);
        switch (compare)
        {
            case Compare.Greater:
                return result;
            case Compare.Less:
                return !result;
            default:
                return false;
        }
    }

}
public enum Compare { Greater, Less }