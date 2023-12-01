using IdentityModel.Client;
using IdentityModel.OidcClient.Browser;
using SpellingTest.Core;
using System.Diagnostics;

namespace SpellingTest.Maui;

public class MauiAuthenticationBrowser : IdentityModel.OidcClient.Browser.IBrowser
{
    public async Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await WebAuthenticator.Default.AuthenticateAsync(
                new Uri(options.StartUrl),
                new Uri(options.EndUrl));

            var url = new RequestUrl(Constants.RedirectUri)
                .Create(new Parameters(result.Properties));

            return new BrowserResult
            {
                Response = url,
                ResultType = BrowserResultType.Success
            };
        }
        catch (TaskCanceledException)
        {
            return new BrowserResult
            {
                ResultType = BrowserResultType.UserCancel
            };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            return new BrowserResult
            {
                ResultType = BrowserResultType.UnknownError
            };
        }
    }



    public string ToRawIdentityUrl(string redirectUrl, WebAuthenticatorResult result)
    {
        try
        {
            IEnumerable<string> parameters = result.Properties.Select(pair => $"{pair.Key}={pair.Value}");
            var modifiedParameters = parameters.ToList();

            var stateParameter = modifiedParameters
                .FirstOrDefault(p => p.StartsWith("state", StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(stateParameter))
            {
                // Remove the state key added by WebAuthenticator that includes appInstanceId
                modifiedParameters = modifiedParameters.Where(p => !p.StartsWith("state", StringComparison.OrdinalIgnoreCase)).ToList();

                stateParameter = System.Web.HttpUtility.UrlDecode(stateParameter).Split('&').Last();
                modifiedParameters.Add(stateParameter);
            }
            var values = string.Join("&", modifiedParameters);
            return $"{redirectUrl}#{values}";
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            throw;
        }
    }
}