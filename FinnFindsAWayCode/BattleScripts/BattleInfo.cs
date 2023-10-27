using Godot;
using System.Collections.Generic;

public class BattleInfo : Node
{
	public enum characterTypes { player, enemy };
	public enum actions { attack, defend, useAbility, useItem, run }

	public Node UI;
	public CharacterUI uiScript;

	public Dictionary<string, float> currentStatAmounts = new Dictionary<string, float>()
	{
		{ "currentEntertainment",  0},
		{ "currentEnergy", 0 },
		{ "currentCreativity", 0 },
		{ "currentCriticality", 0 },
		{ "currentFocus", 0 },
		{ "currentSpice", 0 },
		{ "currentInsensitivity", 0 },
	};

	public int characterIndex;
	public int targetIndex;
	public int currentRound;
	public bool stunned;
	public bool slimeDefense;
	public bool entertainedThisRound;
	public bool likedPerformance;
	public bool dislikedPerformance;
	public bool reverseNegativeAffinities;
	public string abilityToUse;
	public string itemToUse;

	public characterTypes characterType;
	public actions actionChosen;

	public void HealEnemy(float healAmount)
	{
		if ((currentStatAmounts["currentEntertainment"] - healAmount) < 0)
		{
			currentStatAmounts["currentEntertainment"] = 0;
		}
		else
		{
			currentStatAmounts["currentEntertainment"] -= healAmount;
		}
	}

	public void DamageEnemy(float damageAmount, float maxEntertainment, string enemyName)
	{
		uiScript.PlayEnemyHurt(enemyName);
		if (currentStatAmounts["currentEntertainment"] + damageAmount >= maxEntertainment)
		{
			currentStatAmounts["currentEntertainment"] = maxEntertainment;

			uiScript.PlayCharacterEntertained(enemyName);
        }
		else
		{
			currentStatAmounts["currentEntertainment"] += damageAmount;
		}
	}    
}
