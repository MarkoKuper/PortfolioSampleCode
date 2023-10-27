using Godot;
using System;

public class EncounterTextBox : Panel
{
    Label textBoxText;

    SignalManager signalManager;

    BattleDialogueManager battleDialogue;

    [Export]
    public float textSpeed;

    public bool textFinished { get; private set; }

    bool speedUpText;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        textBoxText = GetNode<Label>("Label");
        battleDialogue = GetNode<BattleDialogueManager>("BattleDialogueManager");
        signalManager = GetNode<SignalManager>("/root/SignalManager");

        if(Name == "Textbox")
        {
            signalManager.Connect("displayText", this, "DisplayText");
            signalManager.Connect("characterText", this, "DisplayCharacterDialogue");
        }
        else if(Name == "AudienceTextbox")
        {
            signalManager.Connect("audienceText", this, "DisplayCharacterDialogue");
        }
    }

    public void DisplayCharacterDialogue(string characterName, string abilityName, string targetName, float amount, bool likedPerformance, bool dislikedPerformance)
    {
        DisplayText(battleDialogue.GetDialogue(characterName, targetName, amount, abilityName, likedPerformance, dislikedPerformance));
    }

    public async void DisplayText(string text)
    {
        textFinished = false;
        textBoxText.Text = "";
        Show();
        signalManager.EmitSignal("toggleActionPanelVisibility");
        foreach (char item in text)
        {
            if (speedUpText)
            {
                DisplayAllText(text);
                speedUpText = false;
                break;
            }
            textBoxText.Text += item.ToString();
            await ToSignal(GetTree().CreateTimer(textSpeed), "timeout");
        }
        textFinished= true;
    }

    public void DisplayAllText(string text)
    {
        textBoxText.Text = text;
    }

    public void SpeedUpText()
    {
        speedUpText = true;
    }
}
