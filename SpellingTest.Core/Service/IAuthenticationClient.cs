using System.Threading.Tasks;

namespace SpellingTest.Core.Service;

public interface IAuthenticationClient
{
    Task LoginAsync();

    string AuthenticationToken { get;}
}