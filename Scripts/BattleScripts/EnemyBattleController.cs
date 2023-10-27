using Godot;
using System;
using System.Collections.Generic;

public class EnemyBattleController : Node
{
    GlobalCharacterData globalCharacterData;
    
    SignalManager signalManager;

    List<BasePlayerCharacter> partyMembers;

    List<BaseEnemy> enemyData;

    public override void _Ready()
    {
        globalCharacterData = GetNode<GlobalCharacterData>("/root/GlobalCharacterData");
        partyMembers = globalCharacterData.partyMembers;
        enemyData = globalCharacterData.enemyData;
        signalManager = GetNode<SignalManager>("/root/SignalManager");
    }

    //Runs the enemy AI that is found at enemyData[index]
    public void EnemyTakesTurn(List<BattleInfo> characteraBattleInfo, int index)
    {
        List<BattleInfo> enemies = MakeEnemiesList(characteraBattleInfo);
        BattleInfo myInfo = enemies[index];
        BaseEnemy myData = enemyData[index];
        myInfo.currentStatAmounts["currentCreativity"] += Mathf.Floor(0.1f * myData.baseStats["maxCreativity"]);

        if (myData.name == "Slimette")
        {
            SlimeGuy(characteraBattleInfo, myInfo, myData);
        }
        else if(myData.name == "Griff")
        {
            Griff(characteraBattleInfo, myInfo, myData);
        }
        else if(myData.name == "Slime Guy")
        {
            SlimeGuy(characteraBattleInfo, myInfo, myData);
        }
        else if(myData.name == "Finn")
        {
            FinnAudience(myInfo, myData);
        }
    }

    //Returns the creativity(float) required by the lowest cost ability in a list of BaseAbilities
    float LowestCostAbility(List<BaseAbilities> abilities)
    {
        float lowestCost = abilities[0].abilityCost;
        for (int i = 0; i < abilities.Count; i++)
        {
            if(abilities[i].abilityCost < lowestCost)
            {
                lowestCost = abilities[i].abilityCost;
            }
        }
        return lowestCost;
    }

    //Returns the required creativity needed to use an ability given abilityName in a list of BaseAbilities
    public float RequiredCreativity(List<BaseAbilities> abilities, string abilityName)
    {
        for(int i = 0; i < abilities.Count; i++)
        {
            if(abilities[i].abilityName == abilityName)
            {
                return abilities[i].abilityCost;
            }
        }
        return 0;
    }

    public BaseAbilities GetAbility(List<BaseAbilities> abilities, string abilityName)
    {
        foreach (BaseAbilities ability in abilities)
        {   
            if(abilityName == ability.abilityName)
            {
                return ability;
            }
        }
        return null;
    }

    //Griff's AI used to determine which ability Griff should use during a battle
    void Griff(List<BattleInfo> characterBattleInfo, BattleInfo myInfo, BaseEnemy myData)
    {
        List<BaseAbilities> otherAbilities = new List<BaseAbilities>();
        for (int i = 0; i < myData.abilities.Count; i++) //Will add every abiity that Griff has other than Observe and Hefty Expectations to otherAbilities
        {
            if (myData.abilities[i].abilityName != "Observe" && myData.abilities[i].abilityName != "Hefty Expectations")
            {
                otherAbilities.Add(myData.abilities[i]);
            }
        }
        RandomNumberGenerator randomNumber = new RandomNumberGenerator();
        randomNumber.Randomize();
        int chosenNumber = randomNumber.RandiRange(1, 3);
        if(chosenNumber < 2) //This will allow Griff to favour using Observe over their other abilities
        {
            signalManager.EmitSignal("useAbility", GetAbility(myData.abilities, "Observe"), myInfo, characterBattleInfo);
        }
        else
        {
            if(myInfo.currentStatAmounts["currentCreativity"] >= RequiredCreativity(myData.abilities, "Hefty Expectations")) //If Griff uses an ability other than Observe, they will favour using Hefty Expectations if they have enough Creativity
            {
                signalManager.EmitSignal("useAbility", GetAbility(myData.abilities, "Hefty Expectations"), myInfo, characterBattleInfo);
            }
            else if(myInfo.currentStatAmounts["currentCreativity"] >= LowestCostAbility(otherAbilities)) //Otherwise they will use an ability in otherAbilities if they have enough Creativity
            {
                myInfo.targetIndex = ChooseRandomPlayer();
                BaseAbilities ability = ChooseRandomAbility(otherAbilities, myInfo);
                signalManager.EmitSignal("useAbility", GetAbility(myData.abilities, ability.abilityName), myInfo, characterBattleInfo);
            }
            else //Otherwise they will use Observe if they don't have enough Creativity for their other abilities
            {
                signalManager.EmitSignal("useAbility", GetAbility(myData.abilities, "Observe"), myInfo, characterBattleInfo);
            }
        }
    }

    //SlimeGuy's AI used to determine which ability SlimeGuy should use during a battle
    void SlimeGuy(List<BattleInfo> characterBattleInfo, BattleInfo myInfo, BaseEnemy myData)
    {
        List<BaseAbilities> firstAbilities = new List<BaseAbilities>();
        List<BaseAbilities> highCreativityAbilities = new List<BaseAbilities>();
        List<BaseAbilities> everythingButObserve = new List<BaseAbilities>();

        for (int i = 0; i < myData.abilities.Count; i++)
        {
            if (myData.abilities[i].abilityName == "A Slime's POV" || myData.abilities[i].abilityName == "Slime Time")
            {
                firstAbilities.Add(myData.abilities[i]);
            }
            if (myData.abilities[i].abilityName == "Observe" || myData.abilities[i].abilityName == "Slimastic Defense")
            {
                highCreativityAbilities.Add(myData.abilities[i]);
            }
            if (myData.abilities[i].abilityName != "Observe")
            {
                everythingButObserve.Add(myData.abilities[i]);
            }
        }
        if (myInfo.currentRound == 1) //Slime guy will use one of the abilities found in first if its the first round of combat for slime guy
        {
            myInfo.targetIndex = ChooseRandomPlayer();
            BaseAbilities chosenAbility = ChooseRandomAbility(firstAbilities, myInfo);
            GD.Print("Slimette is using " + chosenAbility.abilityName);
            signalManager.EmitSignal("useAbility", GetAbility(myData.abilities, chosenAbility.abilityName), myInfo, characterBattleInfo);
            signalManager.EmitSignal("PlaySound", "ENC_Slimette_Talent", null);
        }
        //else if (myInfo.currentStatAmounts["currentEntertainment"] > (myData.baseStats["maxEntertainment"] * 0.5) && myInfo.currentStatAmounts["currentCreativity"] >= RequiredCreativity(myData.abilities, "Slimence")) //Slime guy will always choose to heal themselves if their currentEntertainemtn is greater than half of their max Entertainment, if they have enough creativity
        //{
        //    signalManager.EmitSignal("useAbility", GetAbility(myData.abilities, "Slimence"), myInfo, characterBattleInfo);
        //}
        else if(PartyAverageCurrentCreativity() < PartyAverageBaseCreativity() && myInfo.currentStatAmounts["currentCreativity"] >= LowestCostAbility(highCreativityAbilities)) //Slime guy will choose to use the abilities found in highCreativityAbilities when the player's party has a high amount of creativity
        {
            BaseAbilities chosenAbility = ChooseRandomAbility(highCreativityAbilities, myInfo);
            signalManager.EmitSignal("useAbility", GetAbility(myData.abilities, chosenAbility.abilityName), myInfo, characterBattleInfo);
            signalManager.EmitSignal("PlaySound", "ENC_Slimette_Talent", null);
        }
        else if(myInfo.currentStatAmounts["currentCreativity"] >= LowestCostAbility(everythingButObserve)) //Otherwise Slime guy will randomly use one of their abilities other than Observe if they have enough creativity
        {
            myInfo.targetIndex = ChooseRandomPlayer();
            BaseAbilities chosenAbility = ChooseRandomAbility(everythingButObserve, myInfo);
            signalManager.EmitSignal("useAbility", GetAbility(myData.abilities, chosenAbility.abilityName), myInfo, characterBattleInfo);
            signalManager.EmitSignal("PlaySound", "ENC_Slimette_Talent", null);
        }
        else //Otherwise Slime guy will use Observe
        {
            signalManager.EmitSignal("useAbility", GetAbility(myData.abilities, "Observe"), myInfo, characterBattleInfo);
            signalManager.EmitSignal("PlaySound", "ENC_Slimette_Observe", null);
        }
    }
    
    //Slimelette's AI used to determine which ability Slimelette should use during a battle
    void Slimette(List<BattleInfo> characterBattleInfo, BattleInfo myInfo, BaseEnemy myData)
    {
        List<BattleInfo> enemies = MakeEnemiesList(characterBattleInfo);
        List<int> damagedTeam = new List<int>();
        List<int> livingTeam = new List<int>();
        List<int> livingTeamWithoutSelf = new List<int>();

        for (int a = 0; a < enemies.Count; a++)
        {
            if (enemies[a].currentStatAmounts["currentEntertainment"] > 0 && enemies[a].currentStatAmounts["currentEntertainment"] <= enemyData[a].baseStats["maxEntertainment"])
            {
                damagedTeam.Add(a);
            }
            if(enemies[a].currentStatAmounts["currentEntertainment"] < enemyData[a].baseStats["maxEntertainment"])
            {
                livingTeam.Add(a);
            }
            if(enemies[a].currentStatAmounts["currentEntertainment"] < enemyData[a].baseStats["maxEntertainment"] && enemies[a].characterIndex != myInfo.characterIndex)
            {
                livingTeamWithoutSelf.Add(a);
            }
        }
        List<BaseAbilities> otherAbilities = new List<BaseAbilities>();
        for (int i = 0; i < myData.abilities.Count; i++)
        {
            if(myData.abilities[i].abilityName != "Slimica" && myData.abilities[i].abilityName != "Slimure" && myData.abilities[i].abilityName != "Observe")
            {
                otherAbilities.Add(myData.abilities[i]);
            }
        }
        if (damagedTeam.Count > 1 && myInfo.currentStatAmounts["currentCreativity"] >= RequiredCreativity(myData.abilities, "Slimica")) //Slimelette will use Slimica if more than one of the team is damaged
        {
            signalManager.EmitSignal("useAbility", GetAbility(myData.abilities, "Slimica"), myInfo, characterBattleInfo);
        }
        else if (damagedTeam.Count == 1 && myInfo.currentStatAmounts["currentCreativity"] >= RequiredCreativity(myData.abilities, "Slimure")) //Slimelette will use Slimure if only one team member is damaged
        {
            myInfo.targetIndex = damagedTeam[0];
            signalManager.EmitSignal("useAbility", GetAbility(myData.abilities, "Slimure"), myInfo, characterBattleInfo);
        }
        else if (damagedTeam.Count == 0 && myInfo.currentStatAmounts["currentCreativity"] >= LowestCostAbility(otherAbilities) && livingTeamWithoutSelf.Count > 0) //Slimelette will use one of the otherAbilities if no team members are damaged
        {
            RandomNumberGenerator randomNumber = new RandomNumberGenerator();
            randomNumber.Randomize();
            int randomTeamMember = randomNumber.RandiRange(0, livingTeamWithoutSelf.Count - 1);
            myInfo.targetIndex = livingTeamWithoutSelf[randomTeamMember];
            BaseAbilities ability = ChooseRandomAbility(otherAbilities, myInfo);
            signalManager.EmitSignal("useAbility", GetAbility(myData.abilities, ability.abilityName), myInfo, characterBattleInfo);
        }
        else //Otherwise Slimelette will use Observe
        {
            signalManager.EmitSignal("useAbility", GetAbility(myData.abilities, "Observe"), myInfo, characterBattleInfo);
        }
    }

    void FinnAudience(BattleInfo myInfo, BaseEnemy myData)
    {
        signalManager.EmitSignal("useAbility", GetAbility(myData.abilities, "FinnAudience"), myInfo, null);
    }

    //Enemies will critique(damage) the last character that entertained them, otherwise they randomly choose a party member
    //Their critique damage is determined by SetCriticality() and whether the target party member is Defending
    public async void EnemyCritique(List<BattleInfo> characterBattleInfo, int enemyIndex)
    {
        List<BattleInfo> enemies = MakeEnemiesList(characterBattleInfo);
        float damageToBeDone;
        int target = SelectTarget(enemies, enemyIndex);
        BattleInfo targetCharacter = GetPartyMember(characterBattleInfo, target);
        damageToBeDone = Mathf.Floor(SetCriticality(enemies, enemyIndex) - (partyMembers[target].currentStatAmounts["currentConfidence"] * 0.3f));
        for (int a = 0; a < characterBattleInfo.Count; a++)
        {
            BattleInfo character = characterBattleInfo[a];
            if (character.characterType == BattleInfo.characterTypes.player && character.characterIndex == target && character.actionChosen == BattleInfo.actions.defend)
            {
                damageToBeDone = Mathf.Floor(damageToBeDone * 0.5f); 
            }
        }
        if (damageToBeDone <= 0)
        {
            damageToBeDone = 1;
        }
        signalManager.EmitSignal("audienceText", enemyData[enemyIndex].name, "Critique", partyMembers[target].name, damageToBeDone, false, false);
        await ToSignal(signalManager, "textBoxClosed");
        targetCharacter.uiScript.PlayCharacterHurt();
        partyMembers[target].DecreaseEnergy(damageToBeDone);
        targetCharacter.uiScript.SetHealth(partyMembers[target].currentStatAmounts["currentEnergy"], partyMembers[target].baseStats["maxEnergy"]);
        if ((partyMembers[target].currentStatAmounts["currentEnergy"] - damageToBeDone) <= 0)
        {
            signalManager.EmitSignal("displayText", partyMembers[target].name + " has run out of energy!");
            await ToSignal(signalManager, "textBoxClosed");
        }
        signalManager.EmitSignal("actionsCarriedOut");
        signalManager.EmitSignal("PlaySound", "ENC_Slimette_Critique", null);
    }

    //Sets the amount of damage the enemy critique will do based on whether or not they liked or disliked their target's performance
    float SetCriticality(List<BattleInfo> enemies, int index)
    {
        if (enemies[index].likedPerformance)
        {
            return Mathf.Floor(enemies[index].currentStatAmounts["currentCriticality"] * 0.25f);
        }
        else if (enemies[index].dislikedPerformance)
        {
            return Mathf.Floor(enemies[index].currentStatAmounts["currentCriticality"] * 0.85f);
        }
        else
        {
            return Mathf.Floor(enemies[index].currentStatAmounts["currentCriticality"] * 0.55f);
        }
    }

    //Returns a float that represents the average of each party member's max creativity
    float PartyAverageBaseCreativity()
    {
        float totalCreativity = new float();
        foreach (BasePlayerCharacter partyMember in partyMembers)
        {
            totalCreativity += partyMember.baseStats["maxCreativity"];
        }
        return totalCreativity / partyMembers.Count;
    }

    //Returns a float thar represents the avereage of each party member's current creativity
    float PartyAverageCurrentCreativity()
    {
        float totalCreativity = new float();
        foreach (BasePlayerCharacter partyMember in partyMembers)
        {
            totalCreativity += partyMember.currentStatAmounts["currentCreativity"];
        }
        return totalCreativity / partyMembers.Count;
    }

    //Returns an int that will be used to target a random living enemy
    int SelectTarget(List<BattleInfo> enemies, int enemyIndex)
    {
        if(enemies[enemyIndex].entertainedThisRound && partyMembers[enemies[enemyIndex].targetIndex].currentStatAmounts["currentEnergy"] > 0)
        {
            return enemies[enemyIndex].targetIndex;
        }
        else
        {
            return ChooseRandomPlayer();
        }
    }

    //Returns a random ability from a list of base abilities
    BaseAbilities ChooseRandomAbility(List<BaseAbilities> abilities, BattleInfo enemyInfo)
    {
        RandomNumberGenerator randomNumber = new RandomNumberGenerator();
        randomNumber.Randomize();
        int chosenNumber = randomNumber.RandiRange(0, abilities.Count - 1);
        if (enemyInfo.currentStatAmounts["currentCreativity"] >= abilities[chosenNumber].abilityCost)
        {
            return abilities[chosenNumber];
        }
        else
        {
            return ChooseRandomAbility(abilities, enemyInfo);
        }
    }

    //Returns a random int that will be used to target a random party member
    int ChooseRandomPlayer()
    {
        RandomNumberGenerator randomNumber = new RandomNumberGenerator();
        randomNumber.Randomize();
        int chosenNumber = randomNumber.RandiRange(0, globalCharacterData.partyMembers.Count - 1);
        if (globalCharacterData.partyMembers[chosenNumber].currentStatAmounts["currentEnergy"] > 0)
        {
            return chosenNumber;
        }
        else
        {
            return ChooseRandomPlayer();
        }
    }

    BattleInfo GetPartyMember(List<BattleInfo> characterBattleInfo, int characterIndex)
    {
        foreach (BattleInfo character in characterBattleInfo)
        {   
            if(character.characterType == BattleInfo.characterTypes.player && character.characterIndex == characterIndex)
            {
                return character;
            }
        }
        return null;
    }

    List<BattleInfo> MakeEnemiesList(List<BattleInfo> characterBattleInfo)
    {
        List<BattleInfo> enemies = new List<BattleInfo>();
        foreach (BattleInfo character in characterBattleInfo)
        {
            if (character.characterType == BattleInfo.characterTypes.enemy)
            {
                enemies.Add(character);
            }
        }
        return enemies;
    }
}
