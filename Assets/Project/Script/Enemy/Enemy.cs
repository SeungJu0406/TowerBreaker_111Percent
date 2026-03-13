using System;
using UnityEngine;
using UnityEngine.Events;

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

        public event UnityAction OnDie;

        public void SetCanHit(bool canHit) => _canHit = canHit;
        public void SetNeighbor(Enemy prevNeighbor, Enemy nextNeighbor)
        {
            _neighborInfo.PrevNeighbor = prevNeighbor;
            _neighborInfo.NextNeighbor = nextNeighbor;
        }
        public void TakeDamage(float damage)
        {
            if (_canHit == false) return;

            _hp -= damage;

            if (_hp <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            // 죽는 로직
            _canHit = false;
            _neighborInfo.NextNeighbor?.SetCanHit(true);

            // 죽는 이펙트


            OnDie?.Invoke();

            // 비활성화
            gameObject.SetActive(false);
        }
    }
}