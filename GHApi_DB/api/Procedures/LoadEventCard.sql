Create Procedure api.LoadEventCard
	@EventType nvarchar (16) NULL,
	@CardNumber varchar (16) NULL,
	@Situation nvarchar (max) NULL,
	@OptionLetter_A nvarchar (1) NULL,
	@OptionDescription_A nvarchar (max) NULL,
	@Result_A_1 nvarchar (max) NULL,
	@CardDestiny_A_1 nvarchar (16) NULL,
	@RequirementType_A_1_Q1 nvarchar (16) NULL,
	@RequirementDescription_A_1_Q1 nvarchar (32) NULL,
	@CharacterID_A_1_Q1 bigint  NULL,
	@RequirementType_A_1_Q2 nvarchar (16) NULL,
	@RequirementDescription_A_1_Q2 nvarchar (32) NULL,
	@CharacterID_A_1_Q2 bigint  NULL,
	@RequirementType_A_1_Q3 nvarchar (16) NULL,
	@RequirementDescription_A_1_Q3 nvarchar (32) NULL,
	@CharacterID_A_1_Q3 bigint  NULL,
	@Reward_A_1_W1 nvarchar (128) NULL,
	@Reward_A_1_W2 nvarchar (128) NULL,
	@Reward_A_1_W3 nvarchar (128) NULL,
	@Result_A_2 nvarchar (max) NULL,
	@CardDestiny_A_2 nvarchar (16) NULL,
	@RequirementType_A_2_Q1 nvarchar (16) NULL,
	@RequirementDescription_A_2_Q1 nvarchar (32) NULL,
	@CharacterID_A_2_Q1 bigint  NULL,
	@RequirementType_A_2_Q2 nvarchar (16) NULL,
	@RequirementDescription_A_2_Q2 nvarchar (32) NULL,
	@CharacterID_A_2_Q2 bigint  NULL,
	@RequirementType_A_2_Q3 nvarchar (16) NULL,
	@RequirementDescription_A_2_Q3 nvarchar (32) NULL,
	@CharacterID_A_2_Q3 bigint  NULL,
	@Reward_A_2_W1 nvarchar (128) NULL,
	@Reward_A_2_W2 nvarchar (128) NULL,
	@Reward_A_2_W3 nvarchar (128) NULL,
	@OptionLetter_B nvarchar (1) NULL,
	@OptionDescription_B nvarchar (max) NULL,
	@Result_B_1 nvarchar (max) NULL,
	@CardDestiny_B_1 nvarchar (16) NULL,
	@RequirementType_B_1_Q1 nvarchar (8) NULL,
	@RequirementDescription_B_1_Q1 nvarchar (32) NULL,
	@CharacterID_B_1_Q1 bigint  NULL,
	@RequirementType_B_1_Q2 nvarchar (16) NULL,
	@RequirementDescription_B_1_Q2 nvarchar (32) NULL,
	@CharacterID_B_1_Q2 bigint  NULL,
	@RequirementType_B_1_Q3 nvarchar (16) NULL,
	@RequirementDescription_B_1_Q3 nvarchar (32) NULL,
	@CharacterID_B_1_Q3 bigint  NULL,
	@Reward_B_1_W1 nvarchar (128) NULL,
	@Reward_B_1_W2 nvarchar (128) NULL,
	@Reward_B_1_W3 nvarchar (128) NULL,
	@Result_B_2 nvarchar (max) NULL,
	@CardDestiny_B_2 nvarchar (16) NULL,
	@RequirementType_B_2_Q1 nvarchar (16) NULL,
	@RequirementDescription_B_2_Q1 nvarchar (32) NULL,
	@CharacterID_B_2_Q1 bigint  NULL,
	@RequirementType_B_2_Q2 nvarchar (16) NULL,
	@RequirementDescription_B_2_Q2 nvarchar (32) NULL,
	@CharacterID_B_2_Q2 bigint  NULL,
	@RequirementType_B_2_Q3 nvarchar (16) NULL,
	@RequirementDescription_B_2_Q3 nvarchar (32) NULL,
	@CharacterID_B_2_Q3 bigint  NULL,
	@Reward_B_2_W1 nvarchar (128) NULL,
	@Reward_B_2_W2 nvarchar (128) NULL,
	@Reward_B_2_W3 nvarchar (128) NULL
As
/*
Delete api.EventCards
*/

-- Insert Card
Insert Into api.EventCards (EventType, CardNumber, Situation)
Values (@EventType,@CardNumber,@Situation)

-- Get the identity for everything down stream.
Declare @EventCardId BigInt = Scope_Identity()

	Declare @EventCardOptionId BigInt
	Declare @EventCardResultId BigInt

	-- A - Insert Option A
	Insert Into api.EventCardOptions (EventCardId, OptionLetter, OptionDescription)
	Values (@EventCardId, @OptionLetter_A, @OptionDescription_A)

	Set @EventCardOptionId = Scope_Identity()
	
		-- A-1 - Result 1
		Insert Into api.EventCardResults (EventCardOptionId, Result, CardDestiny)
		Values (@EventCardOptionId, @Result_A_1, @CardDestiny_A_1)

		Set @EventCardResultId = Scope_Identity()

			-- A-1 - Requirements
			If (@RequirementType_A_1_Q1 Is Not Null)
				Insert Into api.EventCardRequirements (EventCardResultId, RequirementType, RequirementDescription, CharacterId)
				Values (@EventCardResultId, @RequirementType_A_1_Q1, @RequirementDescription_A_1_Q1, @CharacterID_A_1_Q1)

			If (@RequirementType_A_1_Q2 Is Not Null)
				Insert Into api.EventCardRequirements (EventCardResultId, RequirementType, RequirementDescription, CharacterId)
				Values (@EventCardResultId, @RequirementType_A_1_Q2, @RequirementDescription_A_1_Q2, @CharacterID_A_1_Q2)

			If (@RequirementType_A_1_Q3 Is Not Null)
				Insert Into api.EventCardRequirements (EventCardResultId, RequirementType, RequirementDescription, CharacterId)
				Values (@EventCardResultId, @RequirementType_A_1_Q3, @RequirementDescription_A_1_Q3, @CharacterID_A_1_Q3)

			-- A-1 - Rewards
			If (@Reward_A_1_W1 Is Not Null)
				Insert Into api.EventCardRewards (EventCardResultId, Reward)
				Values (@EventCardResultId, @Reward_A_1_W1)

			If (@Reward_A_1_W2 Is Not Null)
				Insert Into api.EventCardRewards (EventCardResultId, Reward)
				Values (@EventCardResultId, @Reward_A_1_W2)

			If (@Reward_A_1_W3 Is Not Null)
				Insert Into api.EventCardRewards (EventCardResultId, Reward)
				Values (@EventCardResultId, @Reward_A_1_W3)

		-- A-2 - Result 2
		If (@Result_A_2 Is Not Null) Begin
			Insert Into api.EventCardResults (EventCardOptionId, Result, CardDestiny)
			Values (@EventCardOptionId, @Result_A_2, @CardDestiny_A_2)

			Set @EventCardResultId = Scope_Identity()

				-- A-2 - Requirements
				If (@RequirementType_A_2_Q1 Is Not Null)
					Insert Into api.EventCardRequirements (EventCardResultId, RequirementType, RequirementDescription, CharacterId)
					Values (@EventCardResultId, @RequirementType_A_2_Q1, @RequirementDescription_A_2_Q1, @CharacterID_A_2_Q1)

				If (@RequirementType_A_2_Q2 Is Not Null)
					Insert Into api.EventCardRequirements (EventCardResultId, RequirementType, RequirementDescription, CharacterId)
					Values (@EventCardResultId, @RequirementType_A_2_Q2, @RequirementDescription_A_2_Q2, @CharacterID_A_2_Q2)

				If (@RequirementType_A_2_Q3 Is Not Null)
					Insert Into api.EventCardRequirements (EventCardResultId, RequirementType, RequirementDescription, CharacterId)
					Values (@EventCardResultId, @RequirementType_A_2_Q3, @RequirementDescription_A_2_Q3, @CharacterID_A_2_Q3)

				-- A-2 - Rewards
				If (@Reward_A_2_W1 Is Not Null)
					Insert Into api.EventCardRewards (EventCardResultId, Reward)
					Values (@EventCardResultId, @Reward_A_2_W1)

				If (@Reward_A_2_W2 Is Not Null)
					Insert Into api.EventCardRewards (EventCardResultId, Reward)
					Values (@EventCardResultId, @Reward_A_2_W2)

				If (@Reward_A_2_W3 Is Not Null)
					Insert Into api.EventCardRewards (EventCardResultId, Reward)
					Values (@EventCardResultId, @Reward_A_2_W3)
		End

	-- B - Insert Option B
	Insert Into api.EventCardOptions (EventCardId, OptionLetter, OptionDescription)
	Values (@EventCardId, @OptionLetter_B, @OptionDescription_B)

	Set @EventCardOptionId = Scope_Identity()
	
		-- B-1 - Result 1
		Insert Into api.EventCardResults (EventCardOptionId, Result, CardDestiny)
		Values (@EventCardOptionId, @Result_B_1, @CardDestiny_B_1)

		Set @EventCardResultId = Scope_Identity()

			-- B-1 - Requirements
			If (@RequirementType_B_1_Q1 Is Not Null)
				Insert Into api.EventCardRequirements (EventCardResultId, RequirementType, RequirementDescription, CharacterId)
				Values (@EventCardResultId, @RequirementType_B_1_Q1, @RequirementDescription_B_1_Q1, @CharacterID_B_1_Q1)

			If (@RequirementType_B_1_Q2 Is Not Null)
				Insert Into api.EventCardRequirements (EventCardResultId, RequirementType, RequirementDescription, CharacterId)
				Values (@EventCardResultId, @RequirementType_B_1_Q2, @RequirementDescription_B_1_Q2, @CharacterID_B_1_Q2)

			If (@RequirementType_B_1_Q3 Is Not Null)
				Insert Into api.EventCardRequirements (EventCardResultId, RequirementType, RequirementDescription, CharacterId)
				Values (@EventCardResultId, @RequirementType_B_1_Q3, @RequirementDescription_B_1_Q3, @CharacterID_B_1_Q3)

			-- B-1 - Rewards
			If (@Reward_B_1_W1 Is Not Null)
				Insert Into api.EventCardRewards (EventCardResultId, Reward)
				Values (@EventCardResultId, @Reward_B_1_W1)

			If (@Reward_B_1_W2 Is Not Null)
				Insert Into api.EventCardRewards (EventCardResultId, Reward)
				Values (@EventCardResultId, @Reward_B_1_W2)

			If (@Reward_B_1_W3 Is Not Null)
				Insert Into api.EventCardRewards (EventCardResultId, Reward)
				Values (@EventCardResultId, @Reward_B_1_W3)

		-- B-2 - Result 2
		If (@Result_B_2 Is Not Null) Begin
			Insert Into api.EventCardResults (EventCardOptionId, Result, CardDestiny)
			Values (@EventCardOptionId, @Result_B_2, @CardDestiny_B_2)

			Set @EventCardResultId = Scope_Identity()

				-- B-2 - Requirements
				If (@RequirementType_B_2_Q1 Is Not Null)
					Insert Into api.EventCardRequirements (EventCardResultId, RequirementType, RequirementDescription, CharacterId)
					Values (@EventCardResultId, @RequirementType_B_2_Q1, @RequirementDescription_B_2_Q1, @CharacterID_B_2_Q1)

				If (@RequirementType_B_2_Q2 Is Not Null)
					Insert Into api.EventCardRequirements (EventCardResultId, RequirementType, RequirementDescription, CharacterId)
					Values (@EventCardResultId, @RequirementType_B_2_Q2, @RequirementDescription_B_2_Q2, @CharacterID_B_2_Q2)

				If (@RequirementType_B_2_Q3 Is Not Null)
					Insert Into api.EventCardRequirements (EventCardResultId, RequirementType, RequirementDescription, CharacterId)
					Values (@EventCardResultId, @RequirementType_B_2_Q3, @RequirementDescription_B_2_Q3, @CharacterID_B_2_Q3)

				-- B-2 - Rewards
				If (@Reward_B_2_W1 Is Not Null)
					Insert Into api.EventCardRewards (EventCardResultId, Reward)
					Values (@EventCardResultId, @Reward_B_2_W1)

				If (@Reward_B_2_W2 Is Not Null)
					Insert Into api.EventCardRewards (EventCardResultId, Reward)
					Values (@EventCardResultId, @Reward_B_2_W2)

				If (@Reward_B_2_W3 Is Not Null)
					Insert Into api.EventCardRewards (EventCardResultId, Reward)
					Values (@EventCardResultId, @Reward_B_2_W3)
		End