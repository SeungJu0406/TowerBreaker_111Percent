using UnityEngine;

namespace Utility
{
    // Claude - RuntimeInitializeOnLoadMethod는 제네릭 클래스에서 동작하지 않음
    //         비제네릭 베이스를 따로 두어 정적 초기화를 담당하게 분리
    public abstract class SingleTonBase : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetAll()
        {
            s_isQuitting = false;
        }

        // Claude - isQuitting은 모든 싱글톤이 공유해야 하므로 베이스에서 관리
        protected static bool s_isQuitting = false;

        private void OnApplicationQuit() => s_isQuitting = true;
        private void OnDestroy() { if (this == this) s_isQuitting = true; } // 실제 인스턴스 파괴 시
    }

    public abstract class SingleTon<T> : SingleTonBase where T : SingleTon<T>
    {
        protected static T _instance;

        // Claude - _instance 리셋은 각 제네릭 타입에서 Awake로 처리되므로
        //         null 체크 + 이전 참조 유효성 검증으로 대응
        public static T Instance
        {
            get
            {
                SetSingleton();
                return _instance;
            }
        }

        private void Awake()
        {
            // Claude - Domain Reload 꺼진 경우 이전 세션의 파괴된 오브젝트가
            //         _instance에 남아있을 수 있으므로 null 체크 필요
            if (_instance != null && _instance.Equals(null))
            {
                _instance = null;
            }

            InitSingletonBefore();
            if (_instance == null)
            {
                _instance = this as T;
                _instance.transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
                return;
            }
            InitAwake();
        }

        protected virtual void InitSingletonBefore() { }
        protected abstract void InitAwake();

        protected static void SetSingleton()
        {
            if (s_isQuitting) return;

            // Claude - 제네릭 특성상 _instance 리셋이 SubsystemRegistration에서 안되므로
            //         파괴된 오브젝트 참조 여부를 Equals(null)로 체크
            if (_instance != null && _instance.Equals(null))
            {
                _instance = null;
            }

            if (_instance == null)
            {
                _instance = FindAnyObjectByType<T>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<T>();
                    singletonObject.name = typeof(T).ToString();
                    _instance.transform.SetParent(null);
                }
                DontDestroyOnLoad(_instance.gameObject);
            }
        }
    }
}