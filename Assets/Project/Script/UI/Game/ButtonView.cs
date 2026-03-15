using NSJ_MVVM;
using NSJ_Player;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonView : BaseView
{
    [SerializeField] private DashBehaviour _dashBehaviour;
    [SerializeField] private DefenceBehaviour _defenceBehaviour;
    [SerializeField] private AttackBehaviour _attackBehaviour;

    private Button _dashButton;
    private Button _defenceButton;
    private Button _attackButton;
    protected override void InitAwake()
    {
 
    }

    protected override void InitGetUI()
    {
        _dashButton = GetUI<Button>("DashButton");
        _defenceButton = GetUI<Button>("DefenceButton");
        _attackButton = GetUI<Button>("AttackButton");
    }

    protected override void InitStart()
    {
   
    }

    protected override void SubscribeEvents()
    {
        EventTrigger dashTrigger = _dashButton.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry dashEntry = new EventTrigger.Entry();
        dashEntry.eventID = EventTriggerType.PointerDown;
        dashEntry.callback.AddListener(_ => _dashBehaviour.Dash());
        dashTrigger.triggers.Add(dashEntry);

        _defenceButton.onClick.AddListener(_defenceBehaviour.Defence);
        _attackButton.onClick.AddListener(_attackBehaviour.Attack);
    }
}
