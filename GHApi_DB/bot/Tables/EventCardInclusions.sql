CREATE TABLE [bot].[EventCardInclusions]
(
	[EventCardInclusionId] BIGINT IDENTITY (1, 1) NOT NULL,
    [TenantId] VARCHAR(64) NOT NULL, 
    [CardNumber] VARCHAR(16) NOT NULL, 
    [Action] CHAR NOT NULL,
    CONSTRAINT [PK_EventCardInclusions] PRIMARY KEY CLUSTERED ([EventCardInclusionId] ASC)
)
