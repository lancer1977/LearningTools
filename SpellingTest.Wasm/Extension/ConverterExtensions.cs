using System.Security.Claims;
using Blazorise;

namespace SpellingTest.Wasm.Extension;

public static class ConverterExtensions
{
    public static bool IsExpired2(this IEnumerable<Claim>? claims)
    {
        var claim = claims?.FirstOrDefault(claim => claim.Type == "exp");
        if (claim == null) throw new Exception("Wrong claim for exp date?");
        var count = long.Parse(claim.Value) ;
        var tokenDate = DateTimeOffset.FromUnixTimeSeconds(count).UtcDateTime;
         
        return tokenDate < DateTime.UtcNow;
    }
 
    public static Visibility ToVisibility(this bool? value)
    {
        return value.HasValue && value.Value ? Visibility.Visible : Visibility.Invisible;
    }
}