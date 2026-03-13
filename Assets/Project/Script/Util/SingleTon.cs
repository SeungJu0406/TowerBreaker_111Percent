using UnityEngine;

namespace Utility
{
    public abstract class SingleTon<T> : MonoBehaviour where T : SingleTon<T>
    {
        protected static T _instance;
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
                _instance.transform.SetParent(null); // Ensure the singleton is not a child of any other object
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

        /// <summary>
        /// 싱글톤 인스턴스를 설정합니다.
        /// </summary>
        protected static void SetSingleton()
        {
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