using Godot;
using System;

public class ItemResultsUI : HBoxContainer
{
    public void SetUI(Texture texture, string name)
    {
        TextureRect itemPortrait = GetNode<TextureRect>("ItemPortrait");
        Label itemName = GetNode<Label>("ItemName");

        itemPortrait.Texture = texture;
        itemName.Text = name;
    }
}
