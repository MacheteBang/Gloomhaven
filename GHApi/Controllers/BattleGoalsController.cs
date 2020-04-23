using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GHApi.Models;

namespace GHApi.Controllers
{
    [Route("gh/[controller]")]
    [ApiController]
    public class BattleGoalsController : ControllerBase
    {
        private readonly GHContext db;

        public BattleGoalsController(GHContext context)
        {
            db = context;
        }

        // GET: api/BattleGoals
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BattleGoalDTO>>> GetBattleGoals()
        {
            var battleGoals = from b in db.BattleGoals
                              select new BattleGoalDTO()
                              {
                                  CardNumber = b.CardNumber,
                                  GoalName = b.GoalName,
                                  GoalDescription = b.GoalDescription,
                                  Reward = b.Reward,
                                  Source = b.Source,
                                  IsOfficial = b.IsOfficial,
                                  IsExtended = b.IsExtended
                              };

            return await battleGoals.ToListAsync();
        }

        // GET: api/BattleGoals/5
        [HttpGet("{cardNumber}")]
        public async Task<ActionResult<BattleGoalDTO>> GetBattleGoal(string cardNumber)
        {
            var battleGoal = await db.BattleGoals.Select(b =>
                new BattleGoalDTO()
                {
                    CardNumber = b.CardNumber,
                    GoalName = b.GoalName,
                    GoalDescription = b.GoalDescription,
                    Reward = b.Reward,
                    Source = b.Source,
                    IsOfficial = b.IsOfficial,
                    IsExtended = b.IsExtended
                }).SingleOrDefaultAsync(b => b.CardNumber == cardNumber);

            if (battleGoal == null)
            {
                return NotFound();
            }

            return Ok(battleGoal);
        }

        //// PUT: api/BattleGoals/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for
        //// more details see https://aka.ms/RazorPagesCRUD.
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutBattleGoal(long id, BattleGoal battleGoal)
        //{
        //    if (id != battleGoal.BattleGoalId)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(battleGoal).State = EntityState.Modified;

        //    try
        //    {
        //        await db.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!BattleGoalExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //// POST: api/BattleGoals
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for
        //// more details see https://aka.ms/RazorPagesCRUD.
        //[HttpPost]
        //public async Task<ActionResult<BattleGoal>> PostBattleGoal(BattleGoal battleGoal)
        //{
        //    db.BattleGoals.Add(battleGoal);
        //    await db.SaveChangesAsync();

        //    return CreatedAtAction(nameof(GetBattleGoal), new { id = battleGoal.BattleGoalId }, battleGoal);
        //}

        //// DELETE: api/BattleGoals/5
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<BattleGoal>> DeleteBattleGoal(long id)
        //{
        //    var battleGoal = await db.BattleGoals.FindAsync(id);
        //    if (battleGoal == null)
        //    {
        //        return NotFound();
        //    }

        //    db.BattleGoals.Remove(battleGoal);
        //    await db.SaveChangesAsync();

        //    return battleGoal;
        //}

        private bool BattleGoalExists(long id)
        {
            return db.BattleGoals.Any(e => e.BattleGoalId == id);
        }
    }
}
