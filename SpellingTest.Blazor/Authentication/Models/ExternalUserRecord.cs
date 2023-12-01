namespace SpellingTest.Web.Authentication.Models;

/// <summary>
/// This can be left here, the interface is in the core library so we can export this class elsewhere.
/// </summary>
public class ExternalUserRecord : IExternalUserRecord
{
    public ExternalUserRecord()
    {

    }


    public ExternalUserRecord(UserAuthenticationRequest request)
    {
        AccessToken = request.AccessToken;
        Provider = request.Provider;
        Email = request.Email;
        UserId = request.UserId;
    }
    public Guid UserId { get; set; }
    public string Provider { get; set; }
    public string Email { get; set; }
    public string AuthToken { get; }
    public string AccessToken { get; set; }
}