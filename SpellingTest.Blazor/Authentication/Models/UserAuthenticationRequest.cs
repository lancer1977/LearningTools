namespace SpellingTest.Web.Authentication.Models;

public class UserAuthenticationRequest
{
    public string Email { get; set; }
    public string AccessToken { get; set; }
    public string Provider { get; set; }
    public string Name { get; set; }
    public Guid UserId { get; set; } //Optional 

}