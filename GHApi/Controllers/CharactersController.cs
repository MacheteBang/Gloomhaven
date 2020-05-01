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
    public class CharactersController : ControllerBase
    {
        private readonly GHContext db;

        public CharactersController(GHContext context)
        {
            db = context;
        }

        /// <summary>
        /// Gets a list of characters.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CharacterDTO>>> GetCharacters()
        {
            var characters = from b in db.Characters
                             select new CharacterDTO()
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

            return await characters.ToListAsync();
        }

        /// <summary>
        /// Gets a specific character based upon it's "number".
        /// </summary>
        /// <param name="characterNumber"></param>
        /// <returns>The requested character.</returns>
        /// <response code = "200">Returns the requested character.</response>
        /// <response code = "404">Character wasn't found.</response>
        [HttpGet("{characterNumber}")]
        public async Task<ActionResult<CharacterDTO>> GetCharacter(string characterNumber)
        {

            // characterNumber should be a two character number formatted with a leading 0.
            // Reformat as necessary to facilitate the join correctly then continue on if
            // it is truly an integer.
            if (int.TryParse(characterNumber, out int i))
            {
                // Success! It's an integer.  Now reformat it to include a leading zero.
                characterNumber = i.ToString("D2");

                var character = await db.Characters.Select(b =>
                                 new CharacterDTO()
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
            } else
            {
                return NotFound();
            }
        }
    }
}
