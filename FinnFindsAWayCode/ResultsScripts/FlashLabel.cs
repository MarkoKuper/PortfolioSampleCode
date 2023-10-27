using Godot;
using System;

public class FlashLabel : Label
{
    [Export]
    public float flashTime;

    [Export]
    public string labelText;

    float timeLeft;

    public override void _Ready()
    {
        timeLeft = flashTime;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if(timeLeft <= 0)
        {
            if(Text.Length < labelText.Length)
            {
                Text = labelText;
            }
            else
            {
                Text = "";
            }
            timeLeft = flashTime;
        }
        timeLeft -= delta;
    }
}
