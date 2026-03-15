using System.Collections.Generic;
using UnityEngine.Events;
using Utility;

public class UserDataManager : SingleTon<UserDataManager>
{
    public UserData UserData = new UserData();

    public int SkullCount { get => UserData.SkullCount; set { UserData.SkullCount = value; OnSkullCountChanged?.Invoke(value); } }
    public int ChestCount { get => UserData.ChestCount; set { UserData.ChestCount = value; OnChestCountChanged?.Invoke(value); } }

    public Equipment CurEquipment { get => UserData.CurEquipment; set => UserData.CurEquipment =value; }

    public List<Equipment> Equipments
    {
        get
        {
            if (UserData.Equipments == null)
                UserData.Equipments = new List<Equipment>();
            return UserData.Equipments;
        }
    }

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
    public int SkullCount;
    // 오픈 가능한 상자 개수
    public int ChestCount;

    public Equipment CurEquipment;
    // 장비
    public List<Equipment> Equipments;
}
