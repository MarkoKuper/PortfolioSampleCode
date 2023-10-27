using Godot;
using System;

public class EnemySelectorArrow : TextureRect
{
    SignalManager signalManager;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        signalManager = GetNode<SignalManager>("/root/SignalManager");

        signalManager.Connect("hideArrow", this, "HideArrow");
        signalManager.Connect("moveArrow", this, "MoveArrow");
    }

    void HideArrow()
    {
        Hide();
    }

    void MoveArrow(Vector2 enemyUIPosition, Vector2 size)
    {
        Show();
        RectGlobalPosition = enemyUIPosition + new Vector2(size.x - (RectSize.y * 1.30f), - (size.y + RectSize.x + 10f));
    }
}
