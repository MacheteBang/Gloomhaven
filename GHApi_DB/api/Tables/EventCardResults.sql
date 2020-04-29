CREATE TABLE [api].[EventCardResults] (
    [EventCardResultId] BIGINT         IDENTITY (1, 1) NOT NULL,
    [EventCardOptionId] BIGINT         NOT NULL,
    [Result]            NVARCHAR (MAX) NOT NULL,
    [CardDestiny]       NVARCHAR (16)  NOT NULL,
    CONSTRAINT [PK_EventCardResults] PRIMARY KEY CLUSTERED ([EventCardResultId] ASC),
    CONSTRAINT [FK_EventCardResults_EventCardOptions] FOREIGN KEY ([EventCardOptionId]) REFERENCES [api].[EventCardOptions] ([EventCardOptionId]) ON DELETE CASCADE
);

