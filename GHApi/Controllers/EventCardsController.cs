using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using GHApi.Models;
using GHApi.Models.Context;

namespace GHApi.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class EventCardsController : ControllerBase
    {
        // Private Variables
        // -----------------
        private readonly GHApiContext _context;

        // Constructors
        // ------------
        public EventCardsController(GHApiContext context)
        {
            _context = context;
        }

        // Helper Methods
        // ------------
        private IIncludableQueryable<EventEntity, ICollection<EventRewardEntity>> GetEventCardQuery()
        {
            // Query to connect all of the EventCard (inclusive of all of the child tables).  Doing this to reduce code duplication.
            // Learned from https://stackoverflow.com/questions/30072360/include-several-references-on-the-second-level

            return _context.Events
            .Include(e => e.Options).ThenInclude(eo => eo.Results).ThenInclude(r => r.Requirements)
            .Include(e => e.Options).ThenInclude(eo => eo.Results).ThenInclude(r => r.Rewards);
        }
        private MapperConfiguration GetEventCardMapping()
        {
            // Since we don't want to expose all those datbase keys, we're using AutoMapper to map the original DB model to a DTO exposing to the the client.
            // We need to add all the appropriate mappings to the configuration.
            // Doing this to reduce code duplication.

            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<EventEntity, Event>();
                cfg.CreateMap<EventOptionEntity, EventOption>();
                cfg.CreateMap<EventResultEntity, EventResult>();
                cfg.CreateMap<EventRequirementEntity, EventRequirement>();
                cfg.CreateMap<EventRewardEntity, EventReward>();
            });
        }

        /// <summary>
        /// Retrieves all Events.
        /// </summary>
        /// <remarks></remarks>
        /// <returns>
        /// All Events.
        /// </returns>
        /// <response code = "200">Events returned.</response>
        /// <response code = "404">No Events found.</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> GetEventCards()
        {
            // Get the cards from the database.
            var dbEventCards = GetEventCardQuery();

            // In order to hide the database keys, map them to corresponding DTOs and push them out to a list.
            var mapper = GetEventCardMapping().CreateMapper();
            var eventCards = mapper.Map<List<EventEntity>, List<Event>>(await dbEventCards.ToListAsync());

            // Send them out!
            return eventCards;
        }

        /// <summary>
        /// Retrieves a specific Event based upon passed type and card number.
        /// </summary>
        /// <param name="type">CITY or ROAD</param>
        /// <param name="cardNumber">Card Number</param>
        /// <remarks></remarks>
        /// <returns>
        /// A single Event.
        /// </returns>
        /// <response code = "200">Returns the requested event card.</response>
        /// <response code = "404">Event card wasn't found.</response>
        [HttpGet("{eventType}/{cardNumber}")]
        public async Task<ActionResult<Event>> GetEventCard(string type, string cardNumber)
        {
            // Only accept correct event types in the first node of the route, otherwise return a 404.
            if (type.ToUpper() != "CITY" && type.ToUpper() != "ROAD")
            {
                // Send a 404
                return NotFound();
            }

            // Query the databsase and then select the first card that matches the two incoming parameters.
            var dbEventCard =  await GetEventCardQuery()
                .FirstOrDefaultAsync(i => i.CardNumber == cardNumber && i.Type.ToUpper() == type.ToUpper());

            // If the query above returns no result, send back a 404
            if (dbEventCard == null)
            {
                //Send a 404
                return NotFound();
            }

            // In order to hide the database keys, map them to corresponding DTOs.
            var mapper = GetEventCardMapping().CreateMapper();
            var eventCard = mapper.Map<EventEntity, Event>(dbEventCard);

            // Send the card!
            return eventCard;
        }
    }
}
