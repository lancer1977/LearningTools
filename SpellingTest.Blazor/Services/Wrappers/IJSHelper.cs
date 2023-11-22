namespace SpellingTest.Web.Services.Wrappers;

public interface IJSHelper
{
    Task SaveFileToDisk(string fileName, Stream fileStream);
    //Task Alert(string message);

    Task<bool> Confirm(string message, string title);
    //Task<string> Prompt(string message);
}