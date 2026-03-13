using UnityEngine;
using UnityEngine.Events;

public static class Manager
{
    public static EventManager Event;
    public static FloorManager Floor;
    public static void SetEvent(EventManager globalEvent)
    {
        Event = globalEvent;
    }
    public static void SetFloor(FloorManager floor) => Floor = floor;
}
