using Godot;
using System.Collections;
using System.Collections.Generic;
using TransYouth_Repo_2022.Vlad.Scripts;

public class PlayerController : KinematicBody2D
{
    //Sprite playerSprite;

    GlobalCharacterData globalCharacterData;

    SignalManager signalManager;

    Control dialogueBox;

    Node2D dreamWorld;
    Node2D realWorldFinn;

    PlayerAnimation dreamSprite;
    PlayerAnimation realSpriteSkin;
    PlayerAnimation realSpriteEyes;
    PlayerAnimation realSpriteBody;

    Vector2 playerMoveDirection;

    bool readyHasRun;

    [Export]
    public List<BasePlayerCharacter> partyMembers;
    [Export]
    public float playerSpeed = 200;
    [Export]
    public bool movement;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        if (IsCharEditor())
        {
            foreach (Node child in GetChildren())
            {
                if (child is Camera2D camera)
                {
                    camera.Current = false;
                    camera.Visible = false;
                    camera.Zoom = new Vector2(1, 1);

                }
                else if (child.Name == "DreamWorld" &&  child is Node2D d)
                {
                    d.Visible = false;

                }
                else if (child.Name == "RealWorldFinn" && child is Character dd)
                {
                    dd.Visible = true;
                }
            }
            return;
        }
        else
        {
            GetNode<Camera2D>("Camera2D").Zoom = new Vector2(0.2f,0.2f);
            //playerSprite = GetNode<Sprite>("Sprite");
            signalManager = GetNode<SignalManager>("/root/SignalManager");
            globalCharacterData = GetNode<GlobalCharacterData>("/root/GlobalCharacterData");
            dreamWorld = GetNode<Node2D>("DreamWorld");
            realWorldFinn = GetNode<Node2D>("RealWorldFinn");
            dreamSprite = GetNode<PlayerAnimation>("DreamWorld/AnimatedSprite");
            realSpriteSkin = GetNode<PlayerAnimation>("RealWorldFinn/Skin");
            realSpriteEyes = GetNode<PlayerAnimation>("RealWorldFinn/Eyes"); 
            realSpriteBody = GetNode<PlayerAnimation>("RealWorldFinn/Body");

            Node highestNode = GetHighestNode(this);
        
            if(highestNode.FindNode("DialogueBox") != null)
            {
                dialogueBox = highestNode.FindNode("DialogueBox") as Control;
                dialogueBox.AddUserSignal("IncreaseStat");
                dialogueBox.Connect("IncreaseStat", this, "_on_DialogueBox_dialogue_signal");
            }

            
            //UpdatePlayerSprite(0);
            //UpdatePartyMembers();
        }
            
        
        
    }
    bool IsCharEditor()
    {
        if (GetParent().Name=="CharEditor")
        {
            return true;
        }
        else
        {
            return false;
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

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(float delta)
    {
        if (IsCharEditor())
            return;
        
        if (movement && dialogueBox.Visible == false)
        {
            playerMoveDirection = Vector2.Zero;

            playerMoveDirection.x = Input.GetActionRawStrength("Right") - Input.GetActionRawStrength("Left");

            playerMoveDirection.y = Input.GetActionRawStrength("Down") - Input.GetActionRawStrength("Up");

            if (playerMoveDirection.Length() > 0)
            {
                playerMoveDirection = playerMoveDirection.Normalized() * playerSpeed;
                //if (!playingWalkingSFX)
                //{
                //    PlayWalkingSound();
                //}
            }
            //else if (playingWalkingSFX)
            //{
            //    StopWalkingSound();
            //}

            playerMoveDirection = MoveAndSlide(playerMoveDirection);

            UpdateAnimations();
        }

        if(dialogueBox.Visible == true && playerMoveDirection.Length() > 0)
        {
            playerMoveDirection = Vector2.Zero;
            //StopWalkingSound();
            UpdateAnimations();
        }
    }

    void UpdateAnimations()
    {
        if (dreamWorld.Visible)
        {
            dreamSprite.SetAnimation(playerMoveDirection);
        }
        else if (realWorldFinn.Visible)
        {
            realSpriteBody.SetAnimation(playerMoveDirection);
            realSpriteEyes.SetAnimation(playerMoveDirection);
            realSpriteSkin.SetAnimation(playerMoveDirection);
        }
    }

    public void PlayWalkingSound()
    {
        signalManager.EmitSignal("PlaySound", "footStep_wood1", null);
    }

    public void StopWalkingSound()
    {
        signalManager.EmitSignal("StopSound");
    }

    void UpdatePlayerSprite(int i)
    {
        //playerSprite.Texture = partyMembers[i].texture;
    }

    void UpdatePartyMembers()
    {
        for (int i = 0; i < partyMembers.Count; i++)
        {
            globalCharacterData.partyMembers.Add(partyMembers[i]);
        }
    }

    void _on_DialogueBox_dialogue_signal(List<string> data)
    {
        BasePlayerCharacter character = partyMembers[0];

        GD.Print(data[0] + " increased by " + data[1]);
        if (data[0] == "Creativity" || data[0] == "Energy")
        {
            character.IncreaseBaseStat("max" + data[0], int.Parse(data[1]));
        }
        else
        {
            character.IncreaseBaseStat("base" + data[0], int.Parse(data[1]));
        }
        character.IncreaseStatCurrentAmount("current" + data[0], int.Parse(data[1]));
    }

    void _on_AnimatedSprite_frame_changed()
    {
        if (dreamSprite != null)
        {
            if (dreamSprite.Visible || realWorldFinn.Visible)
            {
                if (dreamSprite.Animation.Left(7) == "Walking")
                {
                    if (dreamSprite.Frame == 1)
                    {
                        signalManager.EmitSignal("PlaySound", "footStep_wood2", null);
                    }
                    else if (dreamSprite.Frame == 3)
                    {
                        signalManager.EmitSignal("PlaySound", "footStep_wood3", null);
                    }
                }
                else if (realSpriteBody.Animation.Left(7) == "Walking")
                {
                    if (realSpriteBody.Frame == 1)
                    {
                        signalManager.EmitSignal("PlaySound", "footStep_wood2", null);
                    }
                    else if (realSpriteBody.Frame == 3)
                    {
                        signalManager.EmitSignal("PlaySound", "footStep_wood3", null);
                    }
                }
            }
        }
    }
}
