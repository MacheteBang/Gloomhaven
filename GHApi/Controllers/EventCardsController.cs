using System;
using AutoMapper;
using AutoMapper.Collection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GHApi.Models;

namespace GHApi.Controllers
{
    [Route("gh/[controller]")]
    [ApiController]
    public class EventCardsController : ControllerBase
    {
        private readonly GHContext db;

        public EventCardsController(GHContext context)
        {
            db = context;
        }

        // GET: api/eventcards
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventCardDTO>>> GetEventCards()
        {
            // Below code connects the main object to it's children (and grandchildren, etc)
            // https://stackoverflow.com/questions/30072360/include-several-references-on-the-second-level
            var dbEventCards = db.EventCards
            .Include(e => e.EventCardOptions).ThenInclude(eo => eo.EventCardResults).ThenInclude(r => r.EventCardRequirement)
            .Include(e => e.EventCardOptions).ThenInclude(eo => eo.EventCardResults).ThenInclude(r => r.EventCardReward);

            // Since we don't want to expose all those datbase keys, we're using AutoMapper to map the original DB model to a DTO exposing to the the client.
            // We need to add all the appropriate mappings to the configuration.
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<EventCard, EventCardDTO>();
                cfg.CreateMap<EventCardOption, EventCardOptionDTO>();
                cfg.CreateMap<EventCardResult, EventCardResultDTO>();
                cfg.CreateMap<EventCardRequirement, EventCardRequirementDTO>();
                cfg.CreateMap<EventCardReward, EventCardRewardDTO>();
            });

            // Create a mapper and and map the selected cards to their associated DTOs.
            var mapper = mapperConfiguration.CreateMapper();
            var eventCards = mapper.Map<List<EventCard>, List<EventCardDTO>>(await dbEventCards.ToListAsync());

            return eventCards;
        }

        // GET: api/eventcards/city/5
        [HttpGet("{eventType}/{cardNumber}")]
        public async Task<ActionResult<EventCardDTO>> GetEventCard(string eventType, string cardNumber)
        {
            if (eventType.ToUpper() != "CITY" && eventType.ToUpper() != "ROAD")
            {
                return NotFound();
            }

            // See base call (above)for explaination
            // https://stackoverflow.com/questions/30072360/include-several-references-on-the-second-level
            var dbEventCard =  await db.EventCards
                .Include(e => e.EventCardOptions).ThenInclude(eo => eo.EventCardResults).ThenInclude(r => r.EventCardRequirement)
                .Include(e => e.EventCardOptions).ThenInclude(eo => eo.EventCardResults).ThenInclude(r => r.EventCardReward)
                .FirstOrDefaultAsync(i => i.CardNumber == cardNumber && i.EventType.ToUpper() == eventType.ToUpper());

            // See base call (above)for explaination
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<EventCard, EventCardDTO>();
                cfg.CreateMap<EventCardOption, EventCardOptionDTO>();
                cfg.CreateMap<EventCardResult, EventCardResultDTO>();
                cfg.CreateMap<EventCardRequirement, EventCardRequirementDTO>();
                cfg.CreateMap<EventCardReward, EventCardRewardDTO>();
            });

            // Create a mapper and and map the selected cards to their associated DTOs.
            var mapper = mapperConfiguration.CreateMapper();
            var eventCard = mapper.Map<EventCard, EventCardDTO>(dbEventCard);

            //var eventCard = await db.EventCards.FindAsync(id);

            if (eventCard == null)
            {
                return NotFound();
            }

            return eventCard;
        }

        //// PUT: api/EventCards/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for
        //// more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutEventCard(long id, EventCard eventCard)
        //{
        //    if (id != eventCard.ID_EventCards)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(eventCard).State = EntityState.Modified;

        //    try
        //    {
        //        await db.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!EventCardExists(id))
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

        //// POST: api/EventCards
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for
        //// more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        //[HttpPost]
        //public async Task<ActionResult<EventCard>> PostEventCard(EventCard eventCard)
        //{
        //    db.EventCards.Add(eventCard);
        //    await db.SaveChangesAsync();

        //    return CreatedAtAction("GetEventCard", new { id = eventCard.ID_EventCards }, eventCard);
        //}

        //// DELETE: api/EventCards/5
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<EventCard>> DeleteEventCard(long id)
        //{
        //    var eventCard = await db.EventCards.FindAsync(id);
        //    if (eventCard == null)
        //    {
        //        return NotFound();
        //    }

        //    db.EventCards.Remove(eventCard);
        //    await db.SaveChangesAsync();

        //    return eventCard;
        //}

        private bool EventCardExists(long id)
        {
            return db.EventCards.Any(e => e.EventCardId == id);
        }
    }
}
