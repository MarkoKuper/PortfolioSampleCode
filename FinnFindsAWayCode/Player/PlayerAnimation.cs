using Godot;
using System;

public class PlayerAnimation : AnimatedSprite
{
   bool right;
    bool left;
    bool down;
    bool up;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
       
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        //SetAnimation();
    }

    public void SetAnimation(Vector2 moveDirection)
    {
        if (moveDirection.x > 0)
        {
            if (!right)
            {
                right = true;
                left = false;
                down = false;
                up = false;
            }
            Play("Walking_Right");
        }
        else if (moveDirection.x < 0)
        {
            if (!left)
            {
                right = false;
                left = true;
                down = false;
                up = false;
            }
            Play("Walking_Left");
        }
        else if (moveDirection.y < 0)
        {
            if (!up)
            {
                right = false;
                left = false;
                down = false;
                up = true;
            }
            Play("Walking_Up");
        }
        else if (moveDirection.y > 0)
        {
            if (!down)
            {
                right = false;
                left = false;
                down = true;
                up = false;
            }
            Play("Walking_Down");
        }
        else
        {
            SetIdleDirection();
        }
    }

    void SetIdleDirection()
    {
        if (right)
        {
            Play("Facing_Right");
        }
        else if(left)
        {
            Play("Facing_Left");
        }
        else if (up)
        {
            Play("Facing_Up");
        }
        else if (down)
        {
            Play("Facing_Down");
        }
    }
}
