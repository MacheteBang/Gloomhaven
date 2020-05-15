using GloomBot.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace GloomBot
{
    public static class GHApi
    {
        public async static Task<List<BattleGoal>> GetBattleGoals()
        {
            List<BattleGoal> battleGoals = new List<BattleGoal>();

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(Startup.ApiUrl_BattleGoals))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    battleGoals = JsonConvert.DeserializeObject<List<BattleGoal>>(apiResponse);
                }
            }
            return battleGoals;
        }
    }
}
