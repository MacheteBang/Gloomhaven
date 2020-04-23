using Microsoft.EntityFrameworkCore;

namespace GHApi.Models
{
    public class GHContext : DbContext
    {
        public GHContext(DbContextOptions<GHContext> options) : base(options)
        {

        }

        public DbSet<BattleGoal> BattleGoals { get; set; }

        public DbSet<Character> Characters { get; set; }

        public DbSet<EventCard> EventCards { get; set; }
        public DbSet<EventCardOption> EventCardOptions { get; set; }
        public DbSet<EventCardResult> EventCardResults { get; set; }
        public DbSet<EventCardRequirement> EventCardRequirements { get; set; }
        public DbSet<EventCardReward> EventCardRewards { get; set; }

    }
}