using AutoPool_Tool;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EquipSlot : MonoBehaviour
{
    [SerializeField] private GameObject _slotClickImage;
    [SerializeField] private Image _slotImage;
    [SerializeField] private Button _slotButton;

    public Equipment Equipment => _equipment;
    private Equipment _equipment;


    private void Awake()
    {
        UnClickSlot();
    }

    public void SubscribesEventButton(UnityAction onSlotClick)
    {
        _slotButton.onClick.AddListener(onSlotClick);
    }


    public void ClickSlot()
    {
        _slotClickImage.SetActive(true);
    }

    public void UnClickSlot()
    {
        _slotClickImage.SetActive(false);
    }

    internal void SetEquip(Equipment equipment)
    {
        _equipment = equipment;
        _slotImage.sprite = _equipment.Data.Sprite;
    }
}
