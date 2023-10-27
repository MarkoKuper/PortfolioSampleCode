using Godot;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static BattleInfo;

public class Battle : Control
{
    public enum BattleOutcome { won, lost, run };

    AbilityManager abilityManager;

    EnemyBattleController enemyBattleController;

    BattleDialogueManager battleDialogueManager;

    PanelController panelController;

    SignalManager signalManager;

    EncounterInitiator encounterInitiator;

    EncounterTextBox textBox;
    EncounterTextBox audienceTextBox;

    TextureRect playerArrow;

    GlobalCharacterData characterData;

    Control enemySpawnPlacer;
    Control partyMemberContainer;

    PackedScene enemyBattleUI;
    PackedScene playerBattleUI;

    List<Control> enemyUIs = new List<Control>();

    List<BattleInfo> characterBattleInfo = new List<BattleInfo>();
    List<BattleInfo> enemies = new List<BattleInfo>();

    int partyMemberTurn;

    bool showedEnergyQuip;
    bool showedEntertinmentQuip;

    public List<BaseEnemy> enemyData;
    public List<BasePlayerCharacter> partyMembers;

    public override async void _Ready()
    {
        playerArrow = GetNode<TextureRect>("UILayer/CurrentPlayerArrow");
        characterData = GetNode<GlobalCharacterData>("/root/GlobalCharacterData");
        enemySpawnPlacer = GetNode<Control>("UILayer/EnemySpawnLocations");
        partyMemberContainer = GetNode<Control>("UILayer/PartyMemberUIContainer");
        panelController = GetNode<PanelController>("UILayer/PanelController");
        textBox = GetNode<EncounterTextBox>("UILayer/Textbox");
        audienceTextBox = GetNode<EncounterTextBox>("UILayer/AudienceTextbox");
        abilityManager = panelController.GetNode<AbilityManager>("TalentsPanel");
        enemyBattleController = GetNode<EnemyBattleController>("UILayer/EnemyBattleController");
        battleDialogueManager = GetNode<BattleDialogueManager>("UILayer/Textbox/BattleDialogueManager");
        signalManager = GetNode<SignalManager>("/root/SignalManager");
        encounterInitiator = GetParent() as EncounterInitiator;

        enemyBattleUI = (PackedScene)ResourceLoader.Load("res://Marko/Prefab/NewAudienceUI.tscn");
        playerBattleUI = (PackedScene)ResourceLoader.Load("res://Marko/Prefab/PlayerUI.tscn");

        enemyData = characterData.enemyData;
        partyMembers = characterData.partyMembers;

        InitializeEnemies();
        InitializePartyMembers();
        SortBySpeed();

        signalManager.Connect("selectThisEnemy", this, "SelectEnemy");
        signalManager.Connect("abilitySelected", this, "AbilitySelected");
        signalManager.Connect("characterSelected", this, "CharacterSelected");
        signalManager.Connect("useAbility", abilityManager, "UseAbility");
        signalManager.Connect("checkAffinities", this, "CheckAffinities");
        signalManager.Connect("itemSelected", this, "ItemSelected");

        if (enemyData.Count > 1)
        {
            textBox.DisplayText("A group of audience members has appeared!");
        }
        else
        {
            textBox.DisplayText(enemyData[0].name + " has appeared!");
        }

        await ToSignal(signalManager, "textBoxClosed");
        if (enemyData[0].name == "Slimette")
        {
            Tutorial();
        }
        else
        {
            PlayOutBattle();
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (Input.IsActionJustPressed("LeftMouseClick") || Input.IsActionJustPressed("Space")) //Hides the text box if it visible when the left mouse button is clicked
        {
            if(textBox.Visible && textBox.textFinished || audienceTextBox.Visible && audienceTextBox.textFinished)
            {
                textBox.Hide();
                audienceTextBox.Hide();
                panelController.ToggleActionPanelVisibility();
                signalManager.EmitSignal("textBoxClosed");
            }
            else if(textBox.Visible && !textBox.textFinished)
            {
                textBox.SpeedUpText();
            }
            else if(audienceTextBox.Visible && !audienceTextBox.textFinished)
            {
                audienceTextBox.SpeedUpText();
            }
        }
    }

    //Instances battle UI for each enemy.
    //Grabs each enemy's stats from the GameData script and creates a new BattleInfo class for each enemy.
    //It adds each of the relevant stats to the variables in the battle info class.
    //Finally it sets the enemy's UI to match its current and max entertainment.
    void InitializeEnemies()
    {
        for (int i = 0; i < enemyData.Count; i++)
        {
            Control newBattleUI = enemyBattleUI.Instance() as Control;
            newBattleUI.Name = "EnemyBattleUI" + i;
            enemySpawnPlacer.AddChild(newBattleUI);
            enemyUIs.Add(newBattleUI);
            BattleInfo newEnemyBattleInfo = new BattleInfo();
            newEnemyBattleInfo.UI = newBattleUI;
            if (enemyData[i].hasShadow)
            {
                newEnemyBattleInfo.UI.GetNode<TextureRect>("Shadow").Visible = true;
            }
            newEnemyBattleInfo.currentStatAmounts["currentEntertainment"] = 0;
            newEnemyBattleInfo.currentStatAmounts["currentCreativity"] = enemyData[i].baseStats["maxCreativity"];
            newEnemyBattleInfo.currentStatAmounts["criticality"] = enemyData[i].baseStats["baseCriticality"];
            newEnemyBattleInfo.currentStatAmounts["insensitivity"] = enemyData[i].baseStats["baseInsensitivity"];
            newEnemyBattleInfo.currentStatAmounts["characterSpeed"] = enemyData[i].baseStats["baseFocus"];
            newEnemyBattleInfo.characterIndex = i;
            newEnemyBattleInfo.characterType = BattleInfo.characterTypes.enemy;
            characterBattleInfo.Add(newEnemyBattleInfo);
            enemies.Add(newEnemyBattleInfo);
            newEnemyBattleInfo.uiScript = newBattleUI.GetNode<CharacterUI>("EnemyUI");
            newEnemyBattleInfo.uiScript.SetHealth(newEnemyBattleInfo.currentStatAmounts["currentEntertainment"], enemyData[i].baseStats["maxEntertainment"]);
            newEnemyBattleInfo.uiScript.SetCharacterName(enemyData[i].name);
            if (enemyData[i].hasAnimation)
            {
                newEnemyBattleInfo.uiScript.PlayIdle();
            }
            else
            {
                newEnemyBattleInfo.uiScript.SetEnemySprite(enemyData[i].texture);
            }
        }
    }

    //Instances battle UI for each party member.
    //Grabs each party members stats from the GameData script and creates a new BattleInfo class for each part member.
    //It adds each of the relevant stats to the variables in the battle info class.
    //Finally it sets the party member's UI to match its current and max entertainment, and it's current and max creativity.
    void InitializePartyMembers()
    {
        for (int i = 0; i < partyMembers.Count; i++)
        {
            Node newPlayerBattleUI = playerBattleUI.Instance();
            newPlayerBattleUI.Name = "PlayerBattleUI" + i;
            partyMemberContainer.AddChild(newPlayerBattleUI);
            BattleInfo newCharacterBattleInfo = new BattleInfo();
            newCharacterBattleInfo.UI = newPlayerBattleUI;
            newCharacterBattleInfo.currentStatAmounts["characterSpeed"] = partyMembers[i].baseStats["baseFocus"];
            newCharacterBattleInfo.characterIndex = i;
            newCharacterBattleInfo.characterType = BattleInfo.characterTypes.player;
            characterBattleInfo.Add(newCharacterBattleInfo);
            newCharacterBattleInfo.uiScript = newPlayerBattleUI.GetNode<CharacterUI>(".");
            newCharacterBattleInfo.uiScript.SetHealth(partyMembers[i].baseStats["maxEnergy"], partyMembers[i].baseStats["maxEnergy"]);
            newCharacterBattleInfo.uiScript.SetCreativity(partyMembers[i].baseStats["maxCreativity"], partyMembers[i].baseStats["maxCreativity"]);
            newCharacterBattleInfo.uiScript.SetCharacterName(partyMembers[i].name);
            newPlayerBattleUI.GetNode<TextureRect>("Panel/PlayerSprite").Texture = partyMembers[i].texture;
        }
    }

    async void Tutorial()
    {
        SortBySpeed();
        if (characterBattleInfo[0].currentRound == 0)
        {
            textBox.DisplayText("A performance plays out in turns. You're allowed to perform one action every turn.");
            await ToSignal(signalManager, "textBoxClosed");
            textBox.DisplayText("You can use your mouse to choose actions in a performance. 'Perform' is the most basic way to entertain an Audience.");
            await ToSignal(signalManager, "textBoxClosed");
            textBox.DisplayText("Finn, their friends, and their Audiences get turns in a performance where they can act. Characters' turn orders are decided by their 'Focus'. Character's with higher 'Focus' will carry out their actions first.");
            await ToSignal(signalManager, "textBoxClosed");
        }
        else if(characterBattleInfo[0].currentRound <= 3)
        {
            textBox.DisplayText(battleDialogueManager.TutorialText(characterBattleInfo[0].currentRound));
            await ToSignal(signalManager, "textBoxClosed");
        }
        if(characterBattleInfo[0].currentRound == 2)
        {
            audienceTextBox.DisplayText("Please keep at it!");
            await ToSignal(signalManager, "textBoxClosed");
        }
        if (partyMembers[0].currentStatAmounts["currentEnergy"] <= partyMembers[0].baseStats["maxEnergy"] * 0.5f && !showedEnergyQuip)
        {
            showedEnergyQuip = true;
            audienceTextBox.DisplayText("H-hey there, you're starting to look tired. Maybe you need to eat something or rest!");
            await ToSignal(signalManager, "textBoxClosed");
        }
        PlayOutBattle();
    }


    //Plays out the battle and will continue looping until the battle is finished
    async void PlayOutBattle()
    {
        SortBySpeed();
        ChooseAction();
        await ToSignal(signalManager, "playerTurnEnded");
        for (int i = 0; i < characterBattleInfo.Count; i++)
        {
            BattleInfo currentCharacter = characterBattleInfo[i];
            signalManager.EmitSignal("roundStarted");
            currentCharacter.currentRound++;
            BattleInfo.characterTypes type = currentCharacter.characterType;
            BattleInfo.actions action = currentCharacter.actionChosen;
            //Checks to see if enemy is stunned. If its stunned, it loses its turn to use an ability, if not it uses an ability.
            if (type == BattleInfo.characterTypes.enemy && enemies[currentCharacter.characterIndex].currentStatAmounts["currentEntertainment"] < enemyData[currentCharacter.characterIndex].baseStats["maxEntertainment"])
            {
                if(enemyData[currentCharacter.characterIndex].name == "Slimette" && currentCharacter.currentRound <= 3)
                {
                    if(currentCharacter.currentRound <= 3)
                    {
                        audienceTextBox.DisplayText(battleDialogueManager.SlimetteTutorialText(currentCharacter.currentRound));
                        await ToSignal(signalManager, "textBoxClosed");
                    }
                    else if(currentCharacter.currentStatAmounts["currentEntertainment"] <= enemyData[currentCharacter.characterIndex].baseStats["maxEntertainment"] * 0.5f && currentCharacter.currentRound >= 5 && !showedEntertinmentQuip)
                    {
                        audienceTextBox.DisplayText("Don't worry when you're on stage - keep a level head and 'Perform'!");
                        await ToSignal(signalManager, "textBoxClosed");
                        showedEntertinmentQuip = true;
                    }
                }
                if (currentCharacter.stunned)
                {
                    audienceTextBox.DisplayText(enemyData[currentCharacter.characterIndex].name + " is stunned and can't use an ability!");
                    await ToSignal(signalManager, "textBoxClosed");
                    currentCharacter.stunned = false;
                }
                else
                {
                    for (int a = 0; a < partyMembers.Count; a++)
                    {
                        if (partyMembers[a].currentStatAmounts["currentEnergy"] > 0)
                        {
                            audienceTextBox.DisplayText(enemyData[currentCharacter.characterIndex].name + " uses an ability!");
                            await ToSignal(signalManager, "textBoxClosed");
                            enemyBattleController.EnemyTakesTurn(characterBattleInfo, currentCharacter.characterIndex);
                            await ToSignal(signalManager, "actionsCarriedOut");
                            break;
                        }
                    }
                }
            }
            //If its a player, the player uses its selected action
            else if (type == BattleInfo.characterTypes.player && partyMembers[currentCharacter.characterIndex].currentStatAmounts["currentEnergy"] > 0)
            {
                for (int a = 0; a < enemyData.Count; a++)
                {
                    if (enemies[a].currentStatAmounts["currentEntertainment"] < enemyData[a].baseStats["maxEntertainment"])
                    {
                        if (action == BattleInfo.actions.attack)
                        {
                            PlayerPerformances(currentCharacter);
                            await ToSignal(signalManager, "actionsCarriedOut");
                        }
                        else if (action == BattleInfo.actions.useAbility)
                        {
                            BaseAbilities abilityToUse = new BaseAbilities();
                            foreach (var ability in partyMembers[currentCharacter.characterIndex].abilities)
                            {
                                if(currentCharacter.abilityToUse == ability.abilityName)
                                {
                                    abilityToUse = ability;
                                }
                            }
                            abilityManager.UseAbility(abilityToUse, currentCharacter, characterBattleInfo);
                            await ToSignal(signalManager, "actionsCarriedOut");
                        }
                        else if (action == BattleInfo.actions.useItem)
                        {
                            textBox.DisplayText(partyMembers[currentCharacter.characterIndex].name + " used " + currentCharacter.itemToUse + "!");
                            await ToSignal(signalManager, "textBoxClosed");
                            panelController.itemManager.UseItem(currentCharacter);
                            await ToSignal(signalManager, "actionsCarriedOut");
                        }
                        break;
                    }
                }
            }
        }
        //Enemies use their critiques based on the party's performances
        for (int i = 0; i < enemies.Count; i++)
        {
            for (int a = 0; a < partyMembers.Count; a++)
            {
                if (partyMembers[a].currentStatAmounts["currentEnergy"] > 0 && enemyData[i].name != "Finn" && enemies[i].currentStatAmounts["currentEntertainment"] < enemyData[i].baseStats["maxEntertainment"])
                {
                    enemyBattleController.EnemyCritique(characterBattleInfo, enemies[i].characterIndex);
                    await ToSignal(signalManager, "actionsCarriedOut");
                    enemies[i].entertainedThisRound = false;
                    enemies[i].likedPerformance = false;
                    enemies[i].dislikedPerformance = false;
                    break;
                }
            }
        }
        for (int a = 0; a < enemyData.Count; a++)
        {
            if (enemies[a].currentStatAmounts["currentEntertainment"] < enemyData[a].baseStats["maxEntertainment"])
            {
                break;
            }
            else if (a == (enemyData.Count - 1))
            { 
                signalManager.EmitSignal("displayText", "Everyone has been entertained!");
                abilityManager.encounterOver = true;
                signalManager.EmitSignal("roundStarted");
                await ToSignal(signalManager, "textBoxClosed");
                signalManager.EmitSignal("StopSound"); // PART OF NATE NOTE: TRY MAKING IT ACTUALLY STOP
                characterData.AssignBattleResult(BattleOutcome.won, null);
                encounterInitiator.LoadResultScene();
            }
        }
        for (int a = 0; a < partyMembers.Count; a++)
        {
            if (partyMembers[a].currentStatAmounts["currentEnergy"] > 0)
            {
                break;
            }
            else if (a == (partyMembers.Count - 1))
            {
                foreach (var member in partyMembers)
                {
                    member.ResetHealth();
                }
                signalManager.EmitSignal("displayText", "Your party has run out of energy!");
                abilityManager.encounterOver = true;
                signalManager.EmitSignal("roundStarted");
                await ToSignal(signalManager, "textBoxClosed");
                characterData.AssignBattleResult(BattleOutcome.lost, null);
                encounterInitiator.LoadResultScene();
            }
        }
        if (enemyData[0].name != "Slimette")
        {
            PlayOutBattle();
        }
        else
        {
            Tutorial();
        }
    }

    //Each party member selects what actions they want to use for this round of performances
    async void ChooseAction()
    {
        for (int i = 0; i < partyMembers.Count; i++)
        {
            if (partyMembers[i].currentStatAmounts["currentEnergy"] > 0)
            {
                playerArrow.Show();
                Control characterUI = GetNode<Control>("UILayer/PartyMemberUIContainer/PlayerBattleUI" + i);
                Vector2 arrowPosition = characterUI.RectGlobalPosition + new Vector2(characterUI.RectSize.x * 1.4f, (characterUI.RectSize.y * 0.5f) + (playerArrow.RectSize.y * 0.5f));
                playerArrow.RectGlobalPosition = arrowPosition;
                panelController.SetActionPanelTextBox(partyMembers[i].name);
                partyMemberTurn = i;
                textBox.DisplayText("Select an action for " + partyMembers[i].name + ".");
                await ToSignal(signalManager, "textBoxClosed");
                await ToSignal(signalManager, "actionSelected");
            }
        }
        playerArrow.Hide();
        signalManager.EmitSignal("playerTurnEnded");
    }

    //Entertains the target audience member based on the player's style
    async void PlayerPerformances(BattleInfo character)
    {
        int characterIndex = character.characterIndex;
        int targetIndex = character.targetIndex;
        BattleInfo targetEnemy = enemies[targetIndex];
        float damageToBeDone = Mathf.Floor(partyMembers[characterIndex].currentStatAmounts["currentStyle"] - (targetEnemy.currentStatAmounts["currentInsensitivity"] * 0.3f));
        if(targetEnemy.actionChosen == BattleInfo.actions.defend)
        {
            damageToBeDone = Mathf.Floor(damageToBeDone * 0.75f);
        }
        else if(targetEnemy.slimeDefense == true)
        {
            damageToBeDone = Mathf.Floor(damageToBeDone * 0.5f);
        }
        textBox.DisplayCharacterDialogue(partyMembers[characterIndex].name, "Performance", enemyData[targetIndex].name, 0, targetEnemy.likedPerformance, targetEnemy.dislikedPerformance);
        await ToSignal(signalManager, "textBoxClosed");

        CheckAffinities(character, null);
        targetEnemy.entertainedThisRound = true;
        targetEnemy.targetIndex = characterIndex;
        targetEnemy.DamageEnemy(damageToBeDone, enemyData[targetIndex].baseStats["maxEntertainment"], enemyData[targetIndex].name);
        targetEnemy.uiScript.SetHealth(targetEnemy.currentStatAmounts["currentEntertainment"], enemyData[targetIndex].baseStats["maxEntertainment"]);
        if(targetEnemy.currentStatAmounts["currentEntertainment"] >= enemyData[targetIndex].baseStats["maxEntertainment"])
        {
            if (enemyData[targetIndex].name == "Slimette") signalManager.EmitSignal("PlaySound", "SlimetteGiggleLoop", null); // NATE NOTE: Working on seeing if their is a better place for this
            audienceTextBox.DisplayText(enemyData[targetIndex].name + " is completely entertained!");
            await ToSignal(signalManager, "textBoxClosed");
        }
        signalManager.EmitSignal("actionsCarriedOut");
    }

    //Lets the player run from the battle
    async void _on_Run_pressed()
    {
        signalManager.EmitSignal("PlaySound", "SFX_buttonClick", null);
        textBox.DisplayText("You have run away!");
        await ToSignal(signalManager, "textBoxClosed");
        characterData.AssignBattleResult(BattleOutcome.run, null);
        encounterInitiator.LoadResultScene();
    }

    //Allows the player to select a target to entertain this round
    async void _on_Attack_pressed()
    {
        signalManager.EmitSignal("PlaySound", "SFX_buttonClick", null);
        for (int a = 0; a < characterBattleInfo.Count; a++)
        {
            if (characterBattleInfo[a].characterType == BattleInfo.characterTypes.player && characterBattleInfo[a].characterIndex == partyMemberTurn)
            {
                characterBattleInfo[a].actionChosen = BattleInfo.actions.attack;
            }
        }
        if (enemies.Count > 1)
        {
            textBox.DisplayText("Select a member of the audience!");
            await ToSignal(signalManager, "textBoxClosed");
            panelController.ToggleEnemySelection(characterBattleInfo);
            await ToSignal(signalManager, "targetSelected");
            await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
    }
        else
        {
            for (int a = 0; a<characterBattleInfo.Count; a++)
            {
                if (characterBattleInfo[a].characterType == BattleInfo.characterTypes.player && characterBattleInfo[a].characterIndex == partyMemberTurn)
                {
                    characterBattleInfo[a].targetIndex = 0;
                }
            }
            signalManager.EmitSignal("actionSelected");
        }
    }

    //Allows the player to defend incoming critiques for a round
    async void _on_Defend_pressed()
    {
        signalManager.EmitSignal("PlaySound", "SFX_buttonClick", null);
        for (int i = 0; i < characterBattleInfo.Count; i++)
        {
            if (characterBattleInfo[i].characterType == BattleInfo.characterTypes.player && characterBattleInfo[i].characterIndex == partyMemberTurn)
            {
                characterBattleInfo[i].actionChosen = BattleInfo.actions.defend;
            }
        }
        textBox.DisplayCharacterDialogue(partyMembers[partyMemberTurn].name, "Rest", null, 0, false, false);
        await ToSignal(signalManager, "textBoxClosed");
        signalManager.EmitSignal("actionSelected");
    }

    //Allows the player to select the current party member's abilties if they have enough creativity
    void _on_Talents_pressed()
    {
        signalManager.EmitSignal("PlaySound", "SFX_buttonClick", null);
        for (int a = 0; a < characterBattleInfo.Count; a++)
        {
            if (characterBattleInfo[a].characterType == BattleInfo.characterTypes.player && characterBattleInfo[a].characterIndex == partyMemberTurn)
            {
                characterBattleInfo[a].actionChosen = BattleInfo.actions.useAbility;
            }
        }
        panelController.OnTalentsPressed(partyMemberTurn);
    }

    void _on_UseItem_pressed()
    {
        signalManager.EmitSignal("PlaySound", "SFX_buttonClick", null);
        panelController.OnItemsPressed(characterBattleInfo, partyMemberTurn);
    }

    //Allows the player to select one of their party member's as a target for an ability
    void CharacterSelected(string characterName)
    {
        for (int i = 0; i < partyMembers.Count; i++)
        {
            if (partyMembers[i].name == characterName)
            {
                for (int a = 0; a < characterBattleInfo.Count; a++)
                {
                    if (characterBattleInfo[a].characterType == BattleInfo.characterTypes.player && characterBattleInfo[a].characterIndex == partyMemberTurn)
                    {
                        characterBattleInfo[a].targetIndex = i;
                        panelController.OnCharacterSelected(characterBattleInfo[a]);
                    }
                }
            }
        }
        signalManager.EmitSignal("actionSelected");
    }

    //Allows the player to select a talent from the talents panel
    void AbilitySelected(string abilityName)
    {
       for (int a = 0; a < characterBattleInfo.Count; a++)
       {
            if (characterBattleInfo[a].characterType == BattleInfo.characterTypes.player && characterBattleInfo[a].characterIndex == partyMemberTurn)
            {
                characterBattleInfo[a].abilityToUse = abilityName;
            }
       }
        panelController.OnAbilitySelected(characterBattleInfo, partyMemberTurn);
    }

    //Allows the player to select an enemy when its clicked on
    void SelectEnemy(string enemyName)
    {
        for (int i = 0; i < enemyUIs.Count; i++)
        {
            if (enemyUIs[i].Name == enemyName)
            {
                for (int a = 0; a < characterBattleInfo.Count; a++)
                {
                    if (characterBattleInfo[a].characterType == BattleInfo.characterTypes.player && characterBattleInfo[a].characterIndex == partyMemberTurn)
                    {
                        characterBattleInfo[a].targetIndex = i;
                        panelController.ToggleEnemySelection(characterBattleInfo);
                    }
                }
                signalManager.EmitSignal("actionSelected");
            }
        }
    }

    void ItemSelected(string name)
    {
        for (int a = 0; a < characterBattleInfo.Count; a++)
        {
            if (characterBattleInfo[a].characterType == BattleInfo.characterTypes.player && characterBattleInfo[a].characterIndex == partyMemberTurn)
            {
                characterBattleInfo[a].itemToUse = name;
            }
        }
        panelController.OnItemSelected();
    }
    
    //Sets the bools for whether or not an audience member liked or disliked a party member's performance or ability
    //Can be given a particular affinity if a character's ability has a unique affinity
    void CheckAffinities(BattleInfo character, string affinity)
    {
        int characterIndex = character.characterIndex;
        int targetIndex = character.targetIndex;
        List<string> characterAffinities = partyMembers[characterIndex].affinites;
        List<string> targetPositiveAffinities = enemyData[targetIndex].myPositiveAffinities;
        List<string> targetNegativeAffinities = enemyData[targetIndex].myNegativeAffinities;
        if (affinity != null)
        {
            foreach (string posAffinity in targetPositiveAffinities)
            {
                if (affinity == posAffinity)
                {
                    enemies[targetIndex].likedPerformance = true;
                }
            }
            foreach (string negAffinity in targetNegativeAffinities)
            {
                if (affinity == negAffinity)
                {
                    if (enemies[targetIndex].reverseNegativeAffinities == true)
                    {
                        enemies[targetIndex].likedPerformance = true;
                    }
                    else
                    {
                        enemies[targetIndex].dislikedPerformance = true;
                    }
                }
            }
        }
        else
        {
            for (int a = 0; a < characterAffinities.Count; a++)
            {
                foreach (string posAffinity in targetPositiveAffinities)
                {
                    if (characterAffinities[a] == posAffinity)
                    {
                        enemies[targetIndex].likedPerformance = true;
                    }
                }
                foreach (string negAffinity in targetNegativeAffinities)
                {
                    if (characterAffinities[a] == negAffinity)
                    {
                        if (enemies[targetIndex].reverseNegativeAffinities == true)
                        {
                            enemies[targetIndex].likedPerformance = true;
                        }
                        else
                        {
                            enemies[targetIndex].dislikedPerformance = true;
                        }
                    }
                }
            }
        }
    }

    //Sorts each character in the battle in order of their focus(speed). Used to determine character turn order.
    void SortBySpeed()
    {
        for (int i = 0; i < characterBattleInfo.Count - 1; i++)
        {
            if (characterBattleInfo[i].currentStatAmounts["characterSpeed"] < characterBattleInfo[i + 1].currentStatAmounts["characterSpeed"])
            {
                BattleInfo tempBattleInfo;
                tempBattleInfo = characterBattleInfo[i];
                characterBattleInfo[i] = characterBattleInfo[i + 1];
                characterBattleInfo[i + 1] = tempBattleInfo;

                i = -1;
            }
        }
    }
}