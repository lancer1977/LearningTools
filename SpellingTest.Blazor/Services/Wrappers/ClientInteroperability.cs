using Microsoft.JSInterop;

namespace SpellingTest.Web.Services.Wrappers;

public class ClientInteroperability : IJSHelper
{
    private readonly IJSRuntime _jsRuntime;

    public ClientInteroperability(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task SaveFileToDisk(string fileName, Stream fileStream)
    {
        using var streamRef = new DotNetStreamReference(stream: fileStream);

        try
        {

            await _jsRuntime.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    public async Task Alert(string message)
    {
        await _jsRuntime.InvokeVoidAsync("alert", message);
    }

    public async Task<bool> Confirm(string message, string title)
    {
        return await _jsRuntime.InvokeAsync<bool>("confirm", title + ": " + message);
    }

    public async Task<string> Prompt(string message)
    {
        return await _jsRuntime.InvokeAsync<string>("prompt", message);
    }
}