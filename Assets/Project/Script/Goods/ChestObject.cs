using UnityEngine;

public class ChestObject : MonoBehaviour, IHitable
{
    [SerializeField] private float _health = 20f;


  
    public float TakeDamage(Transform attacker, float damage)
    {
        return damage;
    }

    public bool TryHit(Transform attacker)
    {
        return true;
    }
}
