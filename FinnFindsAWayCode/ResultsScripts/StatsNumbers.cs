using Godot;
using System;
using System.Collections.Generic;

public class StatsNumbers : VBoxContainer
{
    List<Label> statNumLabels = new List<Label>();

    List<float> statNum = new List<float>();
    List<float> statIncrease = new List<float>();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        InitiateLabels();
    }

    void InitiateLabels()
    {
        foreach (Node item in GetChildren())
        {
            statNumLabels.Add(item as Label);
        }
    }

    public void DisplayStats(BasePlayerCharacter character)
    {
        GetStatNumbers(character);
        GetStatIncreases(character);

        for (int i = 0; i < statNumLabels.Count; i++)
        {
            Label label = statNumLabels[i];
            float stat = statNum[i];
            float increase = statIncrease[i];

            label.Text = stat + " +" + increase;
        }
    }

    public void UpdateStatLabels()
    {
        for (int i = 0; i < statNumLabels.Count; i++)
        {
            Label label = statNumLabels[i];
            float stat = statNum[i];
            float increase = statIncrease[i];

            label.Text = (stat + increase).ToString();
        }
    }

    public void GetStatNumbers(BasePlayerCharacter character)
    {
        statNum.Add(character.baseStats["maxEnergy"]);
        statNum.Add(character.baseStats["maxCreativity"]);
        statNum.Add(character.baseStats["baseStyle"]);
        statNum.Add(character.baseStats["baseSpice"]);
        statNum.Add(character.baseStats["maxEnergy"]);
        statNum.Add(character.baseStats["maxEnergy"]);
    }

    public void GetStatIncreases(BasePlayerCharacter character)
    {
        statIncrease.Add(character.energyOnLevelUp);
        statIncrease.Add(character.creativityOnLevelUp);
        statIncrease.Add(character.styleOnLevelUp);
        statIncrease.Add(character.spiceOnLevelUp);
        statIncrease.Add(character.confidenceOnLevelUp);
        statIncrease.Add(character.focusOnLevelUp);
    }
}
