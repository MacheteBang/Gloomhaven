namespace GloomBot.Models.GloomhavenDB
{
    public class Item
    {
        public string Number { get; set; }
        public string Name { get; set;}
        public string Slot { get; set; }
        public string Price { get; set; }
        public string Text { get; set; }
        public string Count { get; set; }
        public string Limit { get; set; }
        public string Uses { get; set; }
        public string NegativeCardsCount { get; set; }
        public string SourceType { get; set; }
        public string SourceId { get; set; }
        public string ImageUrl { get; set; }
        public bool Verified { get; set; }
    }
}
