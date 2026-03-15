using NSJ_MVVM;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyView : BaseView
{
    private Button _start;
    private Button _chest;
    private Button _equip;

    protected override void InitAwake()
    {

    }

    protected override void InitGetUI()
    {
        _start = GetUI<Button>("StartButton");
        _chest = GetUI<Button>("ChestButton");
        _equip = GetUI<Button>("EquipButton");

    }

    protected override void InitStart()
    {

    }

    protected override void SubscribeEvents()
    {
        _start.onClick.AddListener(() => SceneManager.LoadScene("Game"));
        _chest.onClick.AddListener(() => Canvas.ChangePanel(LobbyCanvas.Panel.Chest));
        _equip.onClick.AddListener(() => Canvas.ChangePanel(LobbyCanvas.Panel.Equipment));
    }
}
