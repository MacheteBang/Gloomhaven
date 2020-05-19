using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GHApi.Models;
using GHApi.Models.Context;

namespace GHApi.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class CharactersController : ControllerBase
    {
        // Private Variables
        // -----------------
        private readonly GHApiContext _context;

        // Constructors
        // ------------
        public CharactersController(GHApiContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all Characters.
        /// </summary>
        /// <remarks></remarks>
        /// <returns>
        /// All Characters.
        /// </returns>
        /// <response code = "200">Characters returned.</response>
        /// <response code = "404">No Characters found.</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Character>>> GetCharacters()
        {
            // Get all chacters in the database and return them as a DTO (hiding DB keys)
            var characters = from b in _context.Characters
                             select new Character()
                             {
                                 CharacterNumber = b.CharacterNumber,
                                 FullName = b.FullName,
                                 Race = b.Race,
                                 Class = b.Class,
                                 SpoilerFreeName = b.SpoilerFreeName,
                                 IsSpoiler = b.IsSpoiler,
                                 IsOfficial = b.IsOfficial,
                                 IsExtended = b.IsExtended
                             };

            return Ok(await characters.ToListAsync());
        }

        /// <summary>
        /// Retrieves a specific Character based upon passed character number.
        /// </summary>
        /// <param name="characterNumber">Character Number</param>
        /// <remarks></remarks>
        /// <returns>
        /// A single Character.
        /// </returns>
        /// <response code = "200">Returns the requested character.</response>
        /// <response code = "404">Character wasn't found.</response>
        [HttpGet("{characterNumber}")]
        public async Task<ActionResult<Character>> GetCharacter(string characterNumber)
        {

            // characterNumber should be a two character number formatted with a leading 0.
            // Reformat as necessary to facilitate the join correctly then continue on if
            // it is truly an integer.
            if (int.TryParse(characterNumber, out int i))
            {
                // Success! It's an integer.  Now reformat it to include a leading zero.
                characterNumber = i.ToString("D2");

                var character = await _context.Characters.Select(b =>
                                 new Character()
                                 {
                                     CharacterNumber = b.CharacterNumber,
                                     FullName = b.FullName,
                                     Race = b.Race,
                                     Class = b.Class,
                                     SpoilerFreeName = b.SpoilerFreeName,
                                     IsSpoiler = b.IsSpoiler,
                                     IsOfficial = b.IsOfficial,
                                     IsExtended = b.IsExtended
                                 }).SingleOrDefaultAsync(b => b.CharacterNumber == characterNumber);


                if (character == null)
                {
                    return NotFound();
                }

                return Ok(character);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
