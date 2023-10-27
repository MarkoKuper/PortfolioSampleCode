using Godot;
using System;

public class CharacterResultsUI : HBoxContainer
{
    TextureRect characterPortrait;

    Label characterLevel;
    Label currentExperience;
    Label nextLevelExp;
    Label characterName;

    ProgressBar expProgressBar;

    public void UpdateExpProgress(BasePlayerCharacter partyMember)
    {
        currentExperience.Text = partyMember.currentExperience.ToString();
        expProgressBar.Value = partyMember.currentExperience;
    }

    public void LevelUp(BasePlayerCharacter partyMember)
    {
        expProgressBar.MinValue = partyMember.experienceForLastLevel;
        expProgressBar.MaxValue = partyMember.experienceForNextLevel;
        characterLevel.Text = "Lvl " + partyMember.level;
        nextLevelExp.Text = "Next Lvl: " + partyMember.experienceForNextLevel;
    }

    public void InitializeUI(BasePlayerCharacter partyMember)
    {
        characterLevel = GetNode<Label>("VBoxContainer/CharacterLevel");
        currentExperience = GetNode<Label>("VBoxContainer2/CurrentExperience");
        nextLevelExp = GetNode<Label>("VBoxContainer2/NextLevelExp");
        expProgressBar = GetNode<ProgressBar>("VBoxContainer2/ProgressBar");
        characterPortrait = GetNode<TextureRect>("CharacterPortrait");
        characterName = GetNode<Label>("VBoxContainer/CharacterName");

        characterPortrait.Texture = partyMember.texture;
        characterName.Text = partyMember.name;
        characterLevel.Text = "Lvl: " + partyMember.level.ToString();
        currentExperience.Text = "EXP: " + partyMember.currentExperience.ToString();
        nextLevelExp.Text = "Next Lvl: " + partyMember.experienceForNextLevel.ToString();
        expProgressBar.MinValue = partyMember.experienceForLastLevel;
        expProgressBar.MaxValue = partyMember.experienceForNextLevel;
        expProgressBar.Value = partyMember.currentExperience;
    }
}
