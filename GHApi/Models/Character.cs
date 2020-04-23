using System.ComponentModel.DataAnnotations;

namespace GHApi.Models
{
    public class Character
    {
        [Key]
        public long CharacterId { get; set; }
        public string CharacterNumber { get; set; }
        public string FullName { get; set; }
        public string Race { get; set; }
        public string Class { get; set; }
        public string SpoilerFreeName { get; set; }
        public bool IsSpoiler { get; set; }
    }
}
