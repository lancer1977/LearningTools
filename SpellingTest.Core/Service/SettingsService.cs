using PolyhydraGames.Core.Global.Settings;

namespace SpellingTest.Core.Service;

public class SettingsService : ISettingsService
{

    private StringSetting _name;
    public string Name
    {
        get => _name.Value;
        set => _name.Value = value;
    }
    public SettingsService(ISettings settings)
    {
        _name = new StringSetting(settings, "Name", "Peeblo");
    }
}