using Godot;
using System.Collections.Generic;

public class CutscenesController : AnimationPlayer
{

    SignalManager signalManager;

    Node musicManager;

    public override void _Ready()
    {
        musicManager = GetNode<Node>("/root/MusicManager");

        Play("OpeningCutscene");

        Node highestNode = GetHighestNode(this);

        if (highestNode.FindNode("DialogueBox") != null)
        {
            Node dialogueBox = highestNode.FindNode("DialogueBox");
            dialogueBox.AddUserSignal("PlayCutscene");
            dialogueBox.Connect("PlayCutscene", this, "PlayCutsceneDialogueBox");
        }

        signalManager = GetNode<SignalManager>("/root/SignalManager");
        signalManager.Connect("playCutscene", this, "PlayCutscene");
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

    public void PlayCutsceneDialogueBox(List<string> data)
    {
        Play(data[0]);
    }

    public void PlayCutscene(string name)
    {
        Play(name);
    }

    public void PlaySong(string songName)
    {
        musicManager.EmitSignal("PlayMusic", songName, null);
    }

    public void _on_HallwayStory_area_entered(Area2D area)
    {
       if (area.GetParent().Name == "Player")
        {
            PlaySong("MUS_exploringTheHouse");
        }
    }
}
