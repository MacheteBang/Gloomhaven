using System.Collections.Generic;

namespace GHApi.Models
{
    public class EventCardDTO
    {
        public string EventType { get; set; }
        public string CardNumber { get; set; }
        public string Situation { get; set; }
        public ICollection<EventCardOptionDTO> EventCardOptions { get; set; }
    }
}
