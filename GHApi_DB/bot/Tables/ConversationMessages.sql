CREATE TABLE [bot].[ConversationMessages]
(
	[ConversationMessageId] BIGINT NOT NULL IDENTITY(1, 1), 
    [ChannelId] VARCHAR(256) NULL, 
    [ConversationId] VARCHAR(256) NOT NULL, 
    [FromId] VARCHAR(256) NULL,
    FromName Varchar(64) Null,
    FromRole VarChar(64) Null,
    MessageId VarChar(256) Null,
    LocalTimeStamp DateTimeOffset Null,
    Locale VarChar(8) Null,
    RecipientId VarChar(256) Null,
    RecipientRole VarChar(64) Null,
    ReplyToId VarChar(256) Null,
    ServiceUrl VarChar(64) Null,
    [Text] NVARCHAR(MAX) NULL, 
    CONSTRAINT [PK_ConversationMessages] PRIMARY KEY CLUSTERED ([ConversationMessageId] ASC)
)
