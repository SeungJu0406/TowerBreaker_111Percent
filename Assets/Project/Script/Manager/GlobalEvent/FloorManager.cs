using NUnit.Framework.Internal;
using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    [SerializeField] private List<FloorData> _floorData;

    [SerializeField] private Floor _floorPrefab;

    [SerializeField] private List<Floor> _curActiveFloor;
    [SerializeField] private Floor _currentFloor;
    [SerializeField] private int _currentFloorIndex;

    void Awake()
    {
        Manager.SetFloor(this);
    }

    void OnDestroy()
    {
        if (Manager.Floor == this)
        {
            Manager.SetFloor(null);
        }
    }

    public FloorData GetFloorData(int floorIndex)
    {
        if (floorIndex < 0 || floorIndex >= _floorData.Count)
        {
            Debug.LogError($"Floor index {floorIndex} is out of range.");
            return null;
        }
        return _floorData[floorIndex];
    }

    // 플로어 생성
    private void CreateFloor()
    {
        // 현재 층으로 부터 +3층 생성

    }

    // 플로어 삭제
    private void DestroyFloor()
    {
        // 현재 층으로 부터 -2층 삭제

    }
}
