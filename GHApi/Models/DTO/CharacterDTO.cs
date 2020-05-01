namespace GHApi.Models
{
    public class CharacterDTO
    {
        public string CharacterNumber { get; set; }
        public string FullName { get; set; }
        public string Race { get; set; }
        public string Class { get; set; }
        public string SpoilerFreeName { get; set; }
        public bool IsSpoiler { get; set; }
        public bool IsOfficial { get; set; }
        public bool IsExtended { get; set; }
    }
}
