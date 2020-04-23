using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GHApi.Models
{
    public class EventCard
    {
        [Key]
        public long EventCardId { get; set; }
        public string EventType { get; set; }
        public string CardNumber { get; set; }
        public string Situation { get; set; }

        public ICollection<EventCardOption> EventCardOptions { get; set; }
    }
}
