using UnityEngine;
using UnityEngine.Events;

public static class GlobalEventManager
{
    public static GlobalEventComponent GlobalEvent;

    public static void SetGlobalEvent(GlobalEventComponent globalEvent)
    {
        GlobalEvent = globalEvent;
    }
}
