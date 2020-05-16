namespace GloomBot.Models.GloomhavenDB
{
    public class EventCard
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }
        public EventCardOption OptionA { get; set; }
        public EventCardOption OptionB { get; set; }
        public string ImageUrl { get; set; }
        public bool Verified { get; set; }
    }
}
