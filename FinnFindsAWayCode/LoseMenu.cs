using Godot;
using System;

public class LoseMenu : Panel
{
    GDScript saveScript;

    SignalManager signalManager;

    [Export]
    string sceneToLoad;
    [Export]
    string mainMenu;

    public override void _Ready()
    {
        saveScript = (GDScript)GD.Load("res://Nathaniel/Scripts/Misc/SaveData.gd");

        signalManager = GetNode<SignalManager>("/root/SignalManager");
    }

    void _on_WakeUp_pressed()
    {
        GD.Print("WakeUp Pressed");

        signalManager.EmitSignal("PlaySound", "ButtonSound1", null);

        BasePlayerCharacter finnData = GD.Load("res://Marko/Scripts/PlayerCharacterSOs/Finn.tres") as BasePlayerCharacter;

        BasePlayerCharacter finnResetData = GD.Load("res://Marko/Scripts/PlayerCharacterSOs/FinnReset.tres") as BasePlayerCharacter;

        finnData.ResetResource(finnResetData);

        Node saveScriptNode = (Node)saveScript.New();
        saveScriptNode.Call("ClearInventoryData", "PlayerInventroyTest.txt");

        GetTree().ChangeScene(sceneToLoad);
    }

    void _on_Exit_pressed()
    {
        signalManager.EmitSignal("PlaySound", "ButtonSound1", null);
        GD.Print("Exit Pressed");
        GetTree().ChangeScene(mainMenu);
    }
}
