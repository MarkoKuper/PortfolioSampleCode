using Godot;
using System.Collections.Generic;

public class EnemyOverworld : KinematicBody2D
{
    GlobalCharacterData GlobalCharacterData;

    SignalManager signalManager;

    [Export]
    public List<BaseEnemy> myEnemies;
    [Export]
    public List<BasePlayerCharacter> party;
    [Export]
    public List<Resource> encounterDrops;
    [Export]
    public bool startBattleOnCollision;
    [Export]
    public bool canRunFromEncounter;
    [Export]
    public string afterEncounterCutscene;
    [Export]
    public bool testing = false;

    public override void _Ready()
    {
        GlobalCharacterData = GetNode<GlobalCharacterData>("/root/GlobalCharacterData");
        signalManager = GetNode<SignalManager>("/root/SignalManager");
        if (testing == true) PrepareBattle();
    }

    public void _on_Hurtbox_area_entered(Area2D area)
    {        
        if (testing == false) PrepareBattle(area);
    }

    public void PrepareBattle(Area2D area = null)
    {
        GlobalCharacterData.AddToEnemies(myEnemies);
        signalManager.EmitSignal("setPostEncounterCutscene", afterEncounterCutscene);

        if (party != null && party.Count > 0)
        {
            GlobalCharacterData.partyMembers = party;
        }
        else
        {
            if (area == null) return;
            PlayerController player = area.FindParent("Player").GetNode<PlayerController>(".");
            GlobalCharacterData.partyMembers = player.partyMembers;
        }

        GlobalCharacterData.encounterDrops = encounterDrops;

        if (startBattleOnCollision)
        {
            signalManager.EmitSignal("battleStarted");
            //QueueFree();
        }
    }
}
