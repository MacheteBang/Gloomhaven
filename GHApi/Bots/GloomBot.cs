// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.6.2

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Microsoft.Bot.Connector;
using System;
using Microsoft.Bot.Connector.Authentication;
using GHApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GHApi.Bots
{
    public class GloomBot : ActivityHandler
    {

        // Assemble the help string
        String helpText = "Herest are the commands you can speak unto me!"
            + "\n* Sayest 'help' to divine mine internal knowledge"
            + "\n* Sayest 'battle goal' to bestow Battle Goals for thy comrades in battle (mention @user)";

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            // Figure out what the user wants
            string messageText = turnContext.Activity.Text.ToLower();

            // Look for the word help
            if (messageText.Contains("help"))
            {
                await turnContext.SendActivityAsync(MessageFactory.Text($"By Merlin's Beard!\n{helpText}"), cancellationToken);
            }
            else if (messageText.Contains("battlegoal") || messageText.Contains("battle goal"))
            {
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

                Activity battleGoalResponse = (Activity)MessageFactory.Text($"⚔️ The time is nigh to fight the forces of Gloom!  I shall bestow unto thee two goals for this battle. These warriors shall recieve these visions directly: {connectedNames} ⚔️");
                await turnContext.SendActivityAsync(battleGoalResponse, cancellationToken);

                // Get the activity of the incoming request.
                IMessageActivity activity = turnContext.Activity;

                // Get the tenant
                string tenantID = activity.Conversation.TenantId;
                TenantInfo tenantInfo = new TenantInfo(tenantID);

                // Create a new (private?) channel
                TeamsChannelData channelData = new TeamsChannelData();
                channelData.Tenant = tenantInfo;

                List<BattleGoal> battleGoals = await GetBattleGoals();
                battleGoals = Utility.ShuffleList<BattleGoal>(battleGoals);

                // Send them all a message
                int j = 0;
                foreach (ChannelAccount c in connectedChannelAccounts)
                {
                    SendBattleGoalsMessage(c.Id, channelData, activity.ServiceUrl, cancellationToken, battleGoals[j], battleGoals[j+1]);
                    j += 2;
                }
                
            } else
            {
                Attachment meme = new Attachment("image/jpeg", "https://mmmpizzastorage.blob.core.windows.net/mmmpizzablob/abe_lincoln.jpg");
                Activity unknownResponse = (Activity)MessageFactory.Attachment(meme);
                unknownResponse.Text = "I have confusion and cannot help thee with that!  Ask for mine 'help' and I will explain what I can do for thee!";

                await turnContext.SendActivityAsync(unknownResponse, cancellationToken);
            }
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

        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = $"Huzzah weary traveller! With my oracle like powers, I aim to help thee fight the forces of Gloom!\n{helpText}";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }

        private async Task<List<BattleGoal>> GetBattleGoals()
        {
            var o = new DbContextOptionsBuilder<GHContext>();
            o.UseInMemoryDatabase("GH");
           
            GHContext context = new GHContext(o.Options);

            return await context.BattleGoals.ToListAsync<BattleGoal>();
        }
    }


}
