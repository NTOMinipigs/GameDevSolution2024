using System;
using UnityEngine;

namespace Utils
{
    /// <summary>
    /// Базовый класс для реализации паттерна Singleton в MonoBehaviour
    /// Обеспечивает потокобезопасную реализацию и контроль жизненного цикла
    /// </summary>
    /// <typeparam name="T">Тип компонента, наследующего SingletonBehaviour</typeparam>
    public abstract class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
    {
        #region Статические поля и свойства

        /// <summary>
        /// Экземпляр синглтона
        /// </summary>
        private static T _instance;

        /// <summary>
        /// Объект для блокировки доступа к экземпляру из разных потоков
        /// </summary>
        private static readonly object _lock = new object();

        /// <summary>
        /// Флаг, указывающий на завершение работы приложения
        /// </summary>
        private static bool _applicationIsQuitting = false;

        /// <summary>
        /// Флаг, указывающий, должен ли объект сохраняться при переходе между сценами
        /// </summary>
        [SerializeField] protected bool _dontDestroyOnLoad = true;

        /// <summary>
        /// Доступ к экземпляру синглтона
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_applicationIsQuitting)
                {
                    Debug.LogWarning($"[Singleton] Экземпляр {typeof(T)} уже уничтожен при выходе из приложения. Возвращаем null.");
                    return null;
                }

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        // Ищем экземпляр в сцене
                        T[] instances = FindObjectsOfType<T>();

                        if (instances.Length > 0)
                        {
                            if (instances.Length > 1)
                            {
                                Debug.LogError($"[Singleton] Найдено несколько экземпляров {typeof(T)}. Используем первый, остальные будут уничтожены.");

                                // Оставляем только один экземпляр
                                for (int i = 1; i < instances.Length; i++)
                                {
                                    Destroy(instances[i].gameObject);
                                }
                            }

                            _instance = instances[0];
                            Debug.Log($"[Singleton] Использован существующий экземпляр {typeof(T)}");
                        }
                        else
                        {
                            // Создаем новый экземпляр, если не найден
                            GameObject singleton = new GameObject($"(Singleton) {typeof(T)}");
                            _instance = singleton.AddComponent<T>();
                            Debug.Log($"[Singleton] Создан новый экземпляр {typeof(T)}");
                        }
                    }

                    return _instance;
                }
            }
        }

        /// <summary>
        /// Проверяет, существует ли экземпляр синглтона
        /// </summary>
        public static bool Exists => _instance != null && !_applicationIsQuitting;

        #endregion

        #region События

        /// <summary>
        /// Событие, вызываемое при инициализации синглтона
        /// </summary>
        public static event Action<T> OnSingletonInitialized;

        /// <summary>
        /// Событие, вызываемое перед уничтожением синглтона
        /// </summary>
        public static event Action<T> OnSingletonDestroyed;

        #endregion

        #region Методы жизненного цикла

        /// <summary>
        /// Инициализация синглтона
        /// </summary>
        protected virtual void Awake()
        {
            InitializeSingleton();
        }

        /// <summary>
        /// Установка флага завершения приложения при уничтожении объекта
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                OnSingletonDestroyed?.Invoke((T)this);
                _instance = null;
            }
        }

        /// <summary>
        /// Установка флага завершения приложения при выходе
        /// </summary>
        protected virtual void OnApplicationQuit()
        {
            _applicationIsQuitting = true;
        }

        #endregion

        #region Приватные методы

        /// <summary>
        /// Инициализирует экземпляр синглтона
        /// </summary>
        private void InitializeSingleton()
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = (T)this;

                    if (_dontDestroyOnLoad)
                    {
                        transform.SetParent(null);
                        DontDestroyOnLoad(gameObject);
                    }

                    // Вызываем события после инициализации
                    OnInitialized();
                    OnSingletonInitialized?.Invoke(_instance);
                }
                else if (_instance != this)
                {
                    Debug.Log($"[Singleton] Экземпляр {typeof(T)} уже существует. Уничтожаем дубликат.");
                    Destroy(gameObject);
                }
            }
        }

        #endregion

        #region Защищенные методы

        /// <summary>
        /// Вызывается после инициализации синглтона
        /// Переопределите для выполнения дополнительной инициализации
        /// </summary>
        protected virtual void OnInitialized() { }

        #endregion
    }
}
