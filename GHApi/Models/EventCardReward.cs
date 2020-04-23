using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GHApi.Models
{
    public class EventCardReward
    {
        [Key]
        public long EventCardRewardId { get; set; }

        [ForeignKey("EventCardResultId")]
        public long EventCardResultId { get; set; }
        public EventCardResult EventCardResult { get; set; }

        public string Reward { get; set; }

    }
}
