using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GHApi.Models
{
    public class EventCardOption
    {
        [Key]
        public long EventCardOptionId { get; set; }

        [ForeignKey("EventCardId")]
        public long EventCardId { get; set; }
        public EventCard EventCard { get; set; }

        public char OptionLetter { get; set; }
        public string OptionDescription { get; set; }

        public ICollection<EventCardResult> EventCardResults { get; set; }
    }
}
