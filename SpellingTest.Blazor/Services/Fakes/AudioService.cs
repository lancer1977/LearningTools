using Microsoft.JSInterop;

namespace SpellingTest.Web.Services.Fakes;

public class AudioServiceFake : IAudioService
{
    private readonly IJSRuntime _runtime;

    public AudioServiceFake(IJSRuntime runtime)
    {
        _runtime = runtime;
    }

    public async Task PlaySound(string uri)
    {
        await _runtime.InvokeVoidAsync("PlayAudioFile", $"/sounds/{uri}.mp3");
    }

    public async Task PlayMusic(string title, bool loop = true)
    {
        // await _runtime.InvokeVoidAsync("PlayAudioFile", $"/sounds/{title}.mp3");
    }

    public async Task StopMusic()
    {
        await _runtime.InvokeVoidAsync("StopMusic");
    }

    public async Task PauseMusic()
    {
        await _runtime.InvokeVoidAsync("PauseMusic");
    }

    public bool MultiSound { get; set; }
}