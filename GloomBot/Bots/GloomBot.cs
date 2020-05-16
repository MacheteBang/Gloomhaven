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
using GloomBot.Models.Bot;
using GloomBot.Models.GloomhavenDB;
using System.Text.RegularExpressions;

namespace GloomBot.Bots
{
    public class GloomBot : ActivityHandler
    {
        private BotDBContext db;

        public GloomBot (BotDBContext context)
        {
            db = context;
        }

        // Assemble the help string
        string helpText = "Herest are the commands you can speak unto me!"
            + "\n* Sayest 'help' to divine mine internal knowledge"
            + "\n* Sayest 'battle goal' to bestow Battle Goals for thy comrades in battle (mention @user)"
            + "\n* Sayest '(city or road) (number)' and I will show you a City or Road Event card"
            + "\n* Sayest '(city or road) (number) option (A or B) and I will give you the result of the requested City event card";

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {

            //  Add a DB writer upon every message recieved.
            db.LogMessage((Activity)turnContext.Activity);

            // Declare the response activity.
            Activity responseActivity;

            // Figure out what the user wants ans assemple a message.
            string messageText = turnContext.Activity.Text.ToLower();

            
            if (messageText.Contains("help"))
            // Look for help
            {
                responseActivity = MessageFactory.Text($"By Merlin's Beard!\n{helpText}");
            }
            else if (messageText.Contains("city") || messageText.Contains("road"))
            {
                // Get the type
                string cardType = messageText.Contains("city") ? cardType = "city" : cardType = "road";

                // Find the number
                string cardNumber = Regex.Match(messageText, @"\d+").Value;

                if (cardNumber == "")
                {
                    responseActivity = MessageFactory.Text($"Looking for a {cardType} event, eh? I need to know the number!");
                } else
                {

                    EventCard eventCard = await GloomhavenDB.GetEventCard(cardType, cardNumber);

                    if (messageText.Contains("option"))
                    {
                        if (messageText.Contains(" a"))
                        {
                            Attachment eventCardOptionImage = new Attachment
                            {
                                Name = $"{eventCard.Type} Card {eventCard.Number} - Option A",
                                ContentType = "image/png",
                                ContentUrl = $"https://gloomhavendb.com{eventCard.OptionA.ImageUrl}"
                            };

                            responseActivity = (Activity)MessageFactory.Attachment(eventCardOptionImage);
                        } else if (messageText.Contains(" b"))
                        {
                            Attachment eventCardOptionImage = new Attachment
                            {
                                Name = $"{eventCard.Type} Card {eventCard.Number} - Option B",
                                ContentType = "image/png",
                                ContentUrl = $"https://gloomhavendb.com{eventCard.OptionB.ImageUrl}"
                            };

                            responseActivity = (Activity)MessageFactory.Attachment(eventCardOptionImage);
                        } else
                        {
                            responseActivity = MessageFactory.Text("You're asking for an result, but you didn't tell me which option.");
                        }
                    } else
                    {
                        Attachment eventCardImage = new Attachment
                        {
                            Name = $"{eventCard.Type} Card {eventCard.Number}",
                            ContentType = "image/png",
                            ContentUrl = $"https://gloomhavendb.com{eventCard.ImageUrl}"
                        };
                        responseActivity = (Activity)MessageFactory.Attachment(eventCardImage);
                    }
                }

            }
            else if (messageText.Contains("battlegoal") || messageText.Contains("battle goal"))
            // Look for battle goals
            {

                // Now assemble and send them privately to people that were mentioned.
                // Get a list of people connected to the messages.
                String connectedNames = "";
                connectedNames += turnContext.Activity.From.Name;

                List<ChannelAccount> connectedChannelAccounts = new List<ChannelAccount>();
                connectedChannelAccounts.Add(turnContext.Activity.From);

                foreach (Mention m in turnContext.Activity.GetMentions())
                {
                    if (m.Mentioned.Id != turnContext.Activity.Recipient.Id)
                    {
                        connectedChannelAccounts.Add(m.Mentioned);
                        connectedNames += $", {m.Mentioned.Name}";
                    }
                }

                // Response to query.
                responseActivity = (Activity)MessageFactory.Text($"⚔️ The time is nigh to fight the forces of Gloom!  I shall bestow unto thee two goals for this battle. These warriors shall recieve these visions directly: {connectedNames} ⚔️");

                // Get the activity of the incoming request.
                IMessageActivity activity = turnContext.Activity;

                // Get the tenant
                string tenantID = activity.Conversation.TenantId;
                TenantInfo tenantInfo = new TenantInfo(tenantID);

                // Create a new (private?) channel
                TeamsChannelData channelData = new TeamsChannelData();
                channelData.Tenant = tenantInfo;

                List<BattleGoal> battleGoals = await GHApi.GetBattleGoals();
                battleGoals = Utility.ShuffleList(battleGoals);

                // Send them all a message
                int j = 0;
                foreach (ChannelAccount c in connectedChannelAccounts)
                {
                    SendBattleGoalsMessage(c.Id, channelData, activity.ServiceUrl, cancellationToken, battleGoals[j], battleGoals[j+1]);
                    j += 2;
                }
            }
            else
            // Everything else.
            {
                responseActivity = MessageFactory.Text("I have confusion and cannot help thee with that!  Ask for mine 'help' and I will explain what I can do for thee!");
            }

            // Send the response to this message.
            await turnContext.SendActivityAsync(responseActivity, cancellationToken);
            db.LogMessage(responseActivity);
        }

        private async void SendBattleGoalsMessage(string userId, TeamsChannelData channelData, string serviceUrl, CancellationToken cancellationToken, BattleGoal battleGoal1, BattleGoal battleGoal2)
        {
            // Create a separate connector to send private messages
            MicrosoftAppCredentials creds = new MicrosoftAppCredentials(Startup.BotAppId, Startup.BotAppSecret);
            var connector = new ConnectorClient(new Uri(serviceUrl), creds);

            // Create a channel account
            ChannelAccount channelAccount = new ChannelAccount(userId);

            // Create conversation parameter
            ConversationParameters conversationParameters = new ConversationParameters();
            conversationParameters.ChannelData = channelData;
            conversationParameters.Members = new List<ChannelAccount>() { channelAccount };

            // Create/get the conversation
            var response = await connector.Conversations.CreateConversationAsync(conversationParameters);


            // Send that person a private message
            List<CardImage> cardImage = new List<CardImage> { new CardImage("https://mmmpizzastorage.blob.core.windows.net/mmmpizzablob/battlegoal_back.jpg") };
            ThumbnailCard bg1 = new ThumbnailCard(battleGoal1.GoalName, "Reward: ".PadRight(8 + battleGoal1.Reward, '✓'), $"{battleGoal1.GoalDescription}", cardImage);
            ThumbnailCard bg2 = new ThumbnailCard(battleGoal2.GoalName, "Reward: ".PadRight(8 + battleGoal2.Reward, '✓'), $"{battleGoal2.GoalDescription}", cardImage);


            Activity a = (Activity)MessageFactory.Attachment(bg1.ToAttachment());
            a.Attachments.Add(bg2.ToAttachment());
            a.AttachmentLayout = "list";
            a.Text = "From the darkness, I see two goals for you.  Choose one, and if you complete it, you will grow stronger!";

            await connector.Conversations.SendToConversationAsync(response.Id, a, cancellationToken);
            db.LogMessage(a);
        }

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
                    db.LogMessage(response);
                    
                }
            }
        }
    }


}
