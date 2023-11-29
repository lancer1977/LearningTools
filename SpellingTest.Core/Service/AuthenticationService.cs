using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.OidcClient;
using SpellingTest.Core.Interfaces;

namespace SpellingTest.Core.Service;

public class AuthenticationService(OidcClient client, IPolyhydraToken tokenService) : IAuthenticationClient
{
    public async Task LoginAsync()
    {
        var result = await client.LoginAsync();

        tokenService.Token = result.AccessToken;
        if (result.IsError)
        {
            Debug.WriteLine(result.Error);
        }
        var sb = new StringBuilder(128);

        sb.AppendLine("claims:");
        foreach (var claim in result.User.Claims)
        {
            sb.AppendLine($"{claim.Type}: {claim.Value}");
        }

        sb.AppendLine();
        sb.AppendLine("access token:");
        sb.AppendLine(result.AccessToken);

        if (!string.IsNullOrWhiteSpace(result.RefreshToken))
        {
            sb.AppendLine();
            sb.AppendLine("access token:");
            sb.AppendLine(result.AccessToken);
        } 

        Debug.WriteLine(sb.ToString());

    } 
}
