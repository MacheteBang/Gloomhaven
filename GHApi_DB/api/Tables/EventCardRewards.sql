CREATE TABLE [api].[EventCardRewards] (
    [EventCardRewardId] BIGINT         IDENTITY (1, 1) NOT NULL,
    [EventCardResultId] BIGINT         NOT NULL,
    [Reward]            NVARCHAR (128) NOT NULL,
    CONSTRAINT [PK_EventCardRewards] PRIMARY KEY CLUSTERED ([EventCardRewardId] ASC),
    CONSTRAINT [FK_EventCardRewards_EventCardResults] FOREIGN KEY ([EventCardResultId]) REFERENCES [api].[EventCardResults] ([EventCardResultId]) ON DELETE CASCADE
);

