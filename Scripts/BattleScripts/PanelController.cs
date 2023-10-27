using Godot;
using System.Collections.Generic;

public class PanelController : Control
{
    Panel actionPanel;

    public ItemManager itemManager { get; private set; }

    PlayerSelectionPanel playerSelectionPanel;

    AbilityManager abilityManager;

    Button backButton;

    SignalManager signalManager;

    GlobalCharacterData characterData;

    int myPartyMemberTurn;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        playerSelectionPanel = GetNode<PlayerSelectionPanel>("PlayerSelectionPanel");
        itemManager = GetNode<ItemManager>("ItemManager");
        actionPanel = GetNode<Panel>("ActionPanel");
        abilityManager = GetNode<AbilityManager>("TalentsPanel");
        signalManager = GetNode<SignalManager>("/root/SignalManager");
        characterData = GetNode<GlobalCharacterData>("/root/GlobalCharacterData");
        backButton = GetNode<Button>("BackButton");

        signalManager.Connect("toggleActionPanelVisibility", this, "ToggleActionPanelVisibility");

        if (characterData.canDip == false)
        {
            actionPanel.GetNode<Button>("HBoxContainer/Run").Disabled = true;
        }
    }
    
    public void ToggleActionPanelVisibility()
    {
        if (actionPanel.Visible)
        {
            actionPanel.Hide();
        }
        else
        {
            actionPanel.Show();
        }
    }

    public void SetActionPanelTextBox(string characterName)
    {
        actionPanel.GetNode<Label>("Textbox/Label").Text = characterName;
    }

    public void OnTalentsPressed(int partyMemberTurn)
    {
        Vector2 panelSize = new Vector2();
        myPartyMemberTurn = partyMemberTurn;
        ToggleActionsAvailability();
        abilityManager.Show();
        abilityManager.InitializePanel(partyMemberTurn);
        abilityManager.SetSize(new Vector2(abilityManager.RectSize.x + 20f, abilityManager.RectSize.y + 40f));
        Vector2 abilityPanelPosition = new Vector2(-abilityManager.RectSize.x, actionPanel.RectSize.y - abilityManager.RectSize.y);
        abilityManager.SetPosition(actionPanel.RectGlobalPosition + abilityPanelPosition);
        panelSize.x = abilityManager.RectSize.x;
        backButton.RectPosition = abilityManager.RectPosition + panelSize;
        backButton.Show();
    }

    public async void OnAbilitySelected(List<BattleInfo> characterBattleInfo, int partyMemberTurn)
    {
        BattleInfo currentCharacter = new BattleInfo();
        foreach (BattleInfo character in characterBattleInfo)
        {
            if(character.characterType == BattleInfo.characterTypes.player && character.characterIndex == partyMemberTurn)
            {
                currentCharacter = character;
            }
        }
        BaseAbilities selectedAbility = GetSelectedAbility(currentCharacter);
        abilityManager.ToggleButtonAvailability(partyMemberTurn);
        if (selectedAbility.needsTarget && !selectedAbility.targetsEnemy)
        {
            Vector2 panelSize =new Vector2();
            playerSelectionPanel.Show();
            playerSelectionPanel.SetSize(new Vector2(0, 0));
            playerSelectionPanel.SetSize(new Vector2(playerSelectionPanel.RectSize.x + 20f, playerSelectionPanel.RectSize.y + 40f));
            Vector2 playerPanelPosition = new Vector2(-playerSelectionPanel.RectSize.x, abilityManager.RectSize.y - playerSelectionPanel.RectSize.y);
            playerSelectionPanel.SetPosition(abilityManager.RectGlobalPosition + playerPanelPosition);
            ToggleActionsAvailability();
            playerSelectionPanel.ToggleButtonAvailablility();
            panelSize.x = playerSelectionPanel.RectSize.x;
            backButton.RectGlobalPosition = playerSelectionPanel.RectGlobalPosition + panelSize;
        }
        else if (selectedAbility.needsTarget && selectedAbility.targetsEnemy)
        {
            abilityManager.Hide();
            abilityManager.ToggleButtonAvailability(partyMemberTurn);
            backButton.Hide();
            signalManager.EmitSignal("displayText", "Select a member of the audience!");
            await ToSignal(signalManager, "textBoxClosed");
            ToggleEnemySelection(characterBattleInfo);
            await ToSignal(signalManager, "targetSelected");
            await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
        }
        else
        {
            abilityManager.Hide();
            backButton.Hide();
            signalManager.EmitSignal("actionSelected");
        }

        ToggleActionsAvailability();
    }

    BaseAbilities GetSelectedAbility(BattleInfo currentCharacterBattleInfo)
    {
        foreach (BaseAbilities ability in characterData.partyMembers[currentCharacterBattleInfo.characterIndex].abilities)
        {
            if (ability.abilityName == currentCharacterBattleInfo.abilityToUse)
            {
                return ability;
            }
        }
        return null;
    }

    public void OnItemsPressed(List<BattleInfo> characterBattleInfo, int partyMemberTurn)
    {
        Vector2 panelSize = new Vector2();
        myPartyMemberTurn= partyMemberTurn;
        ToggleActionsAvailability();
        for (int a = 0; a < characterBattleInfo.Count; a++)
        {
            if (characterBattleInfo[a].characterType == BattleInfo.characterTypes.player && characterBattleInfo[a].characterIndex == partyMemberTurn)
            {
                characterBattleInfo[a].actionChosen = BattleInfo.actions.useItem;
            }
        }
        itemManager.Show();
        Vector2 itemManagerPosition = new Vector2(-itemManager.RectSize.x, actionPanel.RectSize.y - itemManager.RectSize.y);
        itemManager.SetPosition(actionPanel.RectGlobalPosition + itemManagerPosition);
        panelSize.x = itemManager.RectSize.x;
        backButton.Show();
        backButton.RectGlobalPosition = itemManager.RectGlobalPosition + panelSize;
    }

    public void OnItemSelected()
    {
        Vector2 panelSize = new Vector2();
        itemManager.ToggleItemsAvailability();
        playerSelectionPanel.Show();
        Vector2 playerPanelPosition = new Vector2(-playerSelectionPanel.RectSize.x, itemManager.RectSize.y - playerSelectionPanel.RectSize.y);
        playerSelectionPanel.SetPosition(itemManager.RectGlobalPosition + playerPanelPosition);
        playerSelectionPanel.ToggleButtonAvailablility();
        panelSize.x = playerSelectionPanel.RectSize.x;
        backButton.RectGlobalPosition = playerSelectionPanel.RectGlobalPosition + panelSize;
    }

    public void OnCharacterSelected(BattleInfo currentCharacter)
    {
        playerSelectionPanel.Hide();
        backButton.Hide();
        ToggleActionsAvailability();
        if (currentCharacter.actionChosen == BattleInfo.actions.useAbility)
        {
            abilityManager.Hide();
            abilityManager.ToggleButtonAvailability(currentCharacter.characterIndex);
        }
        else if (currentCharacter.actionChosen == BattleInfo.actions.useItem)
        {
            itemManager.Hide();
            itemManager.ReduceItemAmountOnButton();
            itemManager.ToggleItemsAvailability();
        }
    }

    void _on_BackButton_pressed()
    {
        signalManager.EmitSignal("PlaySound", "ButtonSound1", null);
        Vector2 panelSize = new Vector2();
        if (playerSelectionPanel.Visible)
        {
            playerSelectionPanel.Hide();
            if (abilityManager.Visible)
            {
                abilityManager.ToggleButtonAvailability(myPartyMemberTurn);
                panelSize.x = abilityManager.RectSize.x;
                backButton.RectPosition = abilityManager.RectPosition + panelSize;
            }
            else if (itemManager.Visible)
            {
                itemManager.ToggleItemsAvailability();
                panelSize.x = playerSelectionPanel.RectSize.x;
                backButton.RectGlobalPosition = playerSelectionPanel.RectGlobalPosition + panelSize;
            }
        }
        else if (abilityManager.Visible)
        {
            abilityManager.Hide();
            ToggleActionsAvailability();
            abilityManager.ToggleButtonAvailability(myPartyMemberTurn);
            backButton.Hide();
        }
        else if (itemManager.Visible)
        {
            itemManager.Hide();
            ToggleActionsAvailability();
            backButton.Hide();
        }
    }

    void ToggleActionsAvailability()
    {
        Control container = actionPanel.GetNode<VBoxContainer>("HBoxContainer");
        for (int i = 0; i < container.GetChildCount(); i++)
        {
            if(container.GetChild<Button>(i).Name == "Run")
            {
                container.GetChild<Button>(i).Disabled = true;
            }
            else
            {
                container.GetChild<Button>(i).Disabled = !container.GetChild<Button>(i).Disabled;
            }
        }
    }    

    public void ToggleEnemySelection(List<BattleInfo> characterBattleInfo)
    {
        CollisionShape2D enemyCollssionShape;

        List<BattleInfo> enemies = GetEnemies(characterBattleInfo);

        ToggleActionPanelVisibility();

        for (int i = 0; i < enemies.Count; i++)
        {
            enemyCollssionShape = enemies[i].UI.GetNode<CollisionShape2D>("EnemyUI/ArrowPlacer/Area2D/CollisionShape2D");
            if (enemies[i].currentStatAmounts["currentEntertainment"] >= characterData.enemyData[i].baseStats["maxEntertainment"])
            {
                enemyCollssionShape.Disabled = true;
            }
            else
            {
                enemyCollssionShape.Disabled = !enemyCollssionShape.Disabled;
            }
        }
    }

    List<BattleInfo> GetEnemies(List<BattleInfo> characterBattleInfo)
    {
        List<BattleInfo> enemies = new List<BattleInfo>();
        foreach (BattleInfo character in characterBattleInfo)
        {
            if(character.characterType == BattleInfo.characterTypes.enemy)
            {
                enemies.Add(character);
            }
        }
        return enemies;
    }
}
