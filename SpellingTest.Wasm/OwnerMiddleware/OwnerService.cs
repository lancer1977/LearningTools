using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SpellingTest.Wasm.OwnerMiddleware;

public class OwnerService : IIdentityService
{
    public Guid OwnerId { get; set; }
    public string? AuthorizationToken { get; set; }
    public string RefreshToken { get; set; }

    public bool IsExpired()
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(AuthorizationToken);
        var exp = token.Claims.First(claim => claim.Type == ClaimTypes.Expiration).Value;
        var expirationDate = DateTime.Parse(exp);
        return expirationDate > DateTime.UtcNow;
    }

    public async Task<Guid> GetOwnerId()
    {
        throw new NotImplementedException();
    }

    public async Task ForceRefresh()
    {
        throw new NotImplementedException();
    }

    public IObservable<Guid> OwnerIdObservable { get; set; }


    public void OnLogout()
    {
        throw new NotImplementedException();
    }
}