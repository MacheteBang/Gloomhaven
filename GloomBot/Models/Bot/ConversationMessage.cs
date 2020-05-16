using System;
using System.ComponentModel.DataAnnotations;

namespace GloomBot.Models.Bot
{
    public class ConversationMessage
    {
        [Key]
        public long ConversationMessageId { get; set; }
        public string ChannelId { get; set; }
        public string ConversationId { get; set; }
        public string FromId { get; set; }
        public string FromName { get; set; }
        public string FromRole { get; set; }
        public string MessageId { get; set; }
        public DateTimeOffset? LocalTimeStamp { get; set; }
        public string Locale { get; set; }
        public string RecipientId { get; set; }
        public string RecipientRole { get; set; }
        public string ReplyToId { get; set; }
        public string ServiceUrl { get; set; }
        public string Text { get; set; }
    }
}
