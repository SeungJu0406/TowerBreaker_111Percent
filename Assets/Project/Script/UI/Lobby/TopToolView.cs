using NSJ_MVVM;
using UnityEngine;
using UnityEngine.UI;

public class TopToolView : BaseView
{
    public Button ExitButton => _exitButton;
    private Button _exitButton;
    protected override void InitAwake()
    {

    }

    protected override void InitGetUI()
    {
        _exitButton = GetUI<Button>("ExitButton");
    }

    protected override void InitStart()
    {

    }

    protected override void SubscribeEvents()
    {
        _exitButton.onClick.AddListener(() => Canvas.ChangePanel(LobbyCanvas.Panel.Lobby));
    }
}

