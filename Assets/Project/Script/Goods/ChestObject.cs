using UnityEngine;

public class ChestObject : MonoBehaviour, IHitable
{
    [SerializeField] private float _health = 20f;


    private bool _canHit = false;
    public float TakeDamage(Transform attacker, float damage)
    {
        // 몹이 다 죽은 후에 때릴 수 있음




        return damage;
    }

    public bool TryHit(Transform attacker)
    { 
        return _canHit;
    }

    private void DetectAllDiedEnemy()
    {
        // 몹이 다 죽은 것을 감지하고 다 죽으면
        // _canHit을 true로 바꿔서 때릴 수 있게 해준다.
    }

    private void Die()
    {
        // 보상 까지는 로직
        UserDataManager.Instance.ChestCount++;

        // 보상 이펙트


        // 비활성화
        gameObject.SetActive(false);
    }


}
