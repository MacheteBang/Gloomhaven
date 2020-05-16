using System;
using Microsoft.Bot.Schema;
using Microsoft.EntityFrameworkCore;

namespace GloomBot.Models.Bot
{
    public class BotDBContext : DbContext
    {
        public BotDBContext(DbContextOptions<BotDBContext> options) : base(options)
        {

        }

        public DbSet<ConversationMessage> ConversationMessages { get; set; }

        public void LogMessage (Activity activity)
        {

            // Add a timestamp and locale if there isn't one.
            DateTimeOffset? timestamp;
            if (activity.Timestamp == null)
            {
                timestamp = DateTimeOffset.Now.UtcDateTime;
            } else
            {
                timestamp = activity.Timestamp;
            }
            string locale;
            if (string.IsNullOrEmpty(activity.Locale))
            {
                locale = System.Threading.Thread.CurrentThread.CurrentCulture.ToString();
            } else
            {
                locale = activity.Locale;
            }

            ConversationMessage cm = new ConversationMessage();
            cm.ChannelId = activity.ChannelId;
            cm.ConversationId = activity.Conversation.Id;
            cm.FromId = activity.From.Id;
            cm.FromName = activity.From.Name;
            cm.FromRole = activity.From.Role;
            cm.MessageId = activity.Id;
            cm.LocalTimeStamp = timestamp;
            cm.Locale = locale;
            cm.RecipientId = activity.Recipient.Id;
            cm.RecipientRole = activity.Recipient.Role;
            cm.ReplyToId = activity.ReplyToId;
            cm.ServiceUrl = activity.ServiceUrl;
            cm.Text = activity.Text;

            ConversationMessages.Add(cm);
            this.SaveChanges();
        }
    }
}
