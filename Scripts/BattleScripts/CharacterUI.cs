using Godot;
using System;

public class CharacterUI : Control
{
    ProgressBar healthBar;
    ProgressBar creativityBar;

    AnimationPlayer myAnimationPlayer;

    string characterName;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        healthBar = GetNode<ProgressBar>("ProgressBar");
        if(Owner != null)
        {
            myAnimationPlayer = Owner.GetNode<AnimationPlayer>("AnimationPlayer");
        }
        else
        {
            myAnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        }
        if (GetNodeOrNull<ProgressBar>("CreativityBar") != null)
        {
            creativityBar = GetNode<ProgressBar>("CreativityBar");
        }
    }

    public void SetHealth(float currentHealth, float maxHealth)
    {
        healthBar.Value = currentHealth;
        healthBar.MaxValue = maxHealth;
        if(creativityBar != null)
        {
            healthBar.GetNode<Label>("Label").Text = "Energy: " + currentHealth + "/" + maxHealth;
        }
        else
        {
            healthBar.GetNode<Label>("Label").Text = "Entertainment: " + currentHealth + "/" + maxHealth;
        }
    }

    public void SetCreativity(float currentCreativity, float maxCreativity)
    {
        creativityBar.Value = currentCreativity;
        creativityBar.MaxValue = maxCreativity;
        creativityBar.GetNode<Label>("Label").Text = "Creativity: " + currentCreativity + "/" + maxCreativity;
    }

    public void SetCharacterName(string name)
    {
        characterName = name;
    }

    public void SetEnemySprite(Texture sprite)
    {
        TextureRect enemySprite = GetNode<TextureRect>("EnemySprite");
        enemySprite.Texture = sprite;
    }

    public void ToggleVisibility()
    {
        Visible = !Visible;
    }

    public void PlayEnemyHurt(string name)
    {
        myAnimationPlayer.Play(name + "Damaged");
    }

    public void PlayCharacterHurt()
    {
        myAnimationPlayer.Play(characterName + "Hurt");
    }

    public void PlayIdle()
    {
        myAnimationPlayer.Play(characterName + "EncounterIdle");
    }

    public void PlayCharacterEntertained(string characterName)
    {
        ToggleVisibility();
        Owner.GetNode<TextureRect>("Shadow").Visible = false;
        myAnimationPlayer.Play(characterName + "Entertained");
    }
}
