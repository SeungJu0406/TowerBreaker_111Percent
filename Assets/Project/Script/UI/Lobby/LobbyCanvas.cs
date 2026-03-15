using NSJ_MVVM;
using UnityEngine;

public class LobbyCanvas : BaseCanvas
{
    public enum Panel
    {
        Lobby,
        Chest,
        Equipment,
    }

    void OnEnable ()
    {
        ChangePanel(Panel.Lobby);
    }
}
