using UnityEngine;

public class SkullObject : MonoBehaviour
{
    [SerializeField] private float throwForce = 10f;

    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // 방향 상단 180도 사이에서 랜덤하게 설정
        Vector2 direction = Quaternion.Euler(0, 0, Random.Range(-90f, 90f)) * Vector2.up;

        Throw(direction, throwForce);
        GetSkullPoint();
    }

    public void Throw(Vector2 direction, float force)
    {
        _rb.AddForce(direction * force, ForceMode2D.Impulse);
    }

    public void GetSkullPoint()
    {
        UserDataManager.Instance.SkullCount += 1;
    }
}
