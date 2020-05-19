using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace GHApi.Models.Context
{
    public class GHApiContext : DbContext
    {
        public GHApiContext(DbContextOptions<GHApiContext> options) : base(options)
        {

        }

        public void SeedDatabase()
        {
            List<BattleGoalEntity> battleGoals = JsonConvert.DeserializeObject<List<BattleGoalEntity>>(File.ReadAllText(@"data\BattleGoals.json"));
            if (!BattleGoals.Any())
            {
                AddRange(battleGoals);
                SaveChanges();
            }

            List<CharacterEntity> characters = JsonConvert.DeserializeObject<List<CharacterEntity>>(File.ReadAllText(@"data\Characters.json"));
            if (!Characters.Any())
            {
                AddRange(characters);
                SaveChanges();
            }

            List<EventEntity> events = JsonConvert.DeserializeObject<List<EventEntity>>(File.ReadAllText(@"data\Events.json"));
            if (!Events.Any())
            {
                AddRange(events);
                SaveChanges();
            }
        }

        public DbSet<BattleGoalEntity> BattleGoals { get; set; }
        public DbSet<CharacterEntity> Characters { get; set; }
        public DbSet<EventEntity> Events { get; set; }
        public DbSet<EventOptionEntity> EventOptions { get; set; }
        public DbSet<EventResultEntity> EventResults { get; set; }
        public DbSet<EventRequirementEntity> EventRequirements { get; set; }
        public DbSet<EventRewardEntity> EventRewards { get; set; }
    }
}
