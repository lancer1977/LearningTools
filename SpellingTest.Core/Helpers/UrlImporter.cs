namespace SpellingTest.Core.Helpers
{
    public static class Formatters
    {
        public static string ToCapitalizedString(this string value)
        {
            if (value.Length == 0) return value;
            string returnValue = string.Empty;
            for (var x= 0;x < value.Length; x++)
            { 
                if (x == 0)
                    returnValue += char.ToUpper(value[0]);
                else
                {
                    if(value[x -1] == ' ')
                        returnValue += char.ToUpper(value[x]);
                    else
                    {
                        returnValue +=  value[x];
                    }
                }
            }
            return returnValue;
        } 
    }
} 