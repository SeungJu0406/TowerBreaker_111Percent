using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public struct AttackResult
{
    public bool Success;
    public bool IsCritical;
    public float TotalDamage;
    public GameObject Target;

    public AttackResult(bool success, bool isCritical, float totalDamage, GameObject target)
    {
        Success = success;
        IsCritical = isCritical;
        TotalDamage = totalDamage;
        Target = target;
    }
}

public interface IBattle
{
    IHitable Hit { get; set; }

    /// <summary>
    /// target, hitDamage, isCritical
    /// </summary>
    public event UnityAction<GameObject, float, bool> OnTargetAttackEvent;
    /// <summary>
    /// hitDamage, isCritical
    /// </summary>
    public event UnityAction<float, bool> OnTakeDamageEvent;
    /// <summary>
    /// 공격 
    /// </summary>
    public AttackResult AttackTarget(GameObject target, float damage, CriticalHit critical = default, bool invokeEvent = true);

    /// <summary>
    /// 공격
    /// </summary>
    public AttackResult AttackTarget<T>(T target, float damage, CriticalHit critical = default, bool invokeEvent = true) where T : Component;

}