using IdentityModel.Client;
using IdentityModel.OidcClient.Browser;
using PolyhydraGames.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.OidcClient;

namespace SpellingTest.Core.Service;

public class AuthenticationService : IAuthenticationClient
{
    public string AuthenticationToken { get; private set; }
    public AuthenticationService(OidcClient client)
    {
        _client = client;
    }
    private readonly OidcClient _client;

    public async Task LoginAsync()
    {
        var result = await _client.LoginAsync();



        AuthenticationToken = result.AccessToken;
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
