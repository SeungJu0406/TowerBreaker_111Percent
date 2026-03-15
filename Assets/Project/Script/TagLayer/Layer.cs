using System.Collections.Generic;
using UnityEngine;

public static class Layer
{
    public static int Enemy => GetLayer("Enemy");
    public static int Boss  => GetLayer("Boss");

    public static int IgnoreCollide => GetLayer("IgnoreCollide");

    // Enemy | Boss — OverlapBoxAll에서 둘 다 감지
    public static int EnemyGroup => Enemy | Boss;

    private static Dictionary<string, int> LayerDic = new Dictionary<string, int>();

    public static int GetLayer(string key)
    {
        if (!LayerDic.ContainsKey(key))
            LayerDic.Add(key, LayerMask.GetMask(key));
        return LayerDic[key];
    }
}
