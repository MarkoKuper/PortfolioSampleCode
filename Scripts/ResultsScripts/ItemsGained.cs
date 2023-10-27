using Godot;
using System;
using System.Collections.Generic;

public class ItemsGained : VBoxContainer
{
    SignalManager signalManager;

    PackedScene resultItemUI;

    VBoxContainer itemsContainer;

    GlobalCharacterData globalCharacterData;

    List<BaseEnemy> enemies;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        globalCharacterData = GetNode<GlobalCharacterData>("/root/GlobalCharacterData");
        itemsContainer = GetNode<VBoxContainer>("Items");
        signalManager = GetNode<SignalManager>("/root/SignalManager");

        resultItemUI = (PackedScene)ResourceLoader.Load("res://Marko/Prefab/ItemResultsUI.tscn");

        enemies = globalCharacterData.enemyData;

        EnemyDrops();
        EncounterDrops();
    }

    void EnemyDrops()
    {
        if (enemies == null || enemies.Count < 1) return;
        foreach (BaseEnemy enemy in enemies)
        {
            if (enemy == null) continue;
            if (enemy.itemDrops == null || enemy.itemDrops.Count < 1) continue;
            foreach (Resource item in enemy.itemDrops)
            {
                if (item == null) continue;
                Node itemUI = resultItemUI.Instance();

                ItemResultsUI script = itemUI.GetNode<ItemResultsUI>(".");

                RandomNumberGenerator randomNumber = new RandomNumberGenerator();
                randomNumber.Randomize();
                int chosenNumber = randomNumber.RandiRange(1, 100);
                float itemDropChance = (float)item.Get("drop_chance");
                if (chosenNumber < itemDropChance)
                {
                    script.SetUI(item.Get("item_icon") as Texture, item.Get("item_name") as string);

                    itemsContainer.AddChild(itemUI);
                    AddItem(item.Get("item_name") as string);
                }
            }
        }
    }

    void EncounterDrops()
    {
        if(globalCharacterData.encounterDrops != null)
        {
            foreach (Resource item in globalCharacterData.encounterDrops)
            {
                Node itemUI = resultItemUI.Instance();

                ItemResultsUI script = itemUI.GetNode<ItemResultsUI>(".");
              
                script.SetUI(item.Get("item_icon") as Texture, item.Get("item_name") as string);

                itemsContainer.AddChild(itemUI);
                AddItem(item.Get("item_name") as string);
            }
        }
    }

    void AddItem(string name)
    {
        GD.Print("Items Gained: Adding " + name);
        signalManager.EmitSignal("addItem", name);
    }
}
