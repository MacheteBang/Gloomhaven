using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GHApi.Models
{
    public class EventCardRequirement
    {
        [Key]
        public long EventCardRequirementId { get; set; }

        [ForeignKey("EventCardResultId")]
        public long EventCardResultId { get; set; }
        public EventCardResult EventCardResult { get; set; }

        public string RequirementType { get; set; }
        public string RequirementDescription { get; set; }
        
        [ForeignKey("CharacterId")]
        public long CharacterId { get; set; }
        public Character Character { get; set; }


    }
}
