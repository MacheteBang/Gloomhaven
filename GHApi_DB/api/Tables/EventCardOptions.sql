CREATE TABLE [api].[EventCardOptions] (
    [EventCardOptionId] BIGINT         IDENTITY (1, 1) NOT NULL,
    [EventCardId]       BIGINT         NOT NULL,
    [OptionLetter]      NVARCHAR (1)   NOT NULL,
    [OptionDescription] NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_EventCardOptions] PRIMARY KEY CLUSTERED ([EventCardOptionId] ASC),
    CONSTRAINT [FK_EventCardOptions_EventCards] FOREIGN KEY ([EventCardId]) REFERENCES [api].[EventCards] ([EventCardID]) ON DELETE CASCADE
);

