using System.Threading.Tasks;

namespace SpellingTest.Core.Interfaces;

public interface IAuthenticationClient
{
    Task LoginAsync();
}