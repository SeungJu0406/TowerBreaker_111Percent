using System.Collections.Generic;
using UnityEngine;

public class Layer
{
    public int Enemy => GetLayer("Enemy");



    private Dictionary<string, int> LayerDic = new Dictionary<string, int>();

    public int GetLayer(string key)
    {
        if(LayerDic.ContainsKey(key) == false)
        {
            LayerDic.Add(key, LayerMask.GetMask(key));
        }
        return LayerDic[key];
    }
}
