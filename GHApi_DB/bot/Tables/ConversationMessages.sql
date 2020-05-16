CREATE TABLE [bot].[ConversationMessages]
(
	[ConversationMessageId] BIGINT NOT NULL IDENTITY(1, 1), 
    [ChannelId] VARCHAR(64) NULL, 
    [ConversationId] VARCHAR(64) NOT NULL, 
    [FromId] VARCHAR(64) NULL,
    FromName Varchar(64) Null,
    FromRole VarChar(64) Null,
    MessageId VarChar(64) Null,
    LocalTimeStamp DateTimeOffset Null,
    Locale VarChar(8) Null,
    RecipientId VarChar(64) Null,
    RecipientRole VarChar(64) Null,
    ReplyToId VarChar(64) Null,
    ServiceUrl VarChar(64) Null,
    [Text] NVARCHAR(MAX) NULL, 
    CONSTRAINT [PK_ConversationMessages] PRIMARY KEY CLUSTERED ([ConversationMessageId] ASC)
)
