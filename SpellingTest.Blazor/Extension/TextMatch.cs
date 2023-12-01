using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Components.Forms;
using System.Globalization;

namespace SpellingTest.Web.Extension
{
    public static class TextMatch
    {

        public static async Task<IList<T>> ReadCsvItems<T>(this IBrowserFile file)
        {
            var path = Path.GetRandomFileName();
            var returnList = new List<T>();
            try
            {
                await using (FileStream fs = new(path, FileMode.Create))
                {
                    await file.OpenReadStream().CopyToAsync(fs);
                }



                using var reader = new StreamReader(new FileStream(path, FileMode.Open));
                var config = new CsvConfiguration(CultureInfo.CurrentCulture)
                {
                    Delimiter = ",",
                    HasHeaderRecord = true,
                    HeaderValidated = null,
                    MissingFieldFound = null,
                    BadDataFound = context =>
                        Debug.WriteLine($"Bad data found at row {context.RawRecord}\r\n" +
                                        $"Raw data: {context.RawRecord}")
                };
                using var csvreader = new CsvReader(reader, config);
                var records = csvreader.GetRecords<T>();

                returnList = records.ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                if (File.Exists(path))
                    File.Delete(path);
            }

            return returnList;
        }

        public static bool IsMatch(this string value, string test)
        {
            if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(test)) return false;
            return (value.Equals(test));
        }

        public static bool IsNotEmpty(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }




        public static IEnumerable<T> CsvImport<T>(this Stream stream)
        {
            //var stream = obj.File.OpenReadStream();
            //var stream = File.OpenRead("C:\\Code\\Websites\\Web.Test\\ROTRL.csv");
            using var reader = new StreamReader(stream);
            var config = new CsvConfiguration(CultureInfo.CurrentCulture)
            {
                HasHeaderRecord = true,
                HeaderValidated = null,
                Delimiter = ",",
                IgnoreBlankLines = true,
                MissingFieldFound = null,
            };
            using var csv = new CsvReader(reader, config);


            var records = csv.GetRecords<T>();
            return records.ToList();
        }

        //public static Stream WriteCsv<T>(IEnumerable<T> items)
        //{
        //    using (var writer = new Stream("path\\to\\file.csv"))
        //    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        //    {
        //        csv.WriteRecords(items);
        //    }
        //}
    }
}