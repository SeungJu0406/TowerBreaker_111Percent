using AutoPool_Tool;
using NSJ_MVVM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenView : BaseView
{
    [SerializeField] private GameObject _chestPrefab;
    [SerializeField] private float _chestMoveDuration=0.3f;
    private Button _openButton;
    private Transform _openSlot;
    private Transform _openQueue;

    private GameObject _chestInSlot;
    private List<GameObject> _chestsInQueue = new List<GameObject>();

    protected override void InitAwake()
    {
      
    }

    protected override void InitGetUI()
    {
        _openButton = GetUI<Button>("OpenButton");
        _openSlot = GetUI("OpenSlot").transform;
        _openQueue = GetUI("OpenQueue").transform;
    }

    protected override void InitStart()
    {
       
    }

    private void OnEnable()
    {
        int chestCount = UserDataManager.Instance.ChestCount;
        for(int i = 0; i < chestCount; i++)
        {
            CreateChestImage();
        }
    }

    private void OnDisable()
    {
        if(_chestInSlot != null)
        {
            ObjectPool.Return(_chestInSlot);
            _chestInSlot= null;
        }
        if(_chestsInQueue.Count > 0)
        {
            for (int i = 0; i < _chestsInQueue.Count; i++)
            {
                ObjectPool.Return( _chestsInQueue[i]);
            }
            _chestsInQueue.Clear();
        }
    }

    protected override void SubscribeEvents()
    {
        _openButton.onClick.AddListener(OpenChest);
    }


    private void OpenChest()
    {
        // UserData에서 ChestCount 체크
        int chestCount = UserDataManager.Instance.ChestCount;
        // 0 이상일 때만 버튼 누를 수 있도록
        if (chestCount <= 0) return;

        // 장비 획득
        UserDataManager.Instance.ChestCount--;

        // 상자 없애고 당겨오기
        RemoveChestInSlot();
    }

    private void CreateChestImage()
    {
        GameObject _newChest = ObjectPool.Get(_chestPrefab);

        if(_chestInSlot == null)
        {
            _newChest.transform.SetParent(_openSlot);
            _newChest.transform.localPosition = Vector3.zero;
            _chestInSlot = _newChest;
            return;
        }
        else
        {
            // 대기열에 넣기
            _newChest.transform.SetParent(_openQueue);
            _chestsInQueue.Add(_newChest);
            return;
        }
    }

    private void RemoveChestInSlot()
    {
        ObjectPool.Return(_chestInSlot);
        _chestInSlot = null;

        if (_chestsInQueue.Count == 0) return;

        GameObject newChestInSlot = _chestsInQueue[_chestsInQueue.Count - 1];
        _chestInSlot = newChestInSlot;
        newChestInSlot.transform.SetParent(_openSlot);

        StartCoroutine(RemoveChestRoutine(
            newChestInSlot.transform,
            newChestInSlot.transform.position,
            _openSlot.position,
            _chestMoveDuration));

        _chestsInQueue.RemoveAt(_chestsInQueue.Count - 1);
    }

    IEnumerator RemoveChestRoutine(Transform mover,Vector3 start, Vector3 target, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            mover.position = Vector3.Lerp(start, target, elapsed / duration);
            yield return null;
        }
    }


}
