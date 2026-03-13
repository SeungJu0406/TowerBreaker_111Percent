using System.Collections;
using UnityEngine;

namespace NSJ_Player
{
    public class DashBehaviour : MonoBehaviour
    {
        [SerializeField] private Player _player;
        [SerializeField] private KeyCode _dashKey = KeyCode.Z;

        [Header("Dash")]
        [SerializeField] private float _dashSpeed = 10f;

        private bool _canDash = true;
        private bool _isDefending = false;
        
        private void Awake()
        {
            if (_player == null)
                _player = GetComponentInParent<Player>();
        }

        private void Start()
        {
            if (GlobalEventManager.GlobalEvent == null) return;
            GlobalEventManager.GlobalEvent.OnDefenceStart += OnDefenceStart;
            GlobalEventManager.GlobalEvent.OnDefenceEnd += OnDefenceEnd;
        }

        private void OnDestroy()
        {
            if (GlobalEventManager.GlobalEvent == null) return;
            GlobalEventManager.GlobalEvent.OnDefenceStart -= OnDefenceStart;
            GlobalEventManager.GlobalEvent.OnDefenceEnd -= OnDefenceEnd;
        }

        private void OnDefenceStart() => _isDefending = true;
        private void OnDefenceEnd() => _isDefending = false;

        private void Update()
        {
            if (_canDash == false || _isDefending) return;

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
            while (_player.IsCollide == false)
            {
                _player.transform.Translate(Vector2.right * _dashSpeed * Time.deltaTime);
                yield return null;
            }

            // 대쉬 중지
            _canDash = true;
        }
    }
}
