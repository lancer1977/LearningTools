using System;
using System.Linq;
using System.Threading.Tasks;
using MerriamWebster.NET;
using PolyhydraGames.Extensions;
using SpellingTest.Core.Interfaces;

namespace SpellingTest.Core.Service
{
    public class DictionaryService : IDictionaryService
    {
        public MerriamWebsterSearch _search;
        public DictionaryService(MerriamWebsterSearch search)
        {
            _search = search;
            var dict = new MerriamWebster.NET.MerriamWebsterConfig()
            {
                ApiKey = _key
            };
            //var loggerFactory = new LoggerFactory();
            //var client = new MerriamWebster.NET.MerriamWebsterClient(service.CreateHttp(), dict,
            //    new Logger<MerriamWebsterClient>(loggerFactory));
            

        }
        private static string _key = "0e9bba1f-ebc9-4978-b838-c4e58cefac20";

        public async Task<string> GetAsync(string word)
        {
            var result = await _search.Search(_key, word);
            return result.Entries.FirstOrDefault().ShortDefs.ToCodedArray(Environment.NewLine);
        }


 
    }
}


