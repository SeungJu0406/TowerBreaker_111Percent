using NSJ_Player;
using System;
using System.Collections;
using UnityEngine;

public class DashBehaviour : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private KeyCode _dashKey = KeyCode.Z;

    [Header("Dash")]
    [SerializeField] private float _dashSpeed = 10f;


    private bool _canDash = true;

    private bool _isCollide = false;
    private void Awake()
    {
        if (_player == null)
            _player = GetComponentInParent<Player>();
    }


    private void Update()
    {
        if (_canDash == false) return;

        if (Input.GetKeyDown(_dashKey))
        {
            Dash();
        }
    }

    private void Dash()
    {
       StartCoroutine(DashRoutine());
    }

    private IEnumerator DashRoutine()
    {
        _canDash = false;

        // 앞으로 이동 Transform.TransLate 사용
        // 적과 충돌전 까지
        while (_player.IsEnemyCollide ==false)
        {
            _player.transform.Translate(Vector2.right * _dashSpeed * Time.deltaTime);
            yield return null;
        }

        Debug.Log(1);
        // 대쉬 중지
        _canDash = true;
    }
    private void SetCanCollide(bool canDash)
    {
        _isCollide = canDash;
    }
}
