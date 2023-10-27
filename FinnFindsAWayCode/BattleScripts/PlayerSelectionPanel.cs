using Godot;
using System;
using System.Collections.Generic;

public class PlayerSelectionPanel : PanelContainer
{
    VBoxContainer buttonContainer;

    GlobalCharacterData globalCharacterData;

    PackedScene baseButton;

    List<Button> buttonsInPanel = new List<Button>();

    string characterName;
    public override void _Ready()
    {
        globalCharacterData = GetNode<GlobalCharacterData>("/root/GlobalCharacterData");
        baseButton = (PackedScene)ResourceLoader.Load("res://Marko/Prefab/ButtonBase.tscn");
        buttonContainer = GetNode<VBoxContainer>("PlayerSelectionContainer");

        InitializePanel();
    }

    public void InitializePanel()
    {
        for (int i = 0; i < globalCharacterData.partyMembers.Count; i++)
        {           
            Button newButton = baseButton.Instance<Button>();
            characterName = globalCharacterData.partyMembers[i].name;
            newButton.Name = characterName;
            newButton.Text = characterName;
            buttonContainer.AddChild(newButton);
            buttonsInPanel.Add(newButton);
        }
    }

    public void ToggleButtonAvailablility()
    {
        for (int i = 0; i < buttonsInPanel.Count; i++)
        {
            if (globalCharacterData.partyMembers[i].currentStatAmounts["currentEnergy"] <= 0)
            {
                buttonsInPanel[i].Disabled = true;
            }
        }
    }
}
