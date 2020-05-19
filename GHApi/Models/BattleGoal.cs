namespace GHApi.Models
{
    public class BattleGoal
    {
        public string CardNumber { get; set; }
        public string GoalName { get; set; }
        public string GoalDescription { get; set; }
        public int Reward { get; set; }
        public string Source { get; set; }
        public bool IsOfficial { get; set; }
        public bool IsExtended { get; set; }
    }
}
