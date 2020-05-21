using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Microsoft.Bot.Connector;
using System;
using Microsoft.Bot.Connector.Authentication;
using GloomBot.Models.GHApi;
using GloomBot.Models.GloomhavenDB;
using System.Text.RegularExpressions;
using AdaptiveCards.Templating;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GloomBot.Bots
{
    public class GloomBot : ActivityHandler
    {

        // Assemble the help string
        string helpText = "Herest are the commands you can speak unto me!"
            + "\n* Sayest 'help' to divine mine internal knowledge"
            + "\n* Sayest 'battle goal' to bestow Battle Goals for thy comrades in battle (mention @user)"
            + "\n* Sayest '(city or road) (number)' and I will show you a City or Road Event card"
            + "\n* Sayest '(city or road) (number) option (A or B) and I will give you the result of the requested City event card";

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = $"Huzzah weary traveller! With my oracle like powers, I aim to help thee fight the forces of Gloom!\n{helpText}";
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

            // Branch based on Card Post Back or User Message
            if (string.IsNullOrWhiteSpace(turnContext.Activity.Text) && turnContext.Activity.Value != null)
            { // This is a card response

                // Get the values in a "parsable" object.
                JToken token = JToken.Parse(turnContext.Activity.Value.ToString());


                // Branch based on the value set coming back.
                switch (token["cardResponse"].ToString())
                {
                    case "EventOption":
                        string eventType = token["eventType"].ToString();
                        string cardNumber = token["cardNumber"].ToString();
                        string option = token["option"].ToString();

                        Activity newActivity = (Activity)(await GiveEventOptionAsync(eventType, cardNumber, option));
                        
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
                }
            }
            else
            { // This is a user chat.

                // Prep the message for parsing below.
                string messageText = turnContext.Activity.Text.ToLower();

                if (messageText.Contains("help"))
                { // User is asking for help.
                    await turnContext.SendActivityAsync(GiveHelp(), cancellationToken);
                }
                else if (messageText.Contains("battlegoal") || messageText.Contains("battle goal"))
                { // User is asking for Battle Goals.
                    await turnContext.SendActivityAsync(await GiveBattleGoalsAsync(turnContext, cancellationToken));
                }
                else if (messageText.Contains("city") || messageText.Contains("road"))
                { // User is asking for an event card
                    await turnContext.SendActivityAsync(await GiveEventAsync(messageText), cancellationToken);
                }
                else
                // Everything else.
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text("I have confusion and cannot help thee with that!  Ask for mine 'help' and I will explain what I can do for thee!"), cancellationToken);
                }
            }
        }
        private Activity GiveHelp()
        {
            return MessageFactory.Text($"By Merlin's Beard!\n{helpText}");

        }
        private async Task<Activity> GiveBattleGoalsAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            // Need to know who is going to get the battle goals.  Assuming it's the sender and
            // anyone else that is mentioned.
            string connectedNames = "";
            List<ChannelAccount> connectedChannelAccounts = new List<ChannelAccount>();

            // Add the sender
            connectedNames += turnContext.Activity.From.Name;
            connectedChannelAccounts.Add(turnContext.Activity.From);

            // Add those mentioned (except this bot)
            foreach (Mention m in turnContext.Activity.GetMentions())
            {
                if (m.Mentioned.Id != turnContext.Activity.Recipient.Id)
                {
                    connectedChannelAccounts.Add(m.Mentioned);
                    connectedNames += $", {m.Mentioned.Name}";
                }
            }

            // Get a list of shuffled battle goals.
            List<BattleGoal> battleGoals = await GHApi.GetBattleGoals();
            battleGoals = Utility.ShuffleList(battleGoals);

            if (turnContext.Activity.ChannelId == "msteams")
            {

                // Send a message in the collected channel accounts.
                int j = 0;
                foreach (ChannelAccount c in connectedChannelAccounts)
                {
                    SendIndividualBattleGoalsAsync(c.Id, turnContext, cancellationToken, new BattleGoal[] { battleGoals[j], battleGoals[j + 1] });
                    j += 2;
                }

                // Return the initial message.
                return MessageFactory.Text($"⚔️ The time is nigh to fight the forces of Gloom!  I have bestowed unto thee two goals for this battle. These warriors shall recieve these visions directly: {connectedNames} ⚔️");
            }
            else
            { // Reply back directly with two battle goals. (duplicated from SendIndividualBattleGoalsAsync)
                /////
                // Get the card (and serialize it)
                string battleGoalData = JsonConvert.SerializeObject(new BattleGoal[] { battleGoals[0], battleGoals[1] });

                // Get the template
                string eventTemplate = System.IO.File.ReadAllText(@"Templates\BattleGoal.json");

                // Create the AdaptiveCard using an AdaptiveTransformer
                AdaptiveTransformer transformer = new AdaptiveTransformer();
                string eventCard = transformer.Transform(eventTemplate, battleGoalData);
                Attachment adaptiveCard = new Attachment()
                {
                    ContentType = "application/vnd.microsoft.card.adaptive",
                    Content = JsonConvert.DeserializeObject(eventCard)
                };

                return (Activity)MessageFactory.Attachment(adaptiveCard);
                ///////
                ///return MessageFactory.Text($"No goals have been sent - this only works in MS Teams. (for the moment)");
            }
        }
        private async void SendIndividualBattleGoalsAsync(string userId, ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken, BattleGoal[] battleGoals)
        {
            // Create a separate connector to send private messages
            MicrosoftAppCredentials creds = new MicrosoftAppCredentials(Startup.BotAppId, Startup.BotAppSecret);
            var connector = new ConnectorClient(new Uri(turnContext.Activity.ServiceUrl), creds);

            // Create a channel account
            ChannelAccount channelAccount = new ChannelAccount(userId);

            // Create conversation parameter
            ConversationParameters conversationParameters = new ConversationParameters()
            {
                ChannelData = new TeamsChannelData() { Tenant = new TenantInfo(turnContext.Activity.Conversation.TenantId) },
                Members = new List<ChannelAccount>() { channelAccount }
            };

            // Create/get the conversation
            var response = await connector.Conversations.CreateConversationAsync(conversationParameters);

            // Get the card (and serialize it)
            string battleGoalData = JsonConvert.SerializeObject(battleGoals);

            // Get the template
            string battleGoalTemplate = System.IO.File.ReadAllText(@"Templates\BattleGoal.json");

            // Create the AdaptiveCard using an AdaptiveTransformer
            AdaptiveTransformer transformer = new AdaptiveTransformer();
            string battleGoalCard = transformer.Transform(battleGoalTemplate, battleGoalData);
            Attachment adaptiveCard = new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(battleGoalCard)
            };

            await connector.Conversations.SendToConversationAsync(response.Id, (Activity)MessageFactory.Attachment(adaptiveCard), cancellationToken);
        }
        private async Task<Activity> GiveEventAsync(string message)
        {
            // Get the type
            string cardType = message.Contains("city") ? cardType = "city" : cardType = "road";

            // Find the number
            string cardNumber = Regex.Match(message, @"\d+").Value;

            if (string.IsNullOrWhiteSpace(cardNumber))
            {
                return MessageFactory.Text($"Looking for a {cardType} event, eh? I need to know the number!");
            }
            else
            {
                // Get the card (and serialize it)
                string eventData = JsonConvert.SerializeObject(await GloomhavenDB.GetEvent(cardType, cardNumber));

                // Get the templat
                string eventTemplate = System.IO.File.ReadAllText(@"Templates\Event.json");

                // Create the AdaptiveCard using an AdaptiveTransformer
                AdaptiveTransformer transformer = new AdaptiveTransformer();
                string eventCard = transformer.Transform(eventTemplate, eventData);
                Attachment adaptiveCard = new Attachment()
                {
                    ContentType = "application/vnd.microsoft.card.adaptive",
                    Content = JsonConvert.DeserializeObject(eventCard)
                };

                return (Activity)MessageFactory.Attachment(adaptiveCard);
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

            // Get the card (and serialize it)
            string eventData = JsonConvert.SerializeObject(e);

            // Get the templat
            string eventTemplate = System.IO.File.ReadAllText(@"Templates\EventResult.json");

            // Create the AdaptiveCard using an AdaptiveTransformer
            AdaptiveTransformer transformer = new AdaptiveTransformer();
            string eventCard = transformer.Transform(eventTemplate, eventData);
            Attachment adaptiveCard = new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(eventCard)
            };

            return (Activity)MessageFactory.Attachment(adaptiveCard);
        }
    }
}
