using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GHApi.Models;
using GHApi.Models.Context;

namespace GHApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class BattleGoalsController : ControllerBase
    {
        // Private Variables
        // -----------------
        private readonly GHApiContext _context;

        // Constructors
        // ------------
        public BattleGoalsController(GHApiContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all Battle Goals.
        /// </summary>
        /// <remarks>
        /// Inclusive of Satire's Extended Battle Goals.
        /// </remarks>
        /// <returns>
        /// All Battel Goals.
        /// </returns>
        /// <response code = "200">Battle Goals returned.</response>
        /// <response code = "404">No Battle Goals found.</response>
        // GET: api/BattleGoals
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BattleGoal>>> GetBattleGoals()
        {
            var battleGoals = from b in _context.BattleGoals
                              select new BattleGoal()
                              {
                                  CardNumber = b.CardNumber,
                                  GoalName = b.GoalName,
                                  GoalDescription = b.GoalDescription,
                                  Reward = b.Reward,
                                  Source = b.Source,
                                  IsOfficial = b.IsOfficial,
                                  IsExtended = b.IsExtended
                              };

            return Ok(await battleGoals.ToListAsync());
        }

        /// <summary>
        /// Retrieves a specific Battle Goal based on card number.
        /// </summary>
        /// <param name="cardNumber">Card Number</param>
        /// <remarks></remarks>
        /// <returns>
        /// A single Battle Goal.
        /// </returns>
        /// <response code = "200">Returns the requested Battle Goal.</response>
        /// <response code = "404">Battle Goal wasn't found.</response>
        [HttpGet("{cardNumber}")]
        public async Task<ActionResult<BattleGoal>> GetBattleGoal(string cardNumber)
        {
            var battleGoal = await _context.BattleGoals.Select(b =>
                            new BattleGoal()
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
