using NSJ_MVVM;
using NSJ_Player;
using UnityEngine;
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
        _dashButton.onClick.AddListener(_dashBehaviour.Dash);
        _defenceButton.onClick.AddListener(_defenceBehaviour.Defence);
        _attackButton.onClick.AddListener(_attackBehaviour.Attack);
    }
}
