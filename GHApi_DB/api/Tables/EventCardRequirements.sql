CREATE TABLE [api].[EventCardRequirements] (
    [EventCardRequirementId] BIGINT        IDENTITY (1, 1) NOT NULL,
    [EventCardResultId]      BIGINT        NOT NULL,
    [RequirementType]        NVARCHAR (8)  NOT NULL,
    [RequirementDescription] NVARCHAR (32) NULL,
    [CharacterId]            BIGINT        NULL,
    CONSTRAINT [EventCardRequirementId] PRIMARY KEY CLUSTERED ([EventCardRequirementId] ASC),
    CONSTRAINT [FK_EventCardRequirements_Characters] FOREIGN KEY ([CharacterId]) REFERENCES [api].[Characters] ([CharacterId]),
    CONSTRAINT [FK_EventCardRequirements_EventCardResults] FOREIGN KEY ([EventCardResultId]) REFERENCES [api].[EventCardResults] ([EventCardResultId]) ON DELETE CASCADE
);

