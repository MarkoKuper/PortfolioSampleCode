using Godot;
using System;
using System.Collections.Generic;

public class BaseEnemy : Resource
{
    [Export]
    public string name = "Enemy";
    [Export]
    public Texture texture = null;
    [Export] 
    public bool hasAnimation = false;
    [Export]
    public bool hasShadow;
    [Export]
    public Dictionary<string, float> baseStats = new Dictionary<string, float>()
    {
        { "maxEntertainment", 0 }, //Health
        { "maxCreativity", 0 }, //Mana
        { "baseCriticality", 0 }, //Attack
        { "baseFocus", 0 }, //Speed
        { "baseSpice", 0 }, //Magic Multiplier
        { "baseInsensitivity", 0 }, //Defense
    };

    [Export]
    public List<BaseAbilities> abilities;
    [Export]
    public List<Resource> itemDrops;
    [Export]
    public List<string> myPositiveAffinities;
    [Export]
    public List<string> myNegativeAffinities;
    [Export]
    public int experiencedDropped;
}
