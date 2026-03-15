using NSJ_MVVM;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverView : BaseView
{
    private GameObject _popUp;
    private Button _exitButton;
    protected override void InitAwake()
    {

    }

    protected override void InitGetUI()
    {
        _popUp = GetUI("PopUp");
        _exitButton = GetUI<Button>("ExitButton");
    }

    protected override void InitStart()
    {
        if (Manager.Event != null)
            Manager.Event.OnPlayerDied += ShowPopUp;


        _popUp.SetActive(false);
    }

    private void OnDestroy()
    {
        if (Manager.Event != null)
            Manager.Event.OnPlayerDied -= ShowPopUp;
    }

    protected override void SubscribeEvents()
    {
        _exitButton.onClick.AddListener(()=>SceneManager.LoadScene("Lobby"));
    }


    private void ShowPopUp()
    {
        _popUp.SetActive(true);
    }
}
