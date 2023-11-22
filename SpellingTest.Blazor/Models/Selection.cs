namespace SpellingTest.Web.Models;

public class Selection<T> 
{
    private string _title;
    public T Value { get; set; }

    public string Title
    {
        get;
        set;
    }

    public Selection()
    {

    }

    public Selection(T value) : this(value,value.ToString())
    {
    }

    public Selection(T value,string title)
    {
        Value = value;
        Title = title;
    }
}