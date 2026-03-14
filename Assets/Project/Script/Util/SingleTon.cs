using UnityEngine;

namespace Utility
{
    public abstract class SingleTon<T> : MonoBehaviour where T : SingleTon<T>
    {
        protected static T _instance;

        // Claude - 앱 종료 여부를 추적하는 플래그. 종료 시 새 인스턴스 생성을 막기 위해 사용
        private static bool s_isQuitting = false;

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

        // Claude - 앱 종료 시작 시점에 호출되며, 이후 SetSingleton의 인스턴스 생성을 차단
        private void OnApplicationQuit()
        {
            s_isQuitting = true;
        }

        // Claude - 씬 언로드 시에도 파괴될 수 있으므로 OnDestroy에서도 플래그 체크
        //         단, 실제 종료가 아닌 중복 파괴(Destroy(gameObject))는 걸러냄
        private void OnDestroy()
        {
            if (_instance == this)
            {
                s_isQuitting = true;
            }
        }

        protected virtual void InitSingletonBefore() { }
        protected abstract void InitAwake();

        /// <summary>
        /// 싱글톤 인스턴스를 설정합니다.
        /// 앱 종료 중이라면 새 인스턴스를 생성하지 않습니다.
        /// </summary>
        protected static void SetSingleton()
        {
            // Claude - 종료 중이면 null을 반환하도록 즉시 탈출 (좀비 오브젝트 생성 방지)
            if (s_isQuitting) return;

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