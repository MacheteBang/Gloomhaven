CREATE TABLE [api].[BattleGoals] (
    [BattleGoalId]    BIGINT         IDENTITY (1, 1) NOT NULL,
    [CardNumber]      VARCHAR (16)   NULL,
    [GoalName]        NVARCHAR (50)  NOT NULL,
    [GoalDescription] NVARCHAR (128) NOT NULL,
    [Reward]          INT            NOT NULL,
    [Source]          NVARCHAR (32)  NOT NULL,
    [IsOfficial]      BIT            NOT NULL,
    [IsExtended]      BIT            NOT NULL,
    CONSTRAINT [PK_BattleGoals] PRIMARY KEY CLUSTERED ([BattleGoalId] ASC)
);

