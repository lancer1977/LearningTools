namespace SpellingTest.Wasm.Services.Fakes;

    public class MainThreadDispatcher : IMainThreadDispatcher
    {
        public void InvokeOnMainThread(Action action)
        {
            action.Invoke();
        }
    }

    public class TextToSpeechServiceFake : ITextToSpeech
    {
        public void Speak(string text)
        {
            throw new NotImplementedException();
        }

        public async Task SpeakAsync(string text)
        {
            throw new NotImplementedException();
        }

        public void Rate(double newvalue)
        {
            throw new NotImplementedException();
        }

        public void ChangeVoice(string voice)
        {
            throw new NotImplementedException();
        }

        public string[] VoiceNames()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public IObservable<PlaybackState> PlaybackState { get; set; }
    }