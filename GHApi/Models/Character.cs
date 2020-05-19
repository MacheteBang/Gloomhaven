namespace GHApi.Models
{
    public class Character
    {
        public string CharacterNumber { get; set; }
        public string SpoilerFreeName { get; set; }
        public string FullName { get; set; }
        public string Race { get; set; }
        public string RaceDescription { get; set; }
        public string Class { get; set; }
        public string ClassDescription { get; set; }
        public string HexColor { get; set; }
        public string PortraitHigh { get; set; }
        public string PortraitLow { get; set; }
        public string IconHigh { get; set; }
        public string IconLow { get; set; }
        public bool IsSpoiler { get; set; }
        public bool IsOfficial { get; set; }
        public bool IsExtended { get; set; }
        public string Source { get; set; }
    }
}
