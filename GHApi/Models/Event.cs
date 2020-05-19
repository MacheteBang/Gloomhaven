using System.Collections.Generic;

namespace GHApi.Models
{
    public class Event
    {
        // Properties
        public string Type { get; set; }
        public string CardNumber { get; set; }
        public string Situation { get; set; }

        // Child Properties
        public List<EventOption> Options { get; set; }
    }
    public class EventOption
    {
        // Properties
        public char Letter { get; set; }
        public string Description { get; set; }

        // Child Properties
        public List<EventResult> Results { get; set; }
    }
    public class EventResult
    {
        // Properties
        public string Description { get; set; }
        public string CardDestiny { get; set; }

        // Child Properties
        public List<EventRequirement> Requirements { get; set; }
        public List<EventReward> Rewards { get; set; }
    }
    public class EventRequirement
    {
        // Properties
        public string Type { get; set; }
        public string Description { get; set; }
        public string CharacterNumber { get; set; }
    }
    public class EventReward
    {
        // Properties
        public string Reward { get; set; }
    }
}
