using Godot;
using System;

public class BaseButton : Button
{
    SignalManager signalManager;
    public override void _Ready()
    {
        signalManager = GetNode<SignalManager>("/root/SignalManager");
        Connect("pressed", this, "ButtonPressed");
    }

    void ButtonPressed()
    {
        signalManager.EmitSignal("PlaySound", "ButtonSound1", null);
        if (GetParent().Name == "AbilitiesContainer")
        {
            signalManager.EmitSignal("selectThisAbility", Name);
        }
        else if(GetParent().Name == "PlayerSelectionContainer")
        {
            signalManager.EmitSignal("characterSelected", Name);
        }
        else if(GetParent().Name == Name + "UI")
        {
            signalManager.EmitSignal("selectThisItem", Name);
        }
    }
}
