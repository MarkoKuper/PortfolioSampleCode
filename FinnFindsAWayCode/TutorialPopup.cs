using Godot;
using System.Collections.Generic;

public class TutorialPopup : Node2D
{
    Area2D myArea2D;

    public CanvasLayer textBoxCV { get; private set; }

    Control dialogueBox;

    float countDown = 1f;

    bool timeout = false;

    [Export]
    public bool turnOffOnKeyPress;

    [Export]
    public bool turnOffAfterTimeDelay;

    Timer timer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        myArea2D = GetNode<Area2D>("Area2D");

        textBoxCV = GetNode<CanvasLayer>("CanvasLayer");

        timer = GetNode<Timer>("Timer");
        timer.Connect("timeout", this, "ClosePopup");

        Node highestNode = GetHighestNode(this);

        if (highestNode.FindNode("DialogueBox") != null)
        {
            dialogueBox = highestNode.FindNode("DialogueBox") as Control;
            if (dialogueBox.HasSignal("ShowTutorialPopup") == false)
            {
                dialogueBox.AddUserSignal("ShowTutorialPopup");
            }
            dialogueBox.Connect("ShowTutorialPopup", this, "ShowPopupFromDialogue");
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("Inventory") && turnOffOnKeyPress)
        {
            ClosePopup();
        }
    }

    Node GetHighestNode(Node thisNode)
    {
        if (thisNode.Owner != null)
        {
            thisNode = thisNode.Owner;
        }
        else
        {
            return thisNode;
        }
        return GetHighestNode(thisNode);
    }

    async void ShowPopupFromDialogue(List<string> data)
    {
        if (data[0] == Name)
        {
            ShowPopup(true);
            await ToSignal(GetTree().CreateTimer(countDown), "timeout");
            timeout = true;
        }
    }

    public void ShowPopup(bool fromDialogue = false)
    {
        textBoxCV.Visible = true;
        myArea2D.Monitorable = true;
        myArea2D.Monitoring = true;

        if (fromDialogue == false || turnOffAfterTimeDelay == true)
        {
            timer.Start(3);
        }
    }

    void ClosePopup()
    {
        textBoxCV.Visible = false;
        myArea2D.SetDeferred("monitorable", false);
        myArea2D.SetDeferred("monitoring", false);
        QueueFree();
    }

    public void _on_Area2D_area_entered(Area2D area)
    {
        //textBoxCV.Visible = true;
    }

    public void _on_Area2D_area_exited(Area2D area)
    {
        ClosePopup();
    }
}
