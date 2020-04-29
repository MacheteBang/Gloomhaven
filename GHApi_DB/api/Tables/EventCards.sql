CREATE TABLE [api].[EventCards] (
    [EventCardID] BIGINT         IDENTITY (1, 1) NOT NULL,
    [EventType]   NVARCHAR (16)  NOT NULL,
    [CardNumber]  VARCHAR (16)   NOT NULL,
    [Situation]   NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_EventCards] PRIMARY KEY CLUSTERED ([EventCardID] ASC)
);

