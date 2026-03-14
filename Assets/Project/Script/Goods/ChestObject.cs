using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 플로어에 배치되는 상자 오브젝트
///
/// 동작 흐름:
///   1. 생성 시 _canHit = false → TryHit()이 false → 공격 불가(무적)
///   2. 같은 플로어의 적이 모두 죽으면 Floor가 Unlock() 호출 → _canHit = true
///   3. 플레이어 공격 성공 → TakeDamage() → Die() → 상자 보상 지급 + OnOpened 이벤트
///   4. Floor는 OnOpened를 받아 모든 상자가 열렸을 때 OnFloorCleared 발생
///
/// 대쉬 차단:
///   Collider2D를 solid(비트리거)로, Tag를 "Chest"로 설정하면
///   Player.OnCollisionEnter2D에서 _isCollide = true → Dash가 상자 앞에서 정지
/// </summary>
public class ChestObject : MonoBehaviour, IHitable
{
    // 층의 모든 적이 죽기 전까지 false — 때릴 수 없는 무적 상태
    private bool _canHit = false;

    // Floor가 구독 → 상자가 열리면 클리어 카운트 증가
    public event UnityAction OnOpened;

    // Floor가 적 클리어 감지 후 호출 → 공격 가능 상태로 전환
    public void Unlock()
    {
        _canHit = true;
    }

    // IBattle이 공격 전 먼저 확인 — _canHit이 false이면 공격 자체가 성립 안 함
    public bool TryHit(Transform attacker)
    {
        return _canHit;
    }

    public float TakeDamage(Transform attacker, float damage)
    {
        if (!_canHit) return 0;

        Die();
        return damage;
    }

    private void Die()
    {
        // 상자 보상 지급
        UserDataManager.Instance.ChestCount++;

        // Floor에 열렸음을 알림 → 모든 상자 개봉 시 다음 층으로 진입
        OnOpened?.Invoke();

        // 보상 이펙트 (추후 구현)

        // 비활성화
        gameObject.SetActive(false);
    }
}
