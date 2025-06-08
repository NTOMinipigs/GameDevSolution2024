using System;
using UnityEngine;

/// <summary>
/// Класс, хранящий данные о текущем игроке
/// Реализует потокобезопасный паттерн Singleton для доступа из любого места кода
/// </summary>
public class Player : MonoBehaviour
{
    #region Singleton Implementation

    private static Player _instance;
    private static readonly object _lockObject = new object();
    private static bool _applicationIsQuitting = false;

    /// <summary>
    /// Событие, вызываемое при инициализации экземпляра Player
    /// </summary>
    public static event Action OnPlayerInitialized;

    /// <summary>
    /// Доступ к экземпляру класса из любого места (потокобезопасная реализация)
    /// </summary>
    public static Player Instance
    {
        get
        {
            if (_applicationIsQuitting)
            {
                Debug.LogWarning("[Singleton] Попытка доступа к экземпляру Player после завершения приложения.");
                return null;
            }

            lock (_lockObject)
            {
                return _instance;
            }
        }
    }

    /// <summary>
    /// Инициализация Singleton при создании объекта
    /// </summary>
    private void Awake()
    {
        InitializeSingleton();
    }

    /// <summary>
    /// Устанавливает флаг завершения приложения при уничтожении объекта
    /// </summary>
    private void OnDestroy()
    {
        if (_instance == this)
        {
            _applicationIsQuitting = true;
        }
    }

    /// <summary>
    /// Инициализирует экземпляр Singleton
    /// </summary>
    private void InitializeSingleton()
    {
        lock (_lockObject)
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);

                // Вызываем событие инициализации
                OnPlayerInitialized?.Invoke();
            }
            else if (_instance != this)
            {
                Debug.Log("Обнаружен дубликат экземпляра Player. Уничтожаем объект.");
                Destroy(gameObject);
            }
        }
    }

    #endregion

    #region Player Data

    [Header("Основные данные игрока")]
    [Tooltip("Имя игрока, используемое для идентификации")]
    [SerializeField] private string _playerName;

    [Tooltip("Сид для генерации случайных событий")]
    [SerializeField] private int _seed;

    /// <summary>
    /// Имя игрока
    /// </summary>
    public string PlayerName
    {
        get => _playerName;
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogWarning("Попытка установить пустое имя игрока");
                return;
            }
            _playerName = value;
        }
    }

    /// <summary>
    /// Сид для генерации случайных событий
    /// Используется для передачи из меню в игру
    /// </summary>
    public int Seed
    {
        get => _seed;
        set => _seed = value;
    }

    /// <summary>
    /// Для обратной совместимости со старым кодом
    /// </summary>
    [Obsolete("Используйте свойство PlayerName вместо публичного поля playerName")]
    public string playerName
    {
        get => PlayerName;
        set => PlayerName = value;
    }

    /// <summary>
    /// Для обратной совместимости со старым кодом
    /// </summary>
    [Obsolete("Используйте свойство Seed вместо публичного поля seed")]
    public int seed
    {
        get => Seed;
        set => Seed = value;
    }

    #endregion

    /// <summary>
    /// Проверяет валидность данных игрока
    /// </summary>
    /// <returns>True, если данные игрока валидны</returns>
    public bool IsValid()
    {
        return !string.IsNullOrEmpty(_playerName);
    }
}
