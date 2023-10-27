using Godot;

public class LevelUpPanel : Panel
{
    SignalManager signalManager;

    StatsNumbers statsNumbers;

    Label charName;
    Label continueLabel;

    bool readyToAddStats;

    public override void _Ready()
    {
        signalManager = GetNode<SignalManager>("/root/SignalManager");

        statsNumbers = GetNode<StatsNumbers>("HBoxContainer/StatsNumbers");

        charName = GetNode<Label>("Name");

        continueLabel = GetNode<Label>("Continue");

        signalManager.Connect("levelUp", this, "LvlUpPanel");
    }

    public override void _Process(float delta)
    {
        if (Input.IsActionJustPressed("LeftMouseClick"))
        {
            if (readyToAddStats)
            {
                continueLabel.Visible = false;
                statsNumbers.UpdateStatLabels();
                readyToAddStats = false;
                continueLabel.Visible = true;
            }
            else if (continueLabel.Visible)
            {
                Hide();
                signalManager.EmitSignal("levelUpPanelFinished");
            }
        }
    }

    public void LvlUpPanel(BasePlayerCharacter character)
    {
        charName.Text = character.name;
        statsNumbers.DisplayStats(character);
        Show();
        readyToAddStats = true;
        continueLabel.Visible = true;
    }
}
