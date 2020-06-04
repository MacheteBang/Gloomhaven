using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Microsoft.Bot.Connector;
using System;
using GloomBot.Models.GHApi;
using GloomBot.Models.GloomhavenDB;
using System.Text.RegularExpressions;
using AdaptiveCards.Templating;
using AdaptiveCards;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net.Http;

namespace GloomBot.Bots
{
    public class GloomBot : ActivityHandler
    {
        //-------- Messaging activity --------//
        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = $"Huzzah weary traveller! With my oracle like powers, I aim to help thee fight the forces of Gloom!  If you need help, simply say 'help'!";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    Activity response;
                    response = MessageFactory.Text(welcomeText, welcomeText);
                    await turnContext.SendActivityAsync(response, cancellationToken);
                }
            }
        }
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            // Send a typing indicator to let the user know it's working
            Activity[] a = { new Activity("typing"), new Activity() {Type="delay", Value=500 } };
            await turnContext.SendActivitiesAsync(a);

            // From the incoming message, get a cleaned dictionary of what the processes below may need.
            Dictionary<string, object> cleanedMessage = GetCleanedInput((Activity)turnContext.Activity);

            // Switch based on what the user is asking for
            switch (cleanedMessage["type"])
            {
                case "help":
                    await turnContext.SendActivityAsync(GiveHelp(), cancellationToken);
                    break;
                case "event":
                    await turnContext.SendActivityAsync(await GiveEventAsync(cleanedMessage), cancellationToken);
                    break;
                case "eventOption":
                    string eventType = cleanedMessage["eventType"].ToString();
                    string cardNumber = cleanedMessage["cardNumber"].ToString();
                    string eventOption = cleanedMessage["eventOption"].ToString();

                    Activity newActivity = await GiveEventOptionAsync(eventType, cardNumber, eventOption);

                    if (turnContext.Activity.ReplyToId != null)
                    {
                        newActivity.Id = turnContext.Activity.ReplyToId;
                        await turnContext.UpdateActivityAsync(newActivity, cancellationToken);
                    }
                    else
                    {
                        await turnContext.SendActivityAsync(newActivity, cancellationToken);
                    }
                    break;
                case "battleGoalRequest":
                    await turnContext.SendActivityAsync(GetBattleGoalRequest(cleanedMessage), cancellationToken);
                    break;
                case "battleGoal":
                    await turnContext.SendActivityAsync(await GiveBattleGoalsAsync(cleanedMessage), cancellationToken);
                    break;
                case "item":
                    await turnContext.SendActivityAsync(await GiveItemAsync(cleanedMessage), cancellationToken);
                    break;
                default:
                    await turnContext.SendActivityAsync(MessageFactory.Text("I have confusion and cannot help thee with that!  Ask for mine 'help' and I will explain what I can do for thee!"), cancellationToken);
                    break;
            }
        }

        //-------- Helper Functions --------//
        private Dictionary<string, object> GetCleanedInput(Activity activity)
        /// <summary>
        /// Evaluates the incoming activity and pulls all relavent information from it and stores it
        /// in a dictionary for standard consumption.  Does the work of parsing text or a call from 
        /// a previously sent card so that the consumer doesn't need to do that work.
        /// </summary>
        /// <param name="activity"></param>
        /// <returns>
        /// Dictionary<string, object> of all the data that a particular activity needs.  If we 'hit'
        /// a valid request, the dictionary should always contain a key of "type" which will be
        /// consumed elsewhere.
        /// </returns>
        {
            // This is what we're manipulating and sending out at the end.
            Dictionary<string, object> activityDictionary = new Dictionary<string, object>();

            // Add keys containing high level information.
            activityDictionary.Add("service", new Uri(activity.ServiceUrl));
            activityDictionary.Add("tenant", new TenantInfo(activity.Conversation.TenantId));
            activityDictionary.Add("channelId", activity.ChannelId);

            // Convert the activity value to a string
            string activityValue = activity.Value == null ? "" : activity.Value.ToString();

            // Branch based on whether the activity is from a card (i.e. on the value property) or from
            // the user requesting something (i.e. on the text property).
            if (string.IsNullOrWhiteSpace(activity.Text) && activityValue != null)
            // From a card
            {
                // Convert the value to a dictionary-like token.  Assumption is that it's coming
                // in as JSON.
                JToken token = JToken.Parse(activityValue);
                
                // Create key/value pairs based upon the "cardResponse" key in the value.
                switch (token["cardResponse"].ToString())
                {
                    case "event":
                        // This is a request for an event card.
                        activityDictionary.Add("type", "event");
                        activityDictionary.Add("eventType", token["eventType"]);

                        // If we have an event number, add it.  Two 'if' statements are required.
                        if (token["eventNumber"] != null)
                            if (!string.IsNullOrEmpty(token["eventNumber"].ToString()))
                                activityDictionary.Add("eventNumber", token["eventNumber"]);

                        break;
                    case "eventOption":
                        // This is a request for A/B of an event.
                        activityDictionary.Add("type", "eventOption");
                        activityDictionary.Add("eventType", token["eventType"]);
                        activityDictionary.Add("cardNumber", token["cardNumber"]);
                        activityDictionary.Add("eventOption", token["option"]);
                        break;
                    case "battleGoalRequest":
                        // This is a request to get the battle goals for the team.
                        activityDictionary.Add("type", "battleGoalRequest");

                        // If the channel is MS Teams, get a list of the members of the group / channel.
                        if (activityDictionary["channelId"].ToString() == "msteams")
                            activityDictionary.Add("battleGoalChoices", GetConversationMembers((Uri)activityDictionary["service"], activity.Conversation.Id));
                        break;
                    case "battleGoal":
                        // This is a request to send battle goals to a selected list of people.
                        activityDictionary.Add("type", "battleGoal");

                        // Create a list of accounts that were chosen.
                        List<ChannelAccount> mentionedAccounts = new List<ChannelAccount>();

                        // Add each account ID that was chosen.
                        foreach (string s in token["playerChoiceSet"].ToString().Split(","))
                            mentionedAccounts.Add(new ChannelAccount(s));

                        // Add it to the dictionary
                        activityDictionary.Add("mentionedAccounts", mentionedAccounts);
                        break;
                    case "item":
                        activityDictionary.Add("type", "item");

                        // If we have an event number, add it.  Two 'if' statements are required.
                        if (token["itemNumber"] != null)
                            if (!string.IsNullOrEmpty(token["itemNumber"].ToString()))
                                activityDictionary.Add("itemNumber", token["itemNumber"].ToString());

                        break;
                }
            }
            else
            // From the user saying something.  Simply looking for specific keywords in what they say.
            {
                if (activity.Text.Contains("help"))
                    // user is asking for help
                    activityDictionary.Add("type", "help");
                else if (activity.Text.Contains("battlegoal") || activity.Text.Contains("battle goal")) // User is asking for Battle Goals.
                // user is asking for battle goals.
                {
                    // Determine if the user mentioned anyone (aside from Gloombot)
                    if (activity.GetMentions().Length == 1 && activity.GetMentions()[0].Mentioned.Id == activity.Recipient.Id)
                    // they didn't mention anyone else, so send them the card that shows them who is in the channel.
                    {
                        activityDictionary.Add("type", "battleGoalRequest");
                        activityDictionary.Add("battleGoalChoices", GetConversationMembers((Uri)activityDictionary["service"], activity.Conversation.Id));
                    }
                    else
                    // they mentioned people, so send them down the path of sending battle goals to those that were mentioned
                    // as well as the person initiating the chat.
                    {
                        activityDictionary.Add("type", "battleGoal");

                        // Create a list of accounts that were mentioned.
                        List<ChannelAccount> mentionedAccounts = new List<ChannelAccount>();

                        // Add each mentioned account to the outgoign list.
                        foreach (Mention m in activity.GetMentions())
                            if (m.Mentioned.Id != activity.Recipient.Id)
                                mentionedAccounts.Add(m.Mentioned);

                        // Add the person that triggered the chat as well.
                        mentionedAccounts.Add(activity.From);

                        // Add it to the dictionary
                        activityDictionary.Add("mentionedAccounts", mentionedAccounts);
                    }
                }
                else if (activity.Text.Contains("city"))
                {
                    activityDictionary.Add("type", "event");
                    activityDictionary.Add("eventType", "city");

                    if (!String.IsNullOrEmpty(Regex.Match(activity.Text, @"\d+").Value))
                        activityDictionary.Add("cardNumber", Regex.Match(activity.Text, @"\d+").Value);
                }
                else if (activity.Text.Contains("road"))
                {
                    activityDictionary.Add("type", "event");
                    activityDictionary.Add("eventType", "road");

                    if (!String.IsNullOrEmpty(Regex.Match(activity.Text, @"\d+").Value))
                        activityDictionary.Add("cardNumber", Regex.Match(activity.Text, @"\d+").Value);
                }
                else if (activity.Text.Contains("item"))
                {
                    activityDictionary.Add("type", "item");

                    if (!String.IsNullOrEmpty(Regex.Match(activity.Text, @"\d+").Value))
                        activityDictionary.Add("itemNumber", Regex.Match(activity.Text, @"\d+").Value);
                }
            }
            return activityDictionary;
        }
        private Activity GiveHelp()
        {
            return (Activity)MessageFactory.Attachment(GetAdaptiveCard("{}", @"Templates\Help.json"));
        }
        private Activity GetBattleGoalRequest(Dictionary<string, object> messageData)
        {
            // Get the template
            string battleGoalRequestTemplate = File.ReadAllText(@"Templates\BattleGoalRequest.json");

            // String replace the choice set "variable" with an actual list of choices.
            string choiceSetJson = JsonConvert.SerializeObject(messageData["battleGoalChoices"]);
            battleGoalRequestTemplate = battleGoalRequestTemplate.Replace("${battleGoalChoices}", choiceSetJson);

            AdaptiveCard battleGoalRequest = AdaptiveCard.FromJson(battleGoalRequestTemplate).Card;

            Attachment result = new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(battleGoalRequest.ToJson())
            };

            return (Activity)MessageFactory.Attachment(result);
        }
        private async Task<Activity> GiveBattleGoalsAsync(Dictionary<string, object> messageData)
        {
            // Get a list of shuffled goals and shuffle them.
            List<BattleGoal> battleGoals = await GHApi.GetBattleGoals();
            battleGoals = Utility.ShuffleList(battleGoals);

            if (messageData["channelId"].ToString() == "msteams")
            {
                // Create a connection to the tenant.
                var botConnectorClient = new ConnectorClient((Uri)messageData["service"], Startup.BotCredentials);

                // Loop through the accounts that were mentioned, and send the message.
                int j = 0; // also loops through the battle goals.
                foreach (ChannelAccount c in (List<ChannelAccount>)messageData["mentionedAccounts"])
                {
                    // Create conversation parameter
                    ConversationParameters conversationParameters = new ConversationParameters()
                    {
                        ChannelData = new TeamsChannelData() { Tenant = (TenantInfo)messageData["tenant"] },
                        Members = new List<ChannelAccount>() { c }
                    };

                    // Create/get the conversation
                    var response = await botConnectorClient.Conversations.CreateConversationAsync(conversationParameters);

                    // Send the individual battle goal
                    await botConnectorClient.Conversations.SendToConversationAsync(response.Id, (Activity)MessageFactory.Attachment(GetAdaptiveCard(new BattleGoal[] { battleGoals[j], battleGoals[j + 1] }, @"Templates\BattleGoal.json")));

                    // increment the counter for the battle goals.
                    j += 2;
                }

                // Return the initial message.
                return MessageFactory.Text($"⚔️ I have sent battle goals to those who need them! ⚔️");
            }
            else
            { // Reply back directly with two battle goals. (duplicated from SendIndividualBattleGoalsAsync)
                return (Activity)MessageFactory.Attachment(GetAdaptiveCard(new BattleGoal[] { battleGoals[0], battleGoals[1] }, @"Templates\BattleGoal.json"));
            }
        }
        private async Task<Activity> GiveEventAsync(Dictionary<string, object> messageData)
        {
            string eventType = messageData["eventType"].ToString();

            if (!messageData.ContainsKey("eventNumber"))
                return (Activity)MessageFactory.Attachment(GetAdaptiveCard($"{{\"eventType\":\"{eventType}\"}}", @"Templates\EventRequest.json"));
            else
            {
                string eventNumber = messageData["eventNumber"].ToString();
                return (Activity)MessageFactory.Attachment(GetAdaptiveCard(await GloomhavenDB.GetEvent(eventType, eventNumber), @"Templates\Event.json"));
            }
        }
        private async Task<Activity> GiveEventOptionAsync(string eventType, string cardNumber, string option)
        {
            // Get the event data
            Event e = await GloomhavenDB.GetEvent(eventType, cardNumber);

            // For simplicity, the template is just going to use the same object and ALWAYS
            // look at the "A" properties.
            if (option == "B")
                e.OptionA = e.OptionB;

            return (Activity)MessageFactory.Attachment(GetAdaptiveCard(e, @"Templates\EventResult.json"));
        }
        private async Task<Activity> GiveItemAsync(Dictionary<string, object> messageData)
        {
            // Find the number
            if (!messageData.ContainsKey("itemNumber"))
                return (Activity)MessageFactory.Attachment(GetAdaptiveCard(@"", @"Templates\ItemRequest.json"));
            else
                return (Activity)MessageFactory.Attachment(GetAdaptiveCard(await GloomhavenDB.GetItem((string)messageData["itemNumber"]), @"Templates\Item.json"));
        }
        private Attachment GetAdaptiveCard(object data, string templateLocation)
        {

            string dataJson;
            // Get the JSON data for the item.  If it's already a string, assume it's JSON straightaway
            if (data.GetType().ToString() != "System.String")
                dataJson = JsonConvert.SerializeObject(data);
            else
                dataJson = data.ToString();

            // Get the template
            string templateJson = File.ReadAllText(templateLocation);

            // Create the AdaptiveCard using an AdaptiveTransformer
            AdaptiveCardTemplate template = new AdaptiveCardTemplate(templateJson);
            string cardJson = template.Expand(dataJson);
            Attachment adaptiveCard = new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(cardJson)
            };

            return adaptiveCard;
        }
        private List<ChoiceItem> GetConversationMembers(Uri service, string conversationId)
        {
            // Create a connection to the tenant.
            var botConnectorClient = new ConnectorClient(service, Startup.BotCredentials);
            IList<ChannelAccount> members = botConnectorClient.Conversations.GetConversationMembersAsync(conversationId).Result;

            // Get a list of all members that are part of the conversation (assumption is MS teams)
            List<ChoiceItem> possibleMembers = new List<ChoiceItem>();

            // Add those members to the list to be displayed.
            foreach (ChannelAccount c in members)
                possibleMembers.Add(new ChoiceItem() { Title = c.Name, Value = c.Id });

            possibleMembers.Sort();

            return possibleMembers;
        }
    }
}
