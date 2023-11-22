using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace SpellingTest.Core
{
    public static class ResourceReader{

        public  static Assembly MyAssembly=> typeof(ResourceReader).GetTypeInfo().Assembly; 

        public static string GetDTD()
        {
            var address =   "SpellingTest.elementary.dtd";
            Debug.WriteLine(address);
            var stream = MyAssembly.GetManifestResourceStream(address);
            var text = "";
            using (var reader = new StreamReader(stream))
            {
                text = reader.ReadToEnd();
            }

            return text;
        }
    }
}