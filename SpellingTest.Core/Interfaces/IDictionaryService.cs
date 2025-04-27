namespace SpellingTest.Core.Interfaces
{
    public interface IDictionaryService
    {
        Task<string> GetAsync(string word);
    }


}