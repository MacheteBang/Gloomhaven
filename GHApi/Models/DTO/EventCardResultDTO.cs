using System.Collections.Generic;

namespace GHApi.Models
{
    public class EventCardResultDTO
    {
        public string Result { get; set; }
        public string CardDestiny { get; set; }
        public ICollection<EventCardRequirementDTO> EventCardRequirement { get; set; }
        public ICollection<EventCardRewardDTO> EventCardReward { get; set; }
    }
}
