using Godot;
using System;

public class DemoOverMenu : Node
{
    void _on_Main_menu_pressed()
    {
        GetTree().Root.GetNode("NewGameHandler").Call("_newGame");
        GetTree().ChangeScene("res://Main/Scene/Maps/SceneSwitcher.tscn");
    }

    void _on_Exit_pressed()
    {
        GetTree().ChangeScene("res://Main/Scene/Menu.tscn");
    }    

}
