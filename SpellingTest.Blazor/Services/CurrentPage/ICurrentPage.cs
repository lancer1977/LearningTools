namespace SpellingTest.Web.Services.CurrentPage;

public interface ICurrentPage
{
    void SetName(string name);
    IObservable<string> NameChanged { get; }
}