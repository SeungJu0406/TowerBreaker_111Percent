using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using Utility;

public class UserDataManager : SingleTon<UserDataManager>
{
    public UserData UserData { get; private set; } = new UserData();

    public int SkullCount { get => UserData.SkullCound; set { UserData.SkullCound = value; OnSkullCountChanged?.Invoke(value); } }
    public int ChestCount { get => UserData.ChestCount; set { UserData.ChestCount = value; OnChestCountChanged?.Invoke(value); } }

    public event UnityAction<int> OnSkullCountChanged;
    public event UnityAction<int> OnChestCountChanged;

    protected override void InitAwake()
    {
        
    }
}


[System.Serializable]
public class UserData
{
    // 해골(돈) 개수
    public int SkullCound;
    // 오픈 가능한 상자 개수
    public int ChestCount;
}
