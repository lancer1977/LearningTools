using MerriamWebster.NET;

namespace SpellingTest.Core.Service
{
    public class DictionaryService : IDictionaryService
    {
        public MerriamWebsterSearch _search;
        public DictionaryService(MerriamWebsterSearch search)
        {
            _search = search;
            //var dict = new MerriamWebster.NET.MerriamWebsterConfig()
            //{
            //    ApiKey = Constants.WebsterKey
            //};
            //var loggerFactory = new LoggerFactory();
            //var client = new MerriamWebster.NET.MerriamWebsterClient(service.CreateHttp(), dict,
            //    new Logger<MerriamWebsterClient>(loggerFactory));


        }

        public async Task<string> GetAsync(string word)
        {
            var result = await _search.Search(Constants.WebsterKey, word);
            return result.Entries.FirstOrDefault().ShortDefs.ToCodedArray(Environment.NewLine);
        }



    }
}


