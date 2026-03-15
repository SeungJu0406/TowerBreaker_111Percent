using NSJ_MVVM;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipView : BaseView
{
    [SerializeField] private EquipSlot _slotPrefab;

    private List<EquipSlot> _slots = new List<EquipSlot>();

    private Image _curImage;
    private TMP_Text _curName;
    private TMP_Text _curDamage;


    private Transform _content;

    private GameObject _popupInfo;
    private Image _popupImage;
    private TMP_Text _popupName;
    private TMP_Text _popupDamage;
    private Button _popupEquipButton;
    private Button _popupExit;


    private EquipSlot _choiceSlot;
    protected override void InitAwake()
    {

    }

    protected override void InitGetUI()
    {
        _curImage = GetUI<Image>("CurImage");
        _curName = GetUI<TMP_Text>("CurName");
        _curDamage = GetUI<TMP_Text>("CurDamage");

        _content = GetUI("Content").transform;

        _popupInfo = GetUI("PopupInfo");
        _popupImage = GetUI<Image>("PopupImage");
        _popupName = GetUI<TMP_Text>("PopupName");
        _popupDamage = GetUI<TMP_Text>("PopupDamage");
        _popupEquipButton = GetUI<Button>("PopupEquipButton");
        _popupExit = GetUI<Button>("PopupExit");

    }

    protected override void InitStart()
    {

    }

    protected override void SubscribeEvents()
    {
        _popupEquipButton.onClick.AddListener(Equip);
        _popupExit.onClick.AddListener(() => _popupInfo.SetActive(false));
    }

    private void OnEnable()
    {
        List<Equipment> equipments = UserDataManager.Instance.Equipments;

        for (int i = 0; i < equipments.Count; i++)
        {
            EquipSlot newSlot = Instantiate(_slotPrefab, _content);

            newSlot.SetEquip(equipments[i]);

            newSlot.SubscribesEventButton(() => ShowInfoPopup(newSlot));

            _slots.Add(newSlot);
        }

        _popupInfo.SetActive(false);
    }

    private void OnDisable()
    {
        for (int i = _slots.Count - 1; i >= 0; i--)
        {
            Destroy(_slots[i]);
        }

        _slots.Clear();
    }

    private void ShowInfoPopup(EquipSlot slot)
    {
        _popupInfo.SetActive(true);
        _popupImage.sprite = slot.Equipment.Data.Sprite;
        _popupName.text = slot.Equipment.Data.Name;
        _popupDamage.text = $"Damage : {slot.Equipment.Data.BasicDamage}";

        _choiceSlot = slot;
    }

    private void Equip()
    {
        Equipment equipment = _choiceSlot.Equipment;
        // 실제 유저 데이터 장비에 입력

        // UI 설정
        _curImage.sprite = equipment.Data.Sprite;
        _curName.text = equipment.Data.Name;
        _curDamage.text =$"Damage : {equipment.Data.BasicDamage}";


        // 리스트에서 제거
         _slots.Remove(_choiceSlot);
        Destroy(_choiceSlot.gameObject);   
        _choiceSlot = null;

        UserDataManager.Instance.Equipments.Remove(equipment);
        UserDataManager.Instance.CurEquipment = equipment;


        _popupInfo.SetActive(false);
    }
}
