using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GHApi.Models
{
    public class EventCardResult
    {
        [Key]
        public long EventCardResultId { get; set; }

        [ForeignKey("EventCardOptionId")]
        public long EventCardOptionId { get; set; }
        public EventCardOption EventCardOption { get; set; }

        public string Result { get; set; }
        public string CardDestiny { get; set; }

        public ICollection<EventCardRequirement> EventCardRequirement { get; set; }
        public ICollection<EventCardReward> EventCardReward { get; set; }
    }
}
