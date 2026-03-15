using NSJ_MVVM;
using NSJ_Player;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHpView : BaseView
{
    [SerializeField] private Player _player;

    private List<GameObject> _hearts = new List<GameObject>();

    protected override void InitAwake()
    {
        
    }

    protected override void InitGetUI()
    {
        _hearts.Add(GetUI("Heart1"));
        _hearts.Add(GetUI("Heart2"));
        _hearts.Add(GetUI("Heart3"));
    }

    protected override void InitStart()
    {
        foreach (GameObject heart in _hearts)
        {
            heart.SetActive(true);
        }

        _player.OnHealthChange += UpdateHeart;
    }

    protected override void SubscribeEvents()
    {
      
    }

    private void UpdateHeart(int health)
    {
        int index = health;

        _hearts[index].SetActive(false);
    }
}
