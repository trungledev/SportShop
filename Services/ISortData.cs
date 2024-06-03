namespace SportShop.Services;


//  Sumary

//  Get data from source

//  Return Model in app

public interface ISortData<T> where T : class
{
    public IEnumerable<T> GetModelsCompareNumber(string nameOfProperty,double number,Compare compare);
    public IEnumerable<T> GetNumbericModels(int numberic);
}