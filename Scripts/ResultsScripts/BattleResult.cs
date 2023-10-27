using Godot;
using System;
using System.Collections.Generic;

public class BattleResult : Control
{
	GlobalCharacterData globalCharacterData;
	SignalManager signalManager;

	EncounterInitiator encounterInitiator;

	ExperienceHandler experienceHandler;

	LevelUpPanel levelUpPanel;

	PackedScene resultPrefab;

	Label continueLabel;

	Panel resultsPanel;
	Panel losePanel;

	List<CharacterResultsUI> characterUIs = new List<CharacterResultsUI>();
	List<BasePlayerCharacter> partyMembers = new List<BasePlayerCharacter>();

	int totalExperience;
	int currentPartyMember = 0;
	bool readyToAddExperience;
	bool lost;


	public override void _Ready()
	{
		globalCharacterData = GetNode<GlobalCharacterData>("/root/GlobalCharacterData");
		resultsPanel = GetNode<Panel>("BattleResultPanel");
		losePanel = GetNode<Panel>("Lose");
		signalManager = GetNode<SignalManager>("/root/SignalManager");
		levelUpPanel = GetNode<LevelUpPanel>("BattleResultPanel/LevelUpPanel");
		continueLabel = GetNode<Label>("BattleResultPanel/Continue");
		encounterInitiator = GetParent() as EncounterInitiator;

		resultPrefab = (PackedScene)ResourceLoader.Load("res://Marko/Prefab/CharacterResultsUI.tscn");

		partyMembers = globalCharacterData.partyMembers;

		ShowBattleResult(globalCharacterData.battleOutcome);
	}

	public override void _Process(float delta)
	{
		if (Input.IsActionJustPressed("LeftMouseClick") && !lost)
		{
			if (readyToAddExperience && !levelUpPanel.Visible && currentPartyMember < partyMembers.Count)
			{
				continueLabel.Visible = false;
				AddExperienceToPartyMemeber(currentPartyMember);
			}
			else if (continueLabel.Visible)
			{
				encounterInitiator.FadeTransitionAfterEncounter();
			}
		}
	}

	void ShowBattleResult(Battle.BattleOutcome result)
	{
		if (result == Battle.BattleOutcome.won)
		{
			Won();
		}
		else if(result == Battle.BattleOutcome.run)
		{
			Run();
		}
		else
		{
			Lost();
		}
	}

	void Won()
	{

		VBoxContainer partyMembersUI = resultsPanel.GetNode<VBoxContainer>("ExperienceGained/PartyMembers");
		Label experiencedGained = resultsPanel.GetNode<Label>("ExperienceGained/EXPBattle");

		foreach (BaseEnemy enemy in globalCharacterData.enemyData)
		{
			totalExperience += enemy.experiencedDropped;
		}

		experiencedGained.Text += " +" + totalExperience;

		foreach (BasePlayerCharacter partyMember in partyMembers)
		{
			Node characterResultUI = resultPrefab.Instance();

			CharacterResultsUI characterResultsUI = characterResultUI.GetNode<CharacterResultsUI>(".");

			characterUIs.Add(characterResultsUI);

			characterResultsUI.InitializeUI(partyMember);

			partyMembersUI.AddChild(characterResultUI);            
		}

		continueLabel.Visible = true;
		readyToAddExperience = true;
	}

	async void AddExperienceToPartyMemeber(int partyMember)
	{
		readyToAddExperience = false;

		experienceHandler = new ExperienceHandler();

		AddChild(experienceHandler);

		experienceHandler.StartAddingExperience(characterUIs[partyMember], partyMembers[partyMember], totalExperience, signalManager);

		currentPartyMember++;

		await ToSignal(signalManager, "experienceAdded");

		readyToAddExperience = true;
		continueLabel.Visible = true;
	}

	void Run()
	{
		VBoxContainer partyMembersUI = resultsPanel.GetNode<VBoxContainer>("ExperienceGained/PartyMembers");
		Label resultLabel = resultsPanel.GetNode<Label>("Label");
		Label experiencedGained = resultsPanel.GetNode<Label>("ExperienceGained/EXPBattle");

		resultLabel.Text = "You got away!";
		experiencedGained.Text += " +" + 0;

		foreach (BasePlayerCharacter partyMember in partyMembers)
		{
			Node characterResultUI = resultPrefab.Instance();

			CharacterResultsUI characterResultsUI = characterResultUI.GetNode<CharacterResultsUI>(".");

			characterUIs.Add(characterResultsUI);

			characterResultsUI.InitializeUI(partyMember);

			partyMembersUI.AddChild(characterResultUI);
		}

		continueLabel.Visible = true;
	}

	void Lost()
	{
		resultsPanel.Hide();
		losePanel.Show();
		lost = true; 
		continueLabel.Visible = true;
	}
}
