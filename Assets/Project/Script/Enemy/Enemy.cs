using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utility;

namespace NSJ_Enemy
{
    public class Enemy : MonoBehaviour, IHitable
    {
        [Serializable]
        public struct NeighborInfo
        {
            public Enemy PrevNeighbor;
            public Enemy NextNeighbor;
        }


        public float Hp => _hp;
        [SerializeField] private float _hp;

        public bool CanHit => _canHit;
        [SerializeField] private bool _canHit;

        public Enemy PrevNeighbor => _neighborInfo.PrevNeighbor;
        public Enemy NextNeighbor => _neighborInfo.NextNeighbor;
        [SerializeField] private NeighborInfo _neighborInfo;

        // 해골 프리팹
        [SerializeField] private SkullObject _skullPrefab;

        [Header("경직")]
        [SerializeField] private float _hitStopDuration = 0.05f;

        public event UnityAction OnDie;

        private bool _isReserveCanHitTrue;
        private void LateUpdate()
        {
            if (_isReserveCanHitTrue)
            {
                _canHit = true;
                _isReserveCanHitTrue = false;
            }
            
        }
        public void SetCanHit(bool canHit) => _isReserveCanHitTrue = canHit;
        public void SetNeighbor(Enemy prevNeighbor, Enemy nextNeighbor)
        {
            _neighborInfo.PrevNeighbor = prevNeighbor;
            _neighborInfo.NextNeighbor = nextNeighbor;
        }
        public bool TryHit(Transform attacker)
        {
            return true;
        }

        public float TakeDamage(Transform attacker, float damage)
        {
            if (_canHit == false) return 0;

            _hp -= damage;

            // 테스트용 데미지 이펙트
            TestDebugTakeDamage();

            if (_hp <= 0)
            {
                Die();
            }

            return damage;
        }

        private void Die()
        {
            HitStop.Instance.Do(_hitStopDuration);

            // 죽는 로직
            _canHit = false;
            _neighborInfo.NextNeighbor?.SetCanHit(true);

            Transform root = transform.root;
            SkullObject skull = Instantiate(_skullPrefab, root);
            skull.transform.position = transform.position;


            // 죽는 이펙트


            OnDie?.Invoke();

            // 비활성화
            gameObject.SetActive(false);
        }






        // --------- Debug -----------------------------------------------------------
        SpriteRenderer _renderer;
        Color _originColor;
        void Awake()
        {
            _renderer = GetComponentInChildren<SpriteRenderer>();
            _originColor = _renderer.color;

        }
        private void TestDebugTakeDamage()
        {
            StartCoroutine(DebugTakeDamgeRoutine());
        }
        IEnumerator DebugTakeDamgeRoutine()
        {
            _renderer.color = Color.yellow;
            yield return 0.2f.Second();
            _renderer.color = _originColor;
        }
    }
}