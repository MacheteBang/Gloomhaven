using System.Collections.Generic;

namespace GHApi.Models
{
    public class EventCardOptionDTO
    {
        public char OptionLetter { get; set; }
        public string OptionDescription { get; set; }
        public ICollection<EventCardResultDTO> EventCardResults { get; set; }
    }
}
