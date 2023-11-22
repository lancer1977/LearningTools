namespace SpellingTest.Core.Service;

public class TextToSpeechFake : ITextToSpeech
{
    public void Speak(string text)
    {
        
    }

    public void Rate(double newvalue)
    {
        
    }

    public void ChangeVoice(string voice)
    { 
    }

    public string[] VoiceNames()
    {
        return default;
    }

    public void Stop()
    { 
    }

    public PlaybackState Pause()
    {
        return default;
    }
}