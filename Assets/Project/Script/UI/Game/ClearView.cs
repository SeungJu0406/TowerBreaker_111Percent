using NSJ_MVVM;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClearView : BaseView
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
          Manager.Event.OnGameClear += ShowPopUp;


        _popUp.SetActive(false);
    }

    private void OnDestroy()
    {
        if (Manager.Event != null)
           Manager.Event.OnGameClear -= ShowPopUp;
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
