using Godot;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

public class SignalManager : Node
{
	#region Post Encounter Scene Signals
	[Signal]
	delegate void levelUpPanelFinished();
	[Signal]
	delegate void levelUp(BasePlayerCharacter character);
	[Signal]
	delegate void experienceAdded();
	#endregion

	#region Text Box Signals
	[Signal]
	delegate void displayText(string text);
	[Signal]
	delegate void textBoxClosed();
	[Signal]
	delegate void characterText(string characterName, string abilityName, string targetName, float amount, bool likedPerformance, bool dislikedPerformace);
	[Signal]
	delegate void audienceText(string characterName, string abilityName, string targetName, float amount, bool likedPerformance, bool dislikedPerformace);
	#endregion

	#region Encounter Signals
	[Signal]
	delegate void toggleActionPanelVisibility();
	[Signal]
	delegate void useAbility(BaseAbilities ability, BattleInfo userBattleInfo, List<BattleInfo> characterBattleInfo);
	[Signal]
	delegate void checkAffinities(int characterIndex);
	[Signal]
	delegate void targetSelected();
	[Signal]
	delegate void actionSelected();
	[Signal]
	delegate void playerTurnEnded();
	[Signal]
	delegate void abilityFinished();
	[Signal]
	delegate void actionsCarriedOut();
	[Signal]
	delegate void moveArrow(Vector2 enemyUIPosition, Vector2 size);
	[Signal]
	delegate void hideArrow();
	[Signal]
	delegate void selectThisEnemy(string nodeName);
	[Signal]
	delegate void characterSelected(string name);
	[Signal]
	delegate void selectThisAbility(string name);
	[Signal]
	delegate void abilitySelected(string abilityName);
	[Signal]
	delegate void roundStarted();
	[Signal]
	delegate void selectThisItem(string name);
	[Signal]
	delegate void itemSelected(string name);
	[Signal]
	delegate void itemUsed();
	#endregion

	[Signal]
	delegate void SetNewColor(Color color, string part);

	#region Encounter Initiator Signals
	[Signal]
	delegate void battleStarted();
	[Signal]
	delegate void playCutscene(string cutsceneName);
	[Signal]
	delegate void setPostEncounterCutscene(string cutsceneName);
	#endregion

	#region Item Signals
	[Signal]
	delegate void addItem(string name);
	[Signal]
	delegate void removeItem(string name);
	[Signal]
	delegate void useItem(string name, int charToUseOn);
	#endregion

	#region SFX Signals
	[Signal]
	delegate void PlaySound(string soundName);
	[Signal]
	delegate void StopSound();
	#endregion
}
