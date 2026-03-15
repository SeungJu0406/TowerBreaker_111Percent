using System.Collections;
using UnityEngine;

namespace NSJ_Player
{
    public class DashBehaviour : MonoBehaviour
    {
        [SerializeField] private Player _player;

        [Header("Dash")]
        [SerializeField] private float _dashSpeed = 10f;

        private bool _canDash = true;
        private bool _isDefending = false;
        // 층 전환 중에는 대쉬 불가 — 화면 밖 이동 중 대쉬하면 위치가 엇나갈 수 있음
        private bool _isTransitioning = false;


        private void Awake()
        {
            if (_player == null)
            {
                _player = GetComponentInParent<Player>();
            }

        }

        private void Start()
        {
            if (Manager.Event == null) return;
            Manager.Event.OnDefenceStart += OnDefenceStart;
            Manager.Event.OnDefenceEnd += OnDefenceEnd;
            Manager.Event.OnStageTransitionStart += OnTransitionStart;
            Manager.Event.OnStageTransitionEnd += OnTransitionEnd;
        }

        private void OnDestroy()
        {
            if (Manager.Event == null) return;
            Manager.Event.OnDefenceStart -= OnDefenceStart;
            Manager.Event.OnDefenceEnd -= OnDefenceEnd;
            Manager.Event.OnStageTransitionStart -= OnTransitionStart;
            Manager.Event.OnStageTransitionEnd -= OnTransitionEnd;
        }

        private void OnDefenceStart() => _isDefending = true;
        private void OnDefenceEnd() => _isDefending = false;
        private void OnTransitionStart() => _isTransitioning = true;
        private void OnTransitionEnd() => _isTransitioning = false;


        public void Dash()
        {
            // _isTransitioning: 층 전환 연출 중에는 입력 차단
            if (_canDash == false || _isDefending || _isTransitioning) return;
            StartCoroutine(DashRoutine());
        }

        private IEnumerator DashRoutine()
        {
            _canDash = false;

            // 앞으로 이동 Transform.TransLate 사용
            // 적과 충돌전 까지
            while (_player.IsCollide == false)
            {
                _player.Rb.MovePosition(_player.Rb.position + Vector2.right * _dashSpeed * Time.fixedDeltaTime);
                yield return new WaitForFixedUpdate();
            }
            // 대쉬 중지
            _canDash = true;
        }
    }
}
