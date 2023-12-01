namespace SpellingTest.Core.Service;

public class SettingsFake : ISettings
{
    public void AddOrUpdateValue(string key, bool value)
    {
    }

    public void AddOrUpdateValue(string key, int value)
    {
    }

    public void AddOrUpdateValue(string key, string value)
    {
    }

    public void AddOrUpdateValue(string key, double value)
    {
    }

    public double GetValueOrDefault(string key, double defaultValue)
    {
        return default;
    }

    public bool GetValueOrDefault(string key, bool defaultValue)
    {
        return default;
    }

    public int GetValueOrDefault(string key, int defaultValue)
    {
        return default;
    }

    public string GetValueOrDefault(string key, string defaultValue)
    {
        return default;
    }
}