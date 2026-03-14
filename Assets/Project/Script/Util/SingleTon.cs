using UnityEngine;

namespace Utility
{
    // Claude - RuntimeInitializeOnLoadMethod�� ���׸� Ŭ�������� �������� ����
    //         �����׸� ���̽��� ���� �ξ� ���� �ʱ�ȭ�� ����ϰ� �и�
    public abstract class SingleTonBase : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetAll()
        {
            s_isQuitting = false;
        }

        // Claude - isQuitting�� ��� �̱����� �����ؾ� �ϹǷ� ���̽����� ����
        protected static bool s_isQuitting = false;

        private void OnApplicationQuit() => s_isQuitting = true;
        // OnDestroy는 씬 전환 시에도 호출되므로 여기서 s_isQuitting을 건드리지 않음
        // 종료 감지는 OnApplicationQuit, 도메인 리셋은 ResetAll()이 담당
    }

    public abstract class SingleTon<T> : SingleTonBase where T : SingleTon<T>
    {
        protected static T _instance;

        // Claude - _instance ������ �� ���׸� Ÿ�Կ��� Awake�� ó���ǹǷ�
        //         null üũ + ���� ���� ��ȿ�� �������� ����
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
            // Claude - Domain Reload ���� ��� ���� ������ �ı��� ������Ʈ��
            //         _instance�� �������� �� �����Ƿ� null üũ �ʿ�
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

            // Claude - ���׸� Ư���� _instance ������ SubsystemRegistration���� �ȵǹǷ�
            //         �ı��� ������Ʈ ���� ���θ� Equals(null)�� üũ
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