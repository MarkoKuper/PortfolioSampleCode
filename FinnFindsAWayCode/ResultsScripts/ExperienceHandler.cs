using Godot;
using System;

public class ExperienceHandler : Node
{
    SignalManager signalManager;
    
    CharacterResultsUI characterResultsUI;

    BasePlayerCharacter partyMember;

    int amountToAdd;

    float timeBetweenExpUpdates = 0.02f;

    bool addExperince = false;

    public void StartAddingExperience(CharacterResultsUI myCharacterResultsUI, BasePlayerCharacter myPartyMember, int myAmountToAdd, SignalManager sigMan)
    {
        signalManager = sigMan;
        characterResultsUI = myCharacterResultsUI;
        partyMember = myPartyMember;
        amountToAdd = myAmountToAdd;
        addExperince = true;
        AddExperience();
    }

    async void AddExperience()
    {
        while (addExperince)
        {
            partyMember.currentExperience++;
            characterResultsUI.UpdateExpProgress(partyMember);
            amountToAdd--;            
            if (partyMember.currentExperience == partyMember.experienceForNextLevel)
            {
                LevelUp(partyMember);
                await ToSignal(signalManager, "levelUpPanelFinished");

            }
            if (amountToAdd <= 0)
            {
                addExperince = false;
                signalManager.EmitSignal("experienceAdded");
            }
            await ToSignal(GetTree().CreateTimer(timeBetweenExpUpdates), "timeout");
        }
    }

    void LevelUp(BasePlayerCharacter member)
    {
        signalManager.EmitSignal("levelUp", member);
        member.experienceForLastLevel = member.experienceForNextLevel;
        member.experienceForNextLevel *= 2;
        member.level++;
        characterResultsUI.LevelUp(member);
        member.LevelUp();
    }
}
