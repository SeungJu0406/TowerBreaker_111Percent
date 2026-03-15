using UnityEngine;

public interface IHitable
{
    /// <summary>
    /// 공격을 받을 수 있는지 판단합니다.
    /// </summary>
    /// <param name="attacker"></param>
    /// <returns></returns>
    bool TryHit(Transform attacker);
    /// <summary>
    /// 데미지를 받습니다.
    /// </summary>
    /// <param name="damage"></param>
    /// <returns></returns>
    float TakeDamage(Transform attacker,float damage);
}
