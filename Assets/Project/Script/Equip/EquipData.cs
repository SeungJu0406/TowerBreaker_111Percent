using UnityEngine;

[CreateAssetMenu(fileName = "EquipData", menuName = "Scriptable Objects/EquipData")]
public class EquipData : ScriptableObject
{
    public string Name;
    public Sprite Sprite;
    public float BasicDamage;
}
