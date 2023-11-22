namespace SpellingTest.Wasm.Models;

public class YTDLPItem
{
    public string YTDLPId { get; set; }
    public Guid OwnerId { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }
    public DateTime LastSyncDate { get; set; }
}