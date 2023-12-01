using MediaManager;
using System.Diagnostics;

namespace SpellingTest.Maui;

public class AudioService : IAudioService
{
    public bool MultiSound { get; set; }
    public bool DisableAudio { get; set; } = true;
    public async Task PlaySound(string title)
    {
        if (DisableAudio) return;
        try
        {
            await CrossMediaManager.Current.PlayFromAssembly($"Audio.{title}.mp3", typeof(SpellingTest.Models.Operator).Assembly);
            await CrossMediaManager.Current.Play("https://ia804604.us.archive.org/19/items/crab-rave_202111/Crab%20Rave.mp3");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }

    }

    public async Task PlayMusic(string title, bool loop = true)
    {
        if (DisableAudio) return;
        try
        {
            //Song = new MediaItem(title);
            await CrossMediaManager.Current.PlayFromResource($"assets:///{title}.mp3");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    public async Task StopMusic()
    {
        throw new NotImplementedException();
        await Task.Delay(0);
    }

    public async Task PauseMusic()
    {
        throw new NotImplementedException();
        await Task.Delay(0);
    }
}