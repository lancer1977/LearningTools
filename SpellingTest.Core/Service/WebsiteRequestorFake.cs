using System;
using System.Threading.Tasks;

namespace SpellingTest.Core.Service;

public class WebsiteRequestorFake : IWebsiteRequestor
{
    public async Task RequestWebsite(string address)
    {
        throw new NotImplementedException();
    }
}
 