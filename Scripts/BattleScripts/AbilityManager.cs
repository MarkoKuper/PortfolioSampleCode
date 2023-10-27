using Godot;
using System;
using System.Collections.Generic;

public class AbilityManager : Control
{
	VBoxContainer container;

	GlobalCharacterData globalCharacterData;

	SignalManager signalManager;

	PackedScene baseButton;

	List<Button> abilityButtons = new List<Button>();

	List<BaseAbilities> abilities = new List<BaseAbilities>();

	List<BaseEnemy> enemyData;

	List<BasePlayerCharacter> partyMembers;

	bool debuffFlashy;

	public bool encounterOver;

	public override void _Ready()
	{
		container = GetNode<VBoxContainer>("AbilitiesContainer");
		globalCharacterData = GetNode<GlobalCharacterData>("/root/GlobalCharacterData");
		enemyData = globalCharacterData.enemyData;
		partyMembers = globalCharacterData.partyMembers;
		signalManager = GetNode<SignalManager>("/root/SignalManager");
		baseButton = (PackedScene)ResourceLoader.Load("res://Marko/Prefab/ButtonBase.tscn");

		signalManager.Connect("selectThisAbility", this, "ButtonPressed");
	}

	//Uses the ability given in abilityName.
	public async void UseAbility(BaseAbilities ability, BattleInfo userBattleInfo, List<BattleInfo> characterBattleInfo)
	{
		if(ability.abilityType == BaseAbilities.abilityTypes.Damage)
		{
			DamageAbility(characterBattleInfo, userBattleInfo, ability);
		}
		else if(ability.abilityType == BaseAbilities.abilityTypes.Heal)
		{
			HealingAbility(characterBattleInfo, userBattleInfo, ability);
		}
		else if(ability.abilityType == BaseAbilities.abilityTypes.Debuff)
		{
			DebuffAbility(characterBattleInfo, userBattleInfo, ability);
		}
		else if(ability.abilityType == BaseAbilities.abilityTypes.Buff)
		{
			BuffAbility(characterBattleInfo, userBattleInfo, ability);
		}
		else if(ability.abilityType == BaseAbilities.abilityTypes.Defence)
		{
			if(ability.abilityName == "Observe")
			{
				Observe(userBattleInfo);
			}
			else if(ability.abilityName == "Slimastic Defense")
			{
				SlimasticDefense(userBattleInfo, ability);
			}
		}
		else if(ability.abilityType == BaseAbilities.abilityTypes.BuffAffinity)
		{
			Inspire(characterBattleInfo,userBattleInfo, ability);
		}
		else if(ability.abilityType == BaseAbilities.abilityTypes.DebuffAffinity)
		{
			EnforcePreference(userBattleInfo, ability);
		}
		else if(ability.abilityType == BaseAbilities.abilityTypes.Passive)
		{
			FinnAudience(userBattleInfo);
		}
		await ToSignal(signalManager, "abilityFinished");
		signalManager.EmitSignal("actionsCarriedOut");
		if (ability.targetsEnemy) signalManager.EmitSignal("PlaySound", "ENC_Talent_Audience", null);
		else signalManager.EmitSignal("PlaySound", "ENC_Talent_Self", null);
	}

	//Adds buttons with the abilities held by the party member whos turn it is.
	public void InitializePanel(int partyMemberTurn)
	{
		abilities = new List<BaseAbilities>();
		if (abilityButtons.Count > 0) //Clears the panel of old ability buttons.
		{
			for(int a = 0; a < abilityButtons.Count; a++)
			{
				container.RemoveChild(abilityButtons[a]);
			}
			abilityButtons.Clear();
		}
		SetSize(new Vector2(0, 0), true);
		for (int i = 0; i < partyMembers[partyMemberTurn].abilities.Count; i++)
		{
			Button newButton = baseButton.Instance<Button>(); 
			newButton.Name = partyMembers[partyMemberTurn].abilities[i].abilityName; 
			newButton.Text = partyMembers[partyMemberTurn].abilities[i].abilityName;
			container.AddChild(newButton);
			abilityButtons.Add(newButton);
			abilities.Add(globalCharacterData.partyMembers[partyMemberTurn].abilities[i]);
		}
		for (int i = 0; i < abilityButtons.Count; i++)        
		{

			if (abilities[i].abilityCost > partyMembers[partyMemberTurn].currentStatAmounts["currentCreativity"])
			{
				abilityButtons[i].Disabled = true;
			}
		}
		SetSize(new Vector2(RectSize.x + 20f, RectSize.y + 40f), true);
	}

	//Toggles the button availability;
	public void ToggleButtonAvailability(int partyMemberTurn)
	{
		for(int a = 0; a < abilityButtons.Count; a++)
		{
			if(abilities[a].abilityCost > partyMembers[partyMemberTurn].currentStatAmounts["currentCreativity"])
			{
				abilityButtons[a].Disabled = true;
			}
			else
			{
				abilityButtons[a].Disabled = !abilityButtons[a].Disabled;
			}
		}
	}

	//Emits a signal that allows other scripts to know that an ability was selected.
	public void ButtonPressed(string name)
	{
		for(int i = 0; i < abilities.Count; i++)
		{
			if(name == abilities[i].abilityName)
			{
				signalManager.EmitSignal("abilitySelected", name);
			}
		}
	}

	void DebuffAbility(List<BattleInfo> characterBattleInfo, BattleInfo userInfo, BaseAbilities ability)
	{
		if (ability.targetsEnemy)
		{

		}
		else
		{
			DebuffPartyMember(userInfo, ability);
			//signalManager.EmitSignal("PlaySound", "ENC_Talent_Self", null);
		}
	}

	void DebuffEnemy(List<BattleInfo> characterBattleInfo, BattleInfo userInfo, BaseAbilities ability)
	{

	}

	async void DebuffPartyMember(BattleInfo userInfo, BaseAbilities ability)
	{
		BaseEnemy myData = enemyData[userInfo.characterIndex];
		BasePlayerCharacter targetCharacter = partyMembers[userInfo.targetIndex];
		float debuffAmount = -Mathf.Floor(myData.baseStats["baseSpice"] * 0.2f + ability.abilityEffectAmount);
		foreach (var stat in ability.affectedStats)
		{
			targetCharacter.currentStatAmounts["current" + stat] += debuffAmount;
		}
		userInfo.currentStatAmounts["currentCreativity"] += ability.abilityCost;
		signalManager.EmitSignal("audienceText", myData.name, ability.abilityName, targetCharacter.name, 0, false, false);
		await ToSignal(signalManager, "textBoxClosed");
		signalManager.EmitSignal("abilityFinished");
		foreach (var stat in ability.affectedStats)
		{
			WaitForRounds(userInfo, null, targetCharacter, userInfo.currentRound, true, ability.abilityLength, stat.ToString(), debuffAmount);
		}
	}

	void BuffAbility(List<BattleInfo> characterBattleInfo, BattleInfo userInfo, BaseAbilities ability)
	{
		if (ability.targetsEnemy)
		{
			BuffEnemy(characterBattleInfo, userInfo, ability);
			//signalManager.EmitSignal("PlaySound", "ENC_Talent_Audience", null);
		}
		else
		{
			BuffPartyMember(characterBattleInfo, userInfo, ability);
			//signalManager.EmitSignal("PlaySound", "ENC_Talent_Self", null);
		}
	}

	async void BuffEnemy(List<BattleInfo> characterBattleInfo, BattleInfo userInfo, BaseAbilities ability)
	{
		BaseEnemy myData = enemyData[userInfo.characterIndex];
		float buffAmount = Mathf.Floor(myData.baseStats["baseSpice"] * 0.2f + ability.abilityEffectAmount);
		if (ability.needsTarget)
		{
			List<BattleInfo> enemies = CreateEnemyList(characterBattleInfo);
			BaseEnemy targetData = enemyData[userInfo.targetIndex];
			BattleInfo targetCharacter = enemies[userInfo.targetIndex];
			foreach (var stat in ability.affectedStats)
			{
				targetCharacter.currentStatAmounts["current" + stat] += buffAmount;
			}
			userInfo.currentStatAmounts["currentCreativity"] -= ability.abilityCost;
			signalManager.EmitSignal("audienceText", myData.name, ability.abilityName, targetData.name, buffAmount, false, false);
			await ToSignal(signalManager, "textBoxClosed");
			foreach (var stat in ability.affectedStats)
			{
				WaitForRounds(userInfo, targetCharacter, null, userInfo.currentRound, false, ability.abilityLength, stat.ToString(), buffAmount);
			}
		}
		else
		{
			foreach (var stat in ability.affectedStats)
			{
				userInfo.currentStatAmounts["current" + stat] += buffAmount;
			}
			userInfo.currentStatAmounts["currentCreativity"] -= ability.abilityCost;
			signalManager.EmitSignal("audienceText", myData.name, ability.abilityName, null, 0, false, false);
			await ToSignal(signalManager, "textBoxClosed");
			foreach (var stat in ability.affectedStats)
			{
				WaitForRounds(userInfo, userInfo, null, userInfo.currentRound, false, ability.abilityLength, stat.ToString(), buffAmount);
			}
		}
		signalManager.EmitSignal("abilityFinished");
	}

	async void BuffPartyMember(List<BattleInfo> characterBattleInfo, BattleInfo userInfo, BaseAbilities ability)
	{
		if (ability.needsTarget)
		{
			int lengthOfAbility = ability.abilityLength;
			BasePlayerCharacter myData = partyMembers[userInfo.characterIndex];
			BasePlayerCharacter targetData = partyMembers[userInfo.targetIndex];
			float buffAmount = Mathf.Floor(myData.currentStatAmounts["currentSpice"] * 0.3f + ability.abilityEffectAmount);
			foreach (var stat in ability.affectedStats)
			{
				targetData.currentStatAmounts["current" + stat] += buffAmount;
			}
			myData.currentStatAmounts["currentCreativity"] -= ability.abilityCost;
			userInfo.uiScript.SetCreativity(myData.currentStatAmounts["currentCreativity"], myData.baseStats["maxCreativity"]);
			signalManager.EmitSignal("characterText", myData.name, ability.abilityName, targetData.name, buffAmount, false, false);
			await ToSignal(signalManager, "textBoxClosed");
			foreach (var stat in ability.affectedStats)
			{
				WaitForRounds(userInfo, null, targetData, userInfo.currentRound, true, lengthOfAbility, stat.ToString(), buffAmount);
			}
		}
		else
		{
			int lengthOfAbility = ability.abilityLength;
			BasePlayerCharacter myData = partyMembers[userInfo.characterIndex];
			float buffAmount = Mathf.Floor(myData.currentStatAmounts["currentSpice"] * 0.3f + ability.abilityEffectAmount);
			foreach (var stat in ability.affectedStats)
			{
				myData.currentStatAmounts["current" + stat] += buffAmount;
			}
			signalManager.EmitSignal("characterText", myData.name, ability.abilityName, null, buffAmount, false, false);
			await ToSignal(signalManager, "textBoxClosed");
			myData.DecreaseCreativity(ability.abilityCost);
			userInfo.uiScript.SetCreativity(myData.currentStatAmounts["currentCreativity"], myData.baseStats["maxCreativity"]);
			foreach (var stat in ability.affectedStats)
			{
				WaitForRounds(userInfo, null, myData, userInfo.currentRound, true, lengthOfAbility, stat.ToString(), buffAmount);
			}
		}
		signalManager.EmitSignal("abilityFinished");
	}

	void DamageAbility(List<BattleInfo> characterBattleInfo, BattleInfo userBattleInfo, BaseAbilities ability)
	{
		if (ability.targetsEnemy)
		{
			DamageEnemy(characterBattleInfo, userBattleInfo, ability);
			//signalManager.EmitSignal("PlaySound", "ENC_Talent_Audience", null);
		}
		else
		{
			DamagePartyMember(characterBattleInfo, userBattleInfo, ability);
			//signalManager.EmitSignal("PlaySound", "ENC_Talent_Self", null);
		}
	}

	async void DamageEnemy(List<BattleInfo> characterBattleInfo, BattleInfo userBattleInfo, BaseAbilities ability)
	{
		List<BattleInfo> enemies = CreateEnemyList(characterBattleInfo);
		BaseEnemy targetData = enemyData[userBattleInfo.targetIndex];
		BattleInfo targetBattleInfo = enemies[userBattleInfo.targetIndex];
		BasePlayerCharacter myData = partyMembers[userBattleInfo.characterIndex];
		float damageAmount = Mathf.Floor(myData.currentStatAmounts["currentSpice"] * 0.5f + ability.abilityEffectAmount);
		if (IsAffinityPresent(myData, "Flashy") && debuffFlashy)
		{
			damageAmount *= 0.75f;
		}
		if(ability.abilityAffinity == BaseAbilities.affinities.None)
		{
			signalManager.EmitSignal("checkAffinities", userBattleInfo, null);
		}
		else
		{
			signalManager.EmitSignal("checkAffinities", userBattleInfo, ability.abilityAffinity.ToString());
		}
		if (ability.stun)
		{
			targetBattleInfo.stunned = true;
		}
		signalManager.EmitSignal("characterText", myData.name, ability.abilityName, targetData.name, damageAmount, false, false);
		await ToSignal(signalManager, "textBoxClosed");
		targetBattleInfo.DamageEnemy(damageAmount, targetData.baseStats["maxEntertainment"], targetData.name);
		targetBattleInfo.uiScript.SetHealth(targetBattleInfo.currentStatAmounts["currentEntertainment"], targetData.baseStats["maxEntertainment"]);
		if (targetBattleInfo.currentStatAmounts["currentEntertainment"] >= targetData.baseStats["maxEntertainment"])
		{
			signalManager.EmitSignal("displayText", targetData.name + " is completely entertained!");
			await ToSignal(signalManager, "textBoxClosed");
		}
		myData.DecreaseCreativity(ability.abilityCost);
		userBattleInfo.uiScript.SetCreativity(myData.currentStatAmounts["currentCreativity"], myData.baseStats["maxCreativity"]);
		signalManager.EmitSignal("abilityFinished");
	}

	async void DamagePartyMember(List<BattleInfo> characterBattleInfo, BattleInfo userBattleInfo, BaseAbilities ability)
	{

		BaseEnemy myData = enemyData[userBattleInfo.characterIndex];
		BasePlayerCharacter partyMember = partyMembers[userBattleInfo.targetIndex];
		BattleInfo targetBattleInfo = new BattleInfo();
		float damageAmount = Mathf.Floor(userBattleInfo.currentStatAmounts["currentCriticality"] * 0.2f + ability.abilityEffectAmount);
		foreach (BattleInfo charInfo in characterBattleInfo)
		{
			if (charInfo.characterType == BattleInfo.characterTypes.player && charInfo.characterIndex == userBattleInfo.targetIndex)
			{
				targetBattleInfo = charInfo;
				break;
			}
		}
		if (targetBattleInfo.actionChosen == BattleInfo.actions.defend)
		{
			damageAmount = Mathf.Floor(damageAmount * 0.5f);
		}
		partyMember.DecreaseEnergy(damageAmount);
		targetBattleInfo.uiScript.PlayCharacterHurt();
		targetBattleInfo.uiScript.SetHealth(partyMember.currentStatAmounts["currentEnergy"], partyMember.baseStats["maxEnergy"]);
		userBattleInfo.currentStatAmounts["currentCreativity"] -= ability.abilityCost;
		signalManager.EmitSignal("audienceText", myData.name, ability.abilityName, partyMember.name, damageAmount, false, false);
		await ToSignal(signalManager, "textBoxClosed");
		signalManager.EmitSignal("abilityFinished");
	}

	void HealingAbility(List<BattleInfo> characterBattleInfo, BattleInfo userBattleInfo, BaseAbilities ability)
	{
		if (ability.targetsEnemy)
		{
			HealEnemy(characterBattleInfo, userBattleInfo, ability);
			//signalManager.EmitSignal("PlaySound", "ENC_Talent_Audience", null);
		}
		else
		{
			HealPartyMember(characterBattleInfo, userBattleInfo, ability);
			//signalManager.EmitSignal("PlaySound", "ENC_Talent_Self", null);
		}
	}

	async void HealEnemy(List<BattleInfo> characterBattleInfo, BattleInfo userBattleInfo, BaseAbilities ability)
	{
		BaseEnemy myData = enemyData[userBattleInfo.characterIndex];
		float healAmount = Mathf.Floor(myData.baseStats["baseSpice"] * 0.3f + ability.abilityEffectAmount);
		if (ability.needsTarget)
		{
			List<BattleInfo> enemies = CreateEnemyList(characterBattleInfo);
			BaseEnemy targetData = enemyData[userBattleInfo.targetIndex];
			BattleInfo targetCharacter = enemies[userBattleInfo.targetIndex];            
			targetCharacter.HealEnemy(healAmount);
			targetCharacter.uiScript.SetHealth(targetCharacter.currentStatAmounts["currentEntertainment"], targetData.baseStats["maxEntertainment"]);
			userBattleInfo.currentStatAmounts["currentCreativity"] -= ability.abilityCost;
			signalManager.EmitSignal("audienceText", myData.name, ability.abilityName, targetData.name, healAmount, false, false);
		}
		else if(ability.targetsEveryone)
		{
			List<BattleInfo> enemies = CreateEnemyList(characterBattleInfo);
			foreach (BattleInfo enemy in enemies)
			{
				BaseEnemy targetData = enemyData[enemy.characterIndex];
				enemy.HealEnemy(healAmount);
				enemy.uiScript.SetHealth(enemy.currentStatAmounts["currentEntertainment"], targetData.baseStats["maxEntertainment"]);
			}
			userBattleInfo.currentStatAmounts["currentCreativity"] -= ability.abilityCost;
			signalManager.EmitSignal("audienceText", myData.name, ability.abilityName, null, healAmount, false, false);
		}
		else
		{
			userBattleInfo.HealEnemy(healAmount);
			userBattleInfo.uiScript.SetHealth(userBattleInfo.currentStatAmounts["currentEntertainment"], myData.baseStats["maxEntertainment"]);
			userBattleInfo.currentStatAmounts["currentCreativity"] -= ability.abilityCost;
			signalManager.EmitSignal("audienceText", myData.name, ability.abilityName, null, 0, false, false);
		}
		await ToSignal(signalManager, "textBoxClosed");
		signalManager.EmitSignal("abilityFinished");
	}

	async void HealPartyMember(List<BattleInfo> characterBattleInfo, BattleInfo userBattleInfo, BaseAbilities ability)
	{
		BasePlayerCharacter myData = partyMembers[userBattleInfo.characterIndex];
		List<BattleInfo> partyInfo = CreatePartyList(characterBattleInfo);
		float energyAmount = Mathf.Floor(myData.currentStatAmounts["currentSpice"] * 0.4f + ability.abilityEffectAmount);
		if (ability.targetsEveryone)
		{
			foreach (BattleInfo character in partyInfo)
			{
				BasePlayerCharacter characterData = partyMembers[character.characterIndex];
				characterData.IncreaseEnergy(energyAmount);
				character.uiScript.SetHealth(characterData.currentStatAmounts["currentEnergy"], characterData.baseStats["maxEnergy"]);
			}
			signalManager.EmitSignal("characterText", myData.name, ability.abilityName, null, energyAmount, false, false);
			await ToSignal(signalManager, "textBoxClosed");
		}
		else
		{

		}
		myData.DecreaseCreativity(ability.abilityCost);
		userBattleInfo.uiScript.SetCreativity(myData.currentStatAmounts["currentCreativity"], myData.baseStats["maxCreativity"]);
		signalManager.EmitSignal("abilityFinished");
	}

	async void Observe(BattleInfo characterInfo)
	{
		BaseEnemy myData = enemyData[characterInfo.characterIndex];
		int lengthOfAbility = 1;
		characterInfo.actionChosen = BattleInfo.actions.defend;
		signalManager.EmitSignal("audienceText", enemyData[characterInfo.characterIndex].name, "Observe", null, 0, false, false);
		await ToSignal(signalManager, "textBoxClosed");
		signalManager.EmitSignal("abilityFinished");
		WaitForRounds(characterInfo, characterInfo, null, characterInfo.currentRound, false, lengthOfAbility, "Defend", 0);
	}

	async void EnforcePreference(BattleInfo characterInfo, BaseAbilities ability)
	{
		BaseEnemy myData = enemyData[characterInfo.characterIndex];
		debuffFlashy = true;
		characterInfo.currentStatAmounts["currentCreativity"] -= ability.abilityCost;
		signalManager.EmitSignal("audienceText", myData.name, ability.abilityName, null, 0, false, false);
		await ToSignal(signalManager, "textBoxClosed");
		signalManager.EmitSignal("abilityFinished");
		WaitForRounds(characterInfo, characterInfo, null, characterInfo.currentRound, false, ability.abilityLength, "DebuffFlashy", 0);
	}

	async void SlimasticDefense(BattleInfo characterInfo, BaseAbilities ability)
	{
		BaseEnemy myData = enemyData[characterInfo.characterIndex];
		characterInfo.slimeDefense = true;
		characterInfo.currentStatAmounts["currentCreativity"] -= ability.abilityCost;
		signalManager.EmitSignal("audienceText", myData.name, ability.abilityName, null, 0, false, false);
		await ToSignal(signalManager, "textBoxClosed");
		signalManager.EmitSignal("abilityFinished");
		WaitForRounds(characterInfo, characterInfo, null, characterInfo.currentRound, false, ability.abilityLength, "SlimeDefense", 0);
	}

	async void Inspire(List<BattleInfo> characterBattleInfo, BattleInfo characterInfo, BaseAbilities ability)
	{
		BasePlayerCharacter myInfo = partyMembers[characterInfo.characterIndex];
		List<BattleInfo> enemies = CreateEnemyList(characterBattleInfo);
		BattleInfo targetCharacter = enemies[characterInfo.targetIndex];
		BaseEnemy targetData = enemyData[characterInfo.targetIndex];
		targetCharacter.reverseNegativeAffinities = true;
		myInfo.currentStatAmounts["currentCreativity"] -= ability.abilityCost;
		signalManager.EmitSignal("characterText", myInfo.name, ability.abilityName, targetData.name, 0, false, false);
		await ToSignal(signalManager, "textBoxClosed");
		characterInfo.uiScript.SetCreativity(myInfo.currentStatAmounts["currentCreativity"], myInfo.baseStats["maxCreativity"]);
		signalManager.EmitSignal("abilityFinished");
		WaitForRounds(characterInfo, targetCharacter, null, characterInfo.currentRound, false, ability.abilityLength, "ReverseNegativeAffinities", 0);
	}

	async void FinnAudience(BattleInfo characterInfo)
	{
		signalManager.EmitSignal("audienceText", "FinnAudience", null, 0, null, false, false);
		await ToSignal(signalManager, "textBoxClosed");
		signalManager.EmitSignal("abilityFinished");
	}

	List<BattleInfo> CreateEnemyList(List<BattleInfo> characterBattleInfo)
	{
		List<BattleInfo> enemies = new List<BattleInfo>();
		foreach (BattleInfo character in characterBattleInfo)
		{
			if (character.characterType == BattleInfo.characterTypes.enemy)
			{
				enemies.Add(character);
			}
		}
		return enemies;
	}

	List<BattleInfo> CreatePartyList(List<BattleInfo> characterBattleInfo)
	{
		List<BattleInfo> party = new List<BattleInfo>();
		foreach (BattleInfo character in characterBattleInfo)
		{
			if (character.characterType == BattleInfo.characterTypes.player)
			{
				party.Add(character);
			}
		}
		return party;
	}

	//Waits the amount rounds for the abilities length and then resets the stat that was changed to its original amount
	async void WaitForRounds(BattleInfo character, BattleInfo targetEnemy, BasePlayerCharacter targetPartyMember, int roundAbilityWasUsed, bool isPlayer, int lenghtOfAbility, string statChanged, float amountStatWasChangedBy)
	{
		int originalRound = roundAbilityWasUsed;
		await ToSignal(signalManager, "roundStarted");
		if ((originalRound + lenghtOfAbility) <= character.currentRound || encounterOver)
		{
			if (statChanged == "ReverseNegativeAffinities" || statChanged == "DebuffFlashy" || statChanged == "Defend" || statChanged == "SlimeDefense")
			{
				ChangeSpecialEffectsBack(targetEnemy, statChanged, amountStatWasChangedBy);
			}
			else if (isPlayer)
			{
				targetPartyMember.currentStatAmounts["current" + statChanged] -= amountStatWasChangedBy;
			}
			else
			{
				targetEnemy.currentStatAmounts["current" + statChanged] -= amountStatWasChangedBy;
			}
		}
		else
		{
			WaitForRounds(character, targetEnemy, targetPartyMember, originalRound, isPlayer, lenghtOfAbility, statChanged, amountStatWasChangedBy);
		}
	}

	//Changes an enemy's stat back to its original state by amountStatWasChangedBy
	void ChangeSpecialEffectsBack(BattleInfo targetCharacter, string statChanged, float amountStatWasChangedBy)
	{
		switch (statChanged)
		{
			case "ReverseNegativeAffinities":
				{
					targetCharacter.reverseNegativeAffinities = false;
					break;
				}
			case "DebuffFlashy":
				{
				   debuffFlashy = false;
				   break;
				}
			case "Defend":
				{
					targetCharacter.actionChosen = BattleInfo.actions.attack;
					break;
				}
			case "SlimeDefense":
				{
					targetCharacter.slimeDefense = false;
					break;
				}
		}
	}

	//Returns true if an affinity is present on a party member, otherwise it returns false
	bool IsAffinityPresent(BasePlayerCharacter partyInfo, string affintiyToLookFor)
	{
		foreach (string affinity in partyInfo.affinites)
		{
			if (affinity == affintiyToLookFor)
			{
				return true;
			}
		}
		return false;
	}
}
