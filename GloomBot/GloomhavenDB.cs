using GloomBot.Models.GloomhavenDB;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace GloomBot
{
    public class GloomhavenDB
    {
        public async static Task<EventCard> GetEventCard(string type, string number)
        {
            if (type.ToLower() != "city" && type.ToLower() != "road")
            {
                return null;
            }
            else
            {
                EventCard eventCard;

                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync($"{Startup.GloomHavenDBUrl_Events}/{type}/{number}"))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        eventCard = JsonConvert.DeserializeObject<EventCard>(apiResponse);
                    }
                }
                return eventCard;
            }
        }

        public async static Task<List<EventCard>> GetEventCards(string type)
        {
            if (type.ToLower() != "city" && type.ToLower() != "road")
            {
                return null;
            }
            else
            {
                List<EventCard> eventCards = new List<EventCard>();

                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync($"{Startup.GloomHavenDBUrl_Events}/{type}"))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        eventCards = JsonConvert.DeserializeObject<List<EventCard>>(apiResponse);
                    }
                }
                return eventCards;
            }
        }
    }
}
