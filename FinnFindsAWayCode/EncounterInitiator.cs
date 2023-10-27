using Godot;
using System.Collections.Generic;

public class EncounterInitiator : Node
{
	Node currentLevel;
	Node encounterScene;
	Node resultsScene;

	PackedScene battleScene;
	PackedScene battleResult;

	SignalManager signalManager;

	string cutsceneToPlay;

	[Export]
	public string currentLevelName;

	Node inventroyValues;
	Node interactability;

    public override void _Ready()
	{
		currentLevel = GetChild(0);
		signalManager = GetNode<SignalManager>("/root/SignalManager");
		battleScene = (PackedScene)ResourceLoader.Load("res://Marko/Scenes/Marko'sTestScene.tscn");
		battleResult = (PackedScene)ResourceLoader.Load("res://Marko/Scenes/BattleResult.tscn");
		inventroyValues = GetNode<Node>("/root/InventroyValues");
		interactability = GetNode<Node>("/root/InteractabilityHandler");

		Node highestNode = GetHighestNode(this);

        if (highestNode.FindNode("DialogueBox") != null)
        {
			Node dialogueBox = highestNode.FindNode("DialogueBox");
			dialogueBox.AddUserSignal("StartBattle");
			dialogueBox.Connect("StartBattle", this, "_on_DialogueBox_dialogue_signal");
		}

		signalManager.Connect("battleStarted", this, "BattleStarted");
		signalManager.Connect("setPostEncounterCutscene", this, "SetPostEncounterCutscene");
    }

    Node GetHighestNode(Node thisNode)
    {
        if (thisNode.Owner != null)
        {
            thisNode = thisNode.Owner;
        }
        else
        {
            return thisNode;
        }
        return GetHighestNode(thisNode);
    }

    void BattleStarted()
	{
		CallDeferred("RemoveScene", currentLevel);
        encounterScene = battleScene.Instance();
		CallDeferred("AddScene", encounterScene);
        GetNode("/root/MusicManager").EmitSignal("PlayMusic", "MUS_Encounter", null);
		WipeOldInventoryAccessInfo();
		UpdateInEncounter(true);
    }

	void WipeOldInventoryAccessInfo()
    {
		inventroyValues.Call("ClearOldAccessInfo");
	}

	void UpdateInEncounter(bool doingEncounter)
    {
		inventroyValues.Call("UpdateInEncounter", doingEncounter);
		interactability.Call("SetCanInteract", !doingEncounter);
    }

    public void LoadResultScene()
	{
        resultsScene = battleResult.Instance();
        encounterScene.QueueFree();
		CallDeferred("AddScene", resultsScene);
	}

	public void FadeTransitionAfterEncounter()
	{
        signalManager.EmitSignal("playCutscene", "FadeFromBlack");
		GetTree().CreateTimer(0.05f).Connect("timeout", this, "LoadAfterTransition");
	}

	private void LoadAfterTransition()
	{
		if (IsInstanceValid(this))
		{
			resultsScene.QueueFree();
			CallDeferred("AddScene", currentLevel);
			GetNode("/root/MusicManager").EmitSignal("PlayMusic", "MUS_exploringTheHouse", null);

			if (cutsceneToPlay != null)
			{
				signalManager.EmitSignal("playCutscene", cutsceneToPlay);
				cutsceneToPlay = null;
			}
			WipeOldInventoryAccessInfo();
			UpdateInEncounter(false);
		}
	}

    void SetPostEncounterCutscene(string cutsceneName)
	{
		cutsceneToPlay = cutsceneName;
	}

	void RemoveScene(Node node)
	{
		RemoveChild(node);
	}

	void AddScene(Node node)
	{
		AddChild(node);
	}
	
	void _on_DialogueBox_dialogue_signal(List<string> data)
	{
		BattleStarted();
	}
}
