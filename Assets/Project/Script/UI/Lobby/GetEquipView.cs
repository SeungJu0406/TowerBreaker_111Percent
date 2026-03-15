using NSJ_MVVM;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GetEquipView : BaseView
{
    private GameObject _popUp;
    private TMP_Text _equipName;
    private Image _equipImage;
    private Button _exitButton;

    public void GetEquipment()
    {
        EquipData equipData = EquipmentContaioner.Instance.GetRandomData();

        Equipment newEquipment = new Equipment();
        newEquipment.Data = equipData;
        newEquipment.ReinforedCount = 0;

        UserDataManager.Instance.UserData.Equipments.Add(newEquipment);

        SetPopUI(equipData);
    }

    protected override void InitAwake()
    {

    }

    protected override void InitGetUI()
    {
        _popUp = GetUI("PopUp");
        _equipName = GetUI<TMP_Text>("EquipName");
        _equipImage = GetUI<Image>("EquipImage");
        _exitButton = GetUI<Button>("ExitButton");
    }

    protected override void InitStart()
    {

    }

    private void OnEnable()
    {
        _popUp.SetActive(false);
    }

    protected override void SubscribeEvents()
    {
        _exitButton.onClick.AddListener(() => _popUp.SetActive(false));
    }

    private void SetPopUI(EquipData equipData)
    {
        _equipName.text = equipData.Name;

        if (equipData.Sprite != null)
            _equipImage.sprite = equipData.Sprite;

        _popUp.SetActive(true);
    }
}
