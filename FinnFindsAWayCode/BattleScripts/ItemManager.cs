using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;


public class ItemManager : Control
{
    GlobalCharacterData globalCharacterData;

    SignalManager signalManager;

    Godot.Collections.Array inventoryList;

    VBoxContainer buttonContainer;

    PackedScene itemUI;

    ScrollContainer itemScrollContainer;

    List<Button> buttonsInPanel = new List<Button>();
    List<Label> itemAmountLabel = new List<Label>();
    List<int> itemAmount = new List<int>();

    string lastItemButtonPressed;

    [Export]
    public string inventorySavePath;

    public override void _Ready()
    {
        itemUI = (PackedScene)ResourceLoader.Load("res://Marko/Prefab/ItemButton.tscn");
        buttonContainer = GetNode<VBoxContainer>("ScrollContainer/VBoxContainer");
        itemScrollContainer = GetNode<ScrollContainer>("ScrollContainer");
        signalManager = GetNode<SignalManager>("/root/SignalManager");
        globalCharacterData = GetNode<GlobalCharacterData>("/root/GlobalCharacterData");
        LoadInventory();
        InitializePanel();

        signalManager.Connect("selectThisItem", this, "ButtonPressed");
    }

    public void InitializePanel()
    {
        foreach (Dictionary item in inventoryList)
        {
            if (item["Category"].ToString() == "Consumable")
            {
                HBoxContainer newItemUI = itemUI.Instance<HBoxContainer>();
                Button newButton = newItemUI.GetNode<Button>("Button");
                Label newUIText = newItemUI.GetNode<Label>("Label");
                newItemUI.Name = item["Name"].ToString() + "UI";
                newButton.Name = item["Name"].ToString();
                newButton.Text = item["Name"].ToString();
                newUIText.Text = item["Quantity"].ToString();
                buttonContainer.AddChild(newItemUI);
                buttonsInPanel.Add(newButton);
                itemAmountLabel.Add(newUIText);
                itemAmount.Add(Convert.ToInt32(item["Quantity"]));
                if (RectSize.x < newItemUI.RectSize.x + 40f)
                {
                    SetSize(new Vector2(newItemUI.RectSize.x + 40f, RectSize.y));
                }
            }
        }
    }

    void UpdatePanel()
    {
        inventoryList.Clear();
        LoadInventory();
        foreach (HBoxContainer items in buttonContainer.GetChildren())
        {
            items.QueueFree();
        }
        buttonsInPanel.Clear();
        InitializePanel();
    }

    void LoadInventory()
    {
        Godot.File file = new Godot.File();
        file.Open(inventorySavePath, Godot.File.ModeFlags.Read);
        string text = file.GetAsText();
        var jsonFile = JSON.Parse(text).Result;
        var completeInventory = jsonFile as Godot.Collections.Array;
        inventoryList = (Godot.Collections.Array)completeInventory[0];
        file.Close();
    }

    public async void UseItem(BattleInfo character)
    {
        GD.Print(character.itemToUse + " On " + character.targetIndex);

        Dictionary item = new Dictionary();
        foreach (Dictionary inventoryItem in inventoryList)
        {
            if (inventoryItem["Name"].ToString() == character.itemToUse)
            {
                item = inventoryItem;
            }
        }
        signalManager.EmitSignal("useItem", character.itemToUse, character.targetIndex);
        await ToSignal(GetTree().CreateTimer(0.01f), "timeout");
        signalManager.EmitSignal("removeItem", item["Name"], 1) ;
        BasePlayerCharacter targetCharacter = globalCharacterData.partyMembers[character.targetIndex];
        character.uiScript.SetHealth(targetCharacter.currentStatAmounts["currentEnergy"], targetCharacter.baseStats["maxEnergy"]);
        character.uiScript.SetCreativity(targetCharacter.currentStatAmounts["currentCreativity"], targetCharacter.baseStats["maxCreativity"]);
        UpdatePanel();
        signalManager.EmitSignal("actionsCarriedOut");
    }

    void ButtonPressed(string name)
    {
        for (int i = 0; i < buttonsInPanel.Count; i++)
        {
            if (name == buttonsInPanel[i].Text && itemAmount[i] > 0)
            {
                lastItemButtonPressed = name;
                signalManager.EmitSignal("itemSelected", name);
            }
        }
    }

    public void ReduceItemAmountOnButton()
    {
        for (int i = 0; i < buttonsInPanel.Count; i++)
        {
            if (lastItemButtonPressed == buttonsInPanel[i].Text)
            {
                itemAmount[i]--;
                itemAmountLabel[i].Text = itemAmount[i].ToString();
            }
        }
    }

    public void ToggleItemsAvailability()
    {
        foreach (Button button in buttonsInPanel)
        {
            button.Disabled = !button.Disabled;
        }
    }
}
