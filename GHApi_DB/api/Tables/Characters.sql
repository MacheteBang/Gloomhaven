CREATE TABLE [api].[Characters] (
    [CharacterId]     BIGINT        IDENTITY (1, 1) NOT NULL,
    [CharacterNumber] VARCHAR (4)   NOT NULL,
    [FullName]        NVARCHAR (32) NOT NULL,
    [Race]            NVARCHAR (32) NOT NULL,
    [Class]           NVARCHAR (32) NOT NULL,
    [SpoilerFreeName] NVARCHAR (32) NOT NULL,
    [IsSpoiler]       BIT           NOT NULL,
    CONSTRAINT [PK_Characters] PRIMARY KEY CLUSTERED ([CharacterId] ASC)
);

