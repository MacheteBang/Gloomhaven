using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GHApi.Models;

namespace GHApi.Controllers
{
    [Produces("application/json")]
    [Route("gh/[controller]")]
    [ApiController]
    public class EventCardsController : ControllerBase
    {
        // Private Variables
        // -----------------
        private readonly GHContext db;

        // Constructors
        // ------------
        public EventCardsController(GHContext context)
        {
            db = context;
        }

        // Helper Methods
        // ------------
        private Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<EventCard, ICollection<EventCardReward>> GetEventCardQuery()
        {
            // Query to connect all of the EventCard (inclusive of all of the shild tables).  Doing this to reduce code duplication.
            // Learned from https://stackoverflow.com/questions/30072360/include-several-references-on-the-second-level

            return db.EventCards
            .Include(e => e.EventCardOptions).ThenInclude(eo => eo.EventCardResults).ThenInclude(r => r.EventCardRequirement)
            .Include(e => e.EventCardOptions).ThenInclude(eo => eo.EventCardResults).ThenInclude(r => r.EventCardReward);
        }
        private MapperConfiguration GetEventCardMapping()
        {
            // Since we don't want to expose all those datbase keys, we're using AutoMapper to map the original DB model to a DTO exposing to the the client.
            // We need to add all the appropriate mappings to the configuration.
            // Doing this to reduce code duplication.

            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<EventCard, EventCardDTO>();
                cfg.CreateMap<EventCardOption, EventCardOptionDTO>();
                cfg.CreateMap<EventCardResult, EventCardResultDTO>();
                cfg.CreateMap<EventCardRequirement, EventCardRequirementDTO>();
                cfg.CreateMap<EventCardReward, EventCardRewardDTO>();
            });
        }

        /// <summary>
        /// Gets a list of event cards.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventCardDTO>>> GetEventCards()
        {
            // Get the cards from the database.
            var dbEventCards = GetEventCardQuery();

            // In order to hide the database keys, map them to corresponding DTOs and push them out to a list.
            var mapper = GetEventCardMapping().CreateMapper();
            var eventCards = mapper.Map<List<EventCard>, List<EventCardDTO>>(await dbEventCards.ToListAsync());

            // Send them out!
            return eventCards;
        }

        /// <summary>
        /// Gets a specific event card based upon it's type and number.
        /// </summary>
        /// <param name="eventType">Can either be City or Road.</param>
        /// <param name="cardNumber">Integer number of the card.</param>
        /// <returns>The requested event card.</returns>
        /// <response code = "200">Returns the requested event card.</response>
        /// <response code = "404">Event card wasn't found.</response>
        [HttpGet("{eventType}/{cardNumber}")]
        public async Task<ActionResult<EventCardDTO>> GetEventCard(string eventType, string cardNumber)
        {
            // Only accept correct event types in the first node of the route, otherwise return a 404.
            if (eventType.ToUpper() != "CITY" && eventType.ToUpper() != "ROAD")
            {
                // Send a 404
                return NotFound();
            }

            // Query the databsase and then select the first card that matches the two incoming parameters.
            var dbEventCard =  await GetEventCardQuery()
                .FirstOrDefaultAsync(i => i.CardNumber == cardNumber && i.EventType.ToUpper() == eventType.ToUpper());

            // If the query above returns no result, send back a 404
            if (dbEventCard == null)
            {
                //Send a 404
                return NotFound();
            }

            // In order to hide the database keys, map them to corresponding DTOs.
            var mapper = GetEventCardMapping().CreateMapper();
            var eventCard = mapper.Map<EventCard, EventCardDTO>(dbEventCard);

            // Send the card!
            return eventCard;
        }
    }
}
