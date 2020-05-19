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
        public async static Task<Event> GetEvent(string type, string number)
        {
            if (type.ToLower() != "city" && type.ToLower() != "road")
            {
                return null;
            }
            else
            {
                Event eventCard;

                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync($"{Startup.GloomHavenDBUrl_Events}/{type}/{number}"))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        eventCard = JsonConvert.DeserializeObject<Event>(apiResponse);
                    }
                }
                return eventCard;
            }
        }

        public async static Task<List<Event>> GetEvents(string type)
        {
            if (type.ToLower() != "city" && type.ToLower() != "road")
            {
                return null;
            }
            else
            {
                List<Event> eventCards = new List<Event>();

                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync($"{Startup.GloomHavenDBUrl_Events}/{type}"))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        eventCards = JsonConvert.DeserializeObject<List<Event>>(apiResponse);
                    }
                }
                return eventCards;
            }
        }

        public async static Task<Item> GetItem(string number)
        {
            Item item;

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"{Startup.GloomHavenDBUrl_Items}/{number}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        item = JsonConvert.DeserializeObject<Item>(apiResponse);
                    }
                    else
                    {
                        item = null;
                    }
                    
                }
            }
            return item;
        }
    }
}
