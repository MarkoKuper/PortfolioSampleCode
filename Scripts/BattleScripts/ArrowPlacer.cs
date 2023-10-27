using Godot;
using System;

public class ArrowPlacer : Control
{
    SignalManager signalManager;

    bool mouseOver = false;
    bool mouseClicked = false;

    public override void _Ready()
    {
        signalManager = GetNode<SignalManager>("/root/SignalManager");
    }

    public override void _Process(float delta)
    {
        SelectEnemy();
    }

    //Emits signal selectThisEnemy and sets mouseClicked to true, when the left mouse button is clicked, and when mouseOver is true and mouseClicked is false
    void SelectEnemy()
    {
        if (Input.IsActionPressed("LeftMouseClick") && mouseOver && !mouseClicked)
        {
            signalManager.EmitSignal("selectThisEnemy", GetParent().GetParent().Name);
            signalManager.EmitSignal("targetSelected");
            mouseClicked = true;
        }
    }

    //Emits signal moveArrow and sets mouseOver to true and mouseClicked to false when the mouse enters an Area2D
    void _on_Area2D_mouse_entered()
    {
        mouseOver = true;
        mouseClicked = false;
        signalManager.EmitSignal("moveArrow", RectGlobalPosition, GetParent<Control>().RectSize);
        GD.Print("Arrow Placer: Mouse Entered");
    }

    //Emits signal hideArrow and sets mouseOver and mouseClicked to false when the mouse exits an Area2D
    void _on_Area2D_mouse_exited()
    {
        mouseOver = false;
        mouseClicked = false;
        signalManager.EmitSignal("hideArrow");
    }

}
