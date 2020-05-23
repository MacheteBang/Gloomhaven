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
                                 CharacterCode = b.CharacterCode,
                                 SpoilerFreeName = b.SpoilerFreeName,
                                 FullName = b.FullName,
                                 Race = b.Race,
                                 RaceDescription = b.RaceDescription,
                                 Class = b.Class,
                                 ClassDescription = b.ClassDescription,
                                 HexColor = b.HexColor,
                                 PortraitHigh = b.PortraitHigh,
                                 PortraitLow = b.PortraitLow,
                                 IconHigh = b.IconHigh,
                                 IconLow = b.IconLow,
                                 IsSpoiler = b.IsSpoiler,
                                 IsOfficial = b.IsOfficial,
                                 IsExtended = b.IsExtended,
                                 Source = b.Source
                             };

            return Ok(await characters.ToListAsync());
        }

        /// <summary>
        /// Retrieves a specific Character based upon passed character number.
        /// </summary>
        /// <param name="characterCode">Character Number</param>
        /// <remarks></remarks>
        /// <returns>
        /// A single Character.
        /// </returns>
        /// <response code = "200">Returns the requested character.</response>
        /// <response code = "404">Character wasn't found.</response>
        [HttpGet("{characterCode}")]
        public async Task<ActionResult<Character>> GetCharacter(string characterCode)
        {
            var character = await _context.Characters.Select(b =>
                                new Character()
                                {
                                    CharacterCode = b.CharacterCode,
                                    SpoilerFreeName = b.SpoilerFreeName,
                                    FullName = b.FullName,
                                    Race = b.Race,
                                    RaceDescription = b.RaceDescription,
                                    Class = b.Class,
                                    ClassDescription = b.ClassDescription,
                                    HexColor = b.HexColor,
                                    PortraitHigh = b.PortraitHigh,
                                    PortraitLow = b.PortraitLow,
                                    IconHigh = b.IconHigh,
                                    IconLow = b.IconLow,
                                    IsSpoiler = b.IsSpoiler,
                                    IsOfficial = b.IsOfficial,
                                    IsExtended = b.IsExtended,
                                    Source = b.Source
                                }).SingleOrDefaultAsync(b => b.CharacterCode == characterCode);

            if (character == null)
            {
                return NotFound();
            }

            return Ok(character);
        }
    }
}
