namespace SpellingTest.Web.Models
{
    [Serializable]
    public class ExpandedLootResult
    {
        public string Level { get; set; }
        public int Id { get; set; }
        public string Type { get; set; }
        public string Results { get; set; }
    }
}