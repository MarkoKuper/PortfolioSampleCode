using Godot;
using System.Collections.Generic;

public class BaseAbilities : Resource
{
	public enum abilityTypes { Heal, Buff, Debuff, Damage, Passive, Defence, BuffAffinity, DebuffAffinity };
	public enum affectedStat { Style, Focus, Spice, Confidence, Criticality, Insensitivity };
	public enum affinities { None, Flashy, Calm, Elegant, Cute, Loud, Angry }

	[Export]
	public string abilityName = "Defense Buff";
	[Export]
	public string abilityDescriptiveWord;
	[Export(PropertyHint.MultilineText)]
	public string abilityDescription;
	[Export]
	public abilityTypes abilityType;
	[Export]
	public bool needsTarget;
	[Export]
	public bool targetsEnemy;
	[Export]
	public bool targetsEveryone;
	[Export]
	public bool stun;
	[Export]
	public List<affectedStat> affectedStats;
	[Export]
	public affinities abilityAffinity = affinities.None;
	[Export]
	public float abilityCost = 3f;
	[Export]
	public float abilityEffectAmount;
	[Export]
	public int abilityLength;
}
