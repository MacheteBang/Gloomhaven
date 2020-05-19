using System.Collections.Generic;

namespace GHApi.Models.Context
{
    public class EventEntity
    {
        // Properties
        public int EventEntityId { get; set; }
        public string Type { get; set; }
        public string CardNumber { get; set; }
        public string Situation { get; set; }

        // Child Properties
        public List<EventOptionEntity> Options { get; set; }
    }
    public class EventOptionEntity
    {
        // Properties
        public int EventOptionEntityId { get; set; }
        public char Letter { get; set; }
        public string Description { get; set; }

        // Child Properties
        public List<EventResultEntity> Results { get; set; }

        // Parent Property
        public long EventEntityId { get; set; }
        public EventEntity EventEntity { get; set; }
    }
    public class EventResultEntity
    {
        // Properties
        public int EventResultEntityId { get; set; }
        public string Description { get; set; }
        public string CardDestiny { get; set; }

        // Child Properties
        public List<EventRequirementEntity> Requirements { get; set; }
        public List<EventRewardEntity> Rewards { get; set; }

        // Parent Property
        public int EventOptionEntityId { get; set; }
        public EventOptionEntity EventOption { get; set; }

    }
    public class EventRequirementEntity
    {
        // Properties
        public int EventRequirementEntityId { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string CharacterNumber { get; set; }

        // Parent Property
        public int EventResultEntityId { get; set; }
        public EventResultEntity EventResult { get; set; }
    }
    public class EventRewardEntity
    {
        // Properties
        public int EventRewardEntityId { get; set; }
        public string Reward { get; set; }

        // Parent Property
        public int EventResultEntityId { get; set; }
        public EventResultEntity EventResult { get; set; }
        }
}
