using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GHApi.Models;

namespace GHApi.Controllers
{
    [Produces("application/json")]
    [Route("gh/[controller]")]
    [ApiController]
    public class BattleGoalsController : ControllerBase
    {
        private readonly GHContext db;

        public BattleGoalsController(GHContext context)
        {
            db = context;
        }

        /// <summary>
        /// Gets all Battle Goals available.
        /// </summary>
        /// <remarks>
        /// Inclusive of Satire's Extended Battle Goals.
        /// </remarks>
        /// <returns></returns>
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

        /// <summary>
        /// Gets a specific Battle Goal based on number/identifier.
        /// </summary>
        /// <remarks>
        /// Includes all Battle Goals inclusive of some third party battle goals.
        ///     For base game, the format is: 0 (the number with no leading zeroes)
        ///     For Satire's Extended Battle Goals, the format is: SE-000 (the number *does* have preceding zeroes with three places represented)
        /// </remarks>
        /// <param name="cardNumber"></param>
        /// <returns>A list of Battle Goals.</returns>
        /// <response code = "200">Returns the requested Battle Goal.</response>
        /// <response code = "404">Battle Goal wasn't found.</response>
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
    }
}
