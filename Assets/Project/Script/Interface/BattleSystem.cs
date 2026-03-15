using AutoPool_Tool;
using UnityEngine;
using UnityEngine.Events;

public enum CrowdControlType { None, Stiff, Stun, Size }
public struct CriticalHit
{
    public float CritRate;
    public float Multiplier;

    public CriticalHit(float critRate, float damage)
    {
        CritRate = critRate;
        Multiplier = damage;
    }
}
public class BattleSystem : MonoBehaviour, IBattle
{
    [SerializeField] private bool _isDisplayDamageText = true;

    public IHitable Hit { get; set; }


    public Transform HitPoint;

    /// <summary>
    /// target, hitDamage, isCritical
    /// </summary>
    public event UnityAction<GameObject, float, bool> OnTargetAttackEvent;
    /// <summary>
    /// hitDamage, isCritical
    /// </summary>
    public event UnityAction<float, bool> OnTakeDamageEvent;

    [HideInInspector] public bool IsDie;
    private void Awake()
    {
        Hit = GetComponent<IHitable>();
        if (HitPoint == null)
        {
            HitPoint = new GameObject("HitTextPoint").transform;
            HitPoint.SetParent(transform, true);
            HitPoint.localPosition = new Vector3(0, 0.5f, 0);
        }
    }

    private void OnDisable()
    {

    }
    #region 공격하기 

    public AttackResult AttackTarget(GameObject target, float baseDamage, CriticalHit critical = default, bool invokeEvent = true)
    {
        BattleSystem battle = target.gameObject.GetComponent<BattleSystem>(); 
        if (battle == null)
            return new AttackResult(false, false, default, null);

        if (battle.TryHit(transform) == false)
            return new AttackResult(false, false, default, null);

        // 크리티컬 계산
        float critRate = Mathf.Clamp01(critical.CritRate);
        bool isCritical = critRate > 0f && Random.value < critRate;

        float totalDamage = baseDamage;
        if (isCritical)
        {
            totalDamage *= critical.Multiplier;
        }


        float hitDamage = battle.TakeDamage(transform, totalDamage, isCritical, invokeEvent); 
        if (invokeEvent == true)
        {
            OnTargetAttackEvent?.Invoke(target, hitDamage, isCritical);
        }
        return new AttackResult(true, isCritical, hitDamage, target);
    }

    public AttackResult AttackTarget<T>(T target, float damage, CriticalHit critical = default, bool invokeEvent = true) where T : Component
    {
        return AttackTarget(target.gameObject, damage, critical, invokeEvent);
    }
    #endregion
    #region 공격 받기
    public bool TryHit(Transform attacker)
    {
        return Hit.TryHit(attacker);
    }

    public float TakeDamage(Transform attacker,float damage, bool isCritical = false, bool invokeEvent = true)
    {
        float hitDamage = Hit.TakeDamage(attacker, damage);

        CreateDamageText(hitDamage, isCritical);

        if(invokeEvent == true)
        {
            OnTakeDamageEvent?.Invoke(damage, false);
        }

        return hitDamage;
    }
    #endregion

    private void CreateDamageText(float damage, bool isCritical)
    {
        if (_isDisplayDamageText == false) return;

        DamageText textPrefab = Resources.Load<DamageText>("DamageUI");

        Vector3 firstSpawnPoint = new Vector3(10000, 10000, 0);
        DamageText text = ObjectPool.Get(textPrefab, firstSpawnPoint, textPrefab.transform.rotation);
        text.SetDamageText(HitPoint, damage, isCritical);
    }
}
