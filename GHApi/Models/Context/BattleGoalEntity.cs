namespace GHApi.Models.Context
{
    public class BattleGoalEntity
    {
        public int BattleGoalEntityId { get; set; }
        public string CardNumber { get; set; }
        public string GoalName { get; set; }
        public string GoalDescription { get; set; }
        public int Reward { get; set; }
        public string Source { get; set; }
        public bool IsOfficial { get; set; }
        public bool IsExtended { get; set; }
    }
}
