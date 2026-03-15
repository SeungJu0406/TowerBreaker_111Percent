using UnityEngine;
using Utility;

public class EquipmentContaioner : SingleTon<EquipmentContaioner>
{
    public EquipData[] EquipDatas;

    protected override void InitAwake()
    {
        
    }

    public EquipData GetRandomData()
    {
        return EquipDatas[Random.Range(0, EquipDatas.Length)];
    }
}
