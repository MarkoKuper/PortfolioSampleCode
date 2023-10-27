using Godot;
using System;
using System.Collections.Generic;

public class GlobalCharacterData : Node
{
    public List<BaseEnemy> enemyData;
    public List<Resource> encounterDrops;
    public List<BasePlayerCharacter> partyMembers = new List<BasePlayerCharacter>();

    public Battle.BattleOutcome battleOutcome;

    public string battleName;

    public bool canDip;

    public void AddToEnemies(List<BaseEnemy> baseEnemies)
    {
        enemyData = new List<BaseEnemy>();
        for (int i = 0; i < baseEnemies.Count; i++)
        {
            enemyData.Add(baseEnemies[i]);
        }
    }

    public void AssignBattleResult(Battle.BattleOutcome outcome, string name)
    {
        battleOutcome = outcome;
        battleName = name;
    }
}
