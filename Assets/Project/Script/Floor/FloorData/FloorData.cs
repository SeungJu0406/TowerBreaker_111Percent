using NSJ_Enemy;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FloorData", menuName = "Scriptable Objects/FloorData")]
public class FloorData : ScriptableObject
{
    public int FloorNumber;
    public List<EnemyGroup> EnemysGroup;
    public float FloorMultiplier = 1f;
    public bool IsExistChest = false;
}

[Serializable]
public class EnemyGroup
{
    public string GroupName;
    public List<Enemy> Enemies;
    public float EnemysSpeed;
    public float EnemyInterval;
}
