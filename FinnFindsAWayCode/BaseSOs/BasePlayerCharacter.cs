using Godot;
using System;
using System.Collections.Generic;

public class BasePlayerCharacter : Resource
{
	[Export]
	public string name = "Character";
	[Export]
	public Texture texture = null;

	[Export]
	public int level = 1;
	public int experienceForLastLevel = 0;
	[Export]
	public int experienceForNextLevel;
	[Export]
	public int currentExperience;

	[Export]
	public Dictionary<string, float> baseStats = new Dictionary<string, float>()
	{
		{ "maxEnergy", 0 }, //Health
		{ "maxCreativity", 0 }, //Mana
		{ "baseStyle", 0 }, //Attack
		{ "baseFocus", 0 }, //Speed
		{ "baseSpice", 0 }, //Magic Multiplier
		{ "baseConfidence", 0 }, //Defense
	};

	[Export]
	public Dictionary<string, float> currentStatAmounts = new Dictionary<string, float>()
	{
		{ "currentEnergy", 0 },
		{ "currentCreativity", 0 },
		{ "currentStyle", 0 },
		{ "currentFocus", 0 },
		{ "currentSpice", 0 },
		{ "currentConfidence", 0 },
	};

	[Export]
	public bool inParty = false;

	[Export]
	public List<BaseAbilities> abilities;
	[Export]
	public List<string> affinites;

	[Export]
	public float energyOnLevelUp;
	[Export]
	public float creativityOnLevelUp;
	[Export]
	public float styleOnLevelUp;
	[Export]
	public float spiceOnLevelUp;
	[Export]
	public float confidenceOnLevelUp;
	[Export]
	public float focusOnLevelUp;

	public void ResetHealth()
	{
		currentStatAmounts["currentEnergy"] = baseStats["maxEnergy"];
	}

	public void DecreaseEnergy(float energyAmount)
	{
		if ((currentStatAmounts["currentEnergy"] - energyAmount) <= 0)
		{
			currentStatAmounts["currentEnergy"] = 0;
		}
		else
		{
			currentStatAmounts["currentEnergy"] -= energyAmount;
		}
	}

	public void IncreaseEnergy(float energyAmount)
	{
		if ((currentStatAmounts["currentEnergy"] + energyAmount) >= baseStats["maxEnergy"])
		{
			currentStatAmounts["currentEnergy"] = baseStats["maxEnergy"];
		}
		else
		{
			currentStatAmounts["currentEnergy"] += energyAmount;
		}
	}

	public void ModifyEnergy(float energyAmount)
    {
		if ((currentStatAmounts["currentEnergy"] + energyAmount) >= baseStats["maxEnergy"])
        {
			currentStatAmounts["currentEnergy"] = baseStats["maxEnergy"];
		}
		else if ((currentStatAmounts["currentEnergy"] + energyAmount) <= 0)
        {
			currentStatAmounts["currentEnergy"] = 0;
		}
		else
        {
			currentStatAmounts["currentEnergy"] += energyAmount;
		}
    }

	public void DecreaseCreativity(float creativityAmount)
	{
		if((currentStatAmounts["currentCreativity"] - creativityAmount) <= 0)
		{
			currentStatAmounts["currentCreativity"] = 0;
		}
		else
		{
			currentStatAmounts["currentCreativity"] -= creativityAmount;
		}
	}

	public void IncreaseCreativity(float creativityAmount)
	{
		if ((currentStatAmounts["currentCreativity"] + creativityAmount) >= baseStats["maxCreativity"])
		{
			currentStatAmounts["currentCreativity"] = baseStats["maxCreativity"];
		}
		else
		{
			currentStatAmounts["currentCreativity"] += creativityAmount;
		}
	}

	public void ModifyCreativity(float creativityAmount)
    {
		if ((currentStatAmounts["currentCreativity"] + creativityAmount) >= baseStats["maxCreativity"])
		{
			currentStatAmounts["currentCreativity"] = baseStats["maxCreativity"];
		}
		else if ((currentStatAmounts["currentCreativity"] + creativityAmount) <= 0)
		{
			currentStatAmounts["currentCreativity"] = 0;
		}
		else
        {
			currentStatAmounts["currentCreativity"] += creativityAmount;
		}
	}

	public void LevelUp()
	{
		baseStats["maxEnergy"] += energyOnLevelUp;
		currentStatAmounts["currentEnergy"] = baseStats["maxEnergy"];
		baseStats["maxCreativity"] += creativityOnLevelUp;
		currentStatAmounts["currentCreativity"] = baseStats["maxCreativity"];
		baseStats["baseStyle"] += styleOnLevelUp;
		currentStatAmounts["currentStyle"] = baseStats["baseStyle"];
		baseStats["baseSpice"] += spiceOnLevelUp;
		currentStatAmounts["currentSpice"] = baseStats["baseSpice"];
		baseStats["baseConfidence"] += confidenceOnLevelUp;
		currentStatAmounts["currentConfidence"] = baseStats["baseConfidence"];
		baseStats["baseFocus"] += focusOnLevelUp;
		currentStatAmounts["currentFocus"] = baseStats["baseFocus"];
	}

	public void IncreaseBaseStat(string statToIncrease, float increaseAmount)
	{
		baseStats[statToIncrease] += increaseAmount;
	}

	public void IncreaseStatCurrentAmount(string statToIncrease, float increaseAmount)
	{
		currentStatAmounts[statToIncrease] += increaseAmount;
	}

	public void ResetResource(BasePlayerCharacter resetData)
	{
		baseStats["maxEnergy"] = resetData.baseStats["maxEnergy"];
		currentStatAmounts["currentEnergy"] = baseStats["maxEnergy"];
		baseStats["maxCreativity"] = resetData.baseStats["maxCreativity"];
		currentStatAmounts["currentCreativity"] = baseStats["maxCreativity"];
		baseStats["baseStyle"] = resetData.baseStats["baseStyle"];
		currentStatAmounts["currentStyle"] = baseStats["baseStyle"];
		baseStats["baseSpice"] = resetData.baseStats["baseSpice"];
		currentStatAmounts["currentSpice"] = baseStats["baseSpice"];
		baseStats["baseConfidence"] = resetData.baseStats["baseConfidence"];
		currentStatAmounts["currentConfidence"] = baseStats["baseConfidence"];
		baseStats["baseFocus"] = resetData.baseStats["baseFocus"];
		currentStatAmounts["currentFocus"] = baseStats["baseFocus"];
	}

	public void ChangeAffinity(string affinity)
    {
		affinites.Clear();
		affinites.Add(affinity);
    }

}
