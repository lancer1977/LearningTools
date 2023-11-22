namespace SpellingTest.Web.Services;

public class HttpFinder : IHttpService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IOwnerService _ownerService;
    private static int count = 0;
    public HttpFinder(IHttpClientFactory clientFactory ,IOwnerService ownerService)
    {
        _clientFactory = clientFactory;
        _ownerService = ownerService;
        count += 1;
        Debug.WriteLine($"HttpFinder {count}"); 
    }

    public async Task<string> GetAuthToken()
    {
        var token =   _ownerService.AuthorizationToken;
        Debug.WriteLine($"AuthorizationToken: {token}");
        return token;
    }


    public HttpClient GetClient  => _clientFactory.CreateClient();
}