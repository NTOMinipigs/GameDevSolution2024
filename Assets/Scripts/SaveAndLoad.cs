using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using Utils;

/// <summary>
/// Класс, отвечающий за загрузку и сохранение игрового прогресса
/// Реализует паттерны Template Method и Strategy для обеспечения гибкой работы с разными типами данных
/// </summary>
public class SaveAndLoad : MonoBehaviour
{
    #region События

    /// <summary>
    /// Событие, вызываемое перед сохранением игры
    /// </summary>
    public static event Action OnBeforeSave;

    /// <summary>
    /// Событие, вызываемое после успешного сохранения игры
    /// </summary>
    public static event Action OnSaveCompleted;

    /// <summary>
    /// Событие, вызываемое перед загрузкой игры
    /// </summary>
    public static event Action OnBeforeLoad;

    /// <summary>
    /// Событие, вызываемое после успешной загрузки игры
    /// </summary>
    public static event Action OnLoadCompleted;

    #endregion

    #region Приватные поля

    /// <summary>
    /// Флаг первого запуска игры
    /// </summary>
    [SerializeField] private bool _isFirstBoot = false;

    /// <summary>
    /// Список стратегий сохранения/загрузки для разных типов данных
    /// </summary>
    private List<ISaveLoadStrategy> _saveLoadStrategies = new List<ISaveLoadStrategy>();

    /// <summary>
    /// Ссылка на компонент SystemSaver для взаимодействия с файловой системой
    /// </summary>
    private SystemSaver _systemSaver;

    #endregion

    #region Жизненный цикл

    private void Awake()
    {
        _systemSaver = GetComponent<SystemSaver>();
        if (_systemSaver == null)
        {
            LoggingUtils.LogError("SystemSaver компонент не найден", this);
            enabled = false;
            return;
        }

        // Инициализируем стратегии сохранения/загрузки
        InitializeSaveLoadStrategies();
    }

    private void Start()
    {
        InitializeGame();
    }

    /// <summary>
    /// При закрытии игры автоматически сохраняем прогресс
    /// </summary>
    private void OnApplicationQuit()
    {
        SaveGame();
    }

    /// <summary>
    /// При приостановке приложения также сохраняем прогресс
    /// </summary>
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveGame();
        }
    }

    #endregion

    #region Инициализация

    /// <summary>
    /// Инициализирует стратегии сохранения/загрузки для различных типов данных
    /// Реализует паттерн Strategy
    /// </summary>
    private void InitializeSaveLoadStrategies()
    {
        _saveLoadStrategies.Add(new BearsSaveLoadStrategy(this));
        _saveLoadStrategies.Add(new BuildsSaveLoadStrategy(this));
        _saveLoadStrategies.Add(new PreferenceSaveLoadStrategy(this));
        _saveLoadStrategies.Add(new InventorySaveLoadStrategy(this));
        _saveLoadStrategies.Add(new GameEventsSaveLoadStrategy(this));
    }

    /// <summary>
    /// Инициализирует игру: загружает сохранение или создает новую игру
    /// </summary>
    private void InitializeGame()
    {
        try
        {
            if (_systemSaver == null)
            {
                LoggingUtils.LogError("SystemSaver не инициализирован", this);
                return;
            }

            // Пробуем загрузить сохраненную игру
            bool loadResult = _systemSaver.LoadGame();

            // Если сохранение не найдено, создаем новую игру
            if (!loadResult)
            {
                LoggingUtils.LogInfo("Сохранение не найдено. Создание новой игры", this);
                _isFirstBoot = true;

                // В режиме отладки создаем игру с дополнительными ресурсами
                if (IsDebugModeEnabled())
                {
                    CreateDebugGame();
                }

                // Создаем новую игру
                CreateNewGame();
            }

            // Загружаем игру
            LoadGame();

            // В режиме отладки применяем дополнительные настройки
            if (IsDebugModeEnabled())
            {
                ApplyDebugSettings();
            }
        }
        catch (Exception ex)
        {
            LoggingUtils.LogException(ex, "Ошибка при инициализации игры", this);
            ShowInitializationErrorMessage();
        }
    }

    /// <summary>
    /// Проверяет, включен ли режим отладки в конфигурации
    /// </summary>
    private bool IsDebugModeEnabled()
    {
        try
        {
            return Config.ConfigManager.Instance != null && 
                   Config.ConfigManager.Instance.config != null && 
                   Config.ConfigManager.Instance.config.debug;
        }
        catch (Exception ex)
        {
            LoggingUtils.LogException(ex, "Ошибка при проверке режима отладки", this);
            return false;
        }
    }

    /// <summary>
    /// Показывает сообщение об ошибке инициализации
    /// </summary>
    private void ShowInitializationErrorMessage()
    {
        // Здесь можно добавить код для отображения UI с ошибкой
        if (AlertsManager.Singleton != null)
        {
            AlertsManager.Singleton.ShowAlert("Ошибка при инициализации игры. Пожалуйста, перезапустите приложение.");
        }
    }

    /// <summary>
    /// Создает игру в режиме отладки с увеличенными лимитами ресурсов
    /// </summary>
    private void CreateDebugGame()
    {
        LoggingUtils.LogInfo("Создание игры в режиме отладки", this);

        try
        {
            // Проставляем увеличенные лимиты ресурсов для отладки
            if (ColonyManager.Singleton != null)
            {
                // Максимальные значения ресурсов
                ColonyManager.Singleton.MaxMaterials = 200;
                ColonyManager.Singleton.MaxEnergy = 200;
                ColonyManager.Singleton.MaxBiofuel = 200;
                ColonyManager.Singleton.MaxFood = 200;
                ColonyManager.Singleton.MaxHoney = 200;
                ColonyManager.Singleton.MaxMaterialsPlus = 200;

                // Начальные значения ресурсов
                ColonyManager.Singleton.MaterialsPlus = 100;
                ColonyManager.Singleton.Materials = 100;
                ColonyManager.Singleton.Energy = 100;
                ColonyManager.Singleton.Biofuel = 100;
                ColonyManager.Singleton.Food = 100;
                ColonyManager.Singleton.Honey = 100;
            }
            else
            {
                LoggingUtils.LogWarning("ColonyManager не инициализирован, не удалось настроить ресурсы для отладки", this);
            }

            // TODO: Добавить создание отладочных построек
        }
        catch (Exception ex)
        {
            LoggingUtils.LogException(ex, "Ошибка при создании игры в режиме отладки", this);
        }
    }

    /// <summary>
    /// Применяет настройки для режима отладки
    /// </summary>
    private void ApplyDebugSettings()
    {
        try
        {
            LoggingUtils.LogInfo("Применение настроек отладки", this);

            // Ограничиваем частоту кадров для экономии ресурсов
            Application.targetFrameRate = 20;

            // Отключаем визуальные эффекты для повышения производительности
            DisableVisualEffects();
        }
        catch (Exception ex)
        {
            LoggingUtils.LogWarning($"Не удалось применить настройки отладки: {ex.Message}", this);
        }
    }

    /// <summary>
    /// Отключает визуальные эффекты для повышения производительности в режиме отладки
    /// </summary>
    private void DisableVisualEffects()
    {
        try
        {
            // Отключаем снег и другие эффекты частиц
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                ParticleSystem[] particleSystems = mainCamera.GetComponentsInChildren<ParticleSystem>();
                foreach (ParticleSystem ps in particleSystems)
                {
                    ps.gameObject.SetActive(false);
                }
            }
        }
        catch (Exception ex)
        {
            LoggingUtils.LogWarning($"Не удалось отключить визуальные эффекты: {ex.Message}", this);
        }
    }

    /// <summary>
    /// Загружает игру, используя зарегистрированные стратегии
    /// Реализует паттерн Template Method
    /// </summary>
    private void LoadGame()
    {
        try
        {
            LoggingUtils.LogInfo("Начало загрузки игры", this);

            // Уведомляем о начале загрузки
            OnBeforeLoad?.Invoke();

            // Применяем все стратегии загрузки
            foreach (ISaveLoadStrategy strategy in _saveLoadStrategies)
            {
                try
                {
                    strategy.Load();
                }
                catch (Exception ex)
                {
                    LoggingUtils.LogException(ex, $"Ошибка при загрузке данных с использованием {strategy.GetType().Name}", this);
                }
            }

            // Создаем карту для путешествий
            if (TravelingManager.Singleton != null && Player.Instance != null)
            {
                TravelingManager.Singleton.CreateNewMap(Player.Instance.Seed);
            }

            // Запускаем первый квест, если это первый запуск игры
            if (_isFirstBoot && QuestSystem.Singleton != null)
            {
                QuestSystem.Singleton.StartFirst();
            }

            // Уведомляем о завершении загрузки
            OnLoadCompleted?.Invoke();

            LoggingUtils.LogInfo("Игра успешно загружена", this);
        }
        catch (Exception ex)
        {
            LoggingUtils.LogException(ex, "Критическая ошибка при загрузке игры", this);
        }
    }

    /// <summary>
    /// Сохраняет игру, используя зарегистрированные стратегии
    /// Метод публичный, чтобы игру можно было сохранять из любого места кода
    /// </summary>
    public void SaveGame()
    {
        try
        {
            LoggingUtils.LogInfo("Начало сохранения игры", this);

            // Проверяем доступность SystemSaver
            if (_systemSaver == null)
            {
                _systemSaver = GetComponent<SystemSaver>();
                if (_systemSaver == null)
                {
                    throw new InvalidOperationException("SystemSaver не найден");
                }
            }

            // Уведомляем о начале сохранения
            OnBeforeSave?.Invoke();

            // Применяем все стратегии сохранения
            foreach (ISaveLoadStrategy strategy in _saveLoadStrategies)
            {
                try
                {
                    strategy.Save();
                }
                catch (Exception ex)
                {
                    LoggingUtils.LogException(ex, $"Ошибка при сохранении данных с использованием {strategy.GetType().Name}", this);
                }
            }

            // Сохраняем игру через SystemSaver
            _systemSaver.SaveGame();

            // Уведомляем о завершении сохранения
            OnSaveCompleted?.Invoke();

            LoggingUtils.LogInfo("Игра успешно сохранена", this);
        }
        catch (Exception ex)
        {
            LoggingUtils.LogException(ex, "Критическая ошибка при сохранении игры", this);
            ShowSaveErrorMessage();
        }
    }

    /// <summary>
    /// Показывает сообщение об ошибке сохранения
    /// </summary>
    private void ShowSaveErrorMessage()
    {
        if (AlertsManager.Singleton != null)
        {
            AlertsManager.Singleton.ShowAlert("Не удалось сохранить игру. Пожалуйста, проверьте свободное место на устройстве.");
        }
    }


    #region Создание новой игры

    /// <summary>
    /// Создает новую игру, инициализируя все необходимые компоненты
    /// </summary>
    private void CreateNewGame()
    {
        try
        {
            LoggingUtils.LogInfo("Создание новой игры", this);

            // Применяем паттерн Template Method для последовательного создания игровых элементов
            foreach (ISaveLoadStrategy strategy in _saveLoadStrategies)
            {
                try
                {
                    strategy.Create();
                }
                catch (Exception ex)
                {
                    LoggingUtils.LogException(ex, $"Ошибка при создании данных с использованием {strategy.GetType().Name}", this);
                }
            }

            // Устанавливаем флаг первого запуска
            _isFirstBoot = true;

            LoggingUtils.LogInfo("Новая игра успешно создана", this);
        }
        catch (Exception ex)
        {
            LoggingUtils.LogException(ex, "Критическая ошибка при создании новой игры", this);
            ShowInitializationErrorMessage();
        }
    }

    #endregion

    #region Стратегии сохранения/загрузки

    /// <summary>
    /// Интерфейс для стратегий сохранения/загрузки данных
    /// Реализует паттерн Strategy
    /// </summary>
    public interface ISaveLoadStrategy
    {
        /// <summary>
        /// Создает новые данные
        /// </summary>
        void Create();

        /// <summary>
        /// Загружает данные
        /// </summary>
        void Load();

        /// <summary>
        /// Сохраняет данные
        /// </summary>
        void Save();
    }

    /// <summary>
    /// Базовый класс для стратегий сохранения/загрузки
    /// </summary>
    private abstract class BaseSaveLoadStrategy : ISaveLoadStrategy
    {
        protected SaveAndLoad SaveAndLoadInstance { get; }

        protected BaseSaveLoadStrategy(SaveAndLoad saveAndLoad)
        {
            SaveAndLoadInstance = saveAndLoad;
        }

        /// <summary>
        /// Шаблонный метод для создания данных
        /// </summary>
        public virtual void Create()
        {
            try
            {
                LoggingUtils.LogInfo($"Создание данных с использованием {GetType().Name}");
                CreateInternal();
            }
            catch (Exception ex)
            {
                LoggingUtils.LogException(ex, $"Ошибка при создании данных в {GetType().Name}");
                throw;
            }
        }

        /// <summary>
        /// Шаблонный метод для загрузки данных
        /// </summary>
        public virtual void Load()
        {
            try
            {
                LoggingUtils.LogInfo($"Загрузка данных с использованием {GetType().Name}");
                LoadInternal();
            }
            catch (Exception ex)
            {
                LoggingUtils.LogException(ex, $"Ошибка при загрузке данных в {GetType().Name}");
                throw;
            }
        }

        /// <summary>
        /// Шаблонный метод для сохранения данных
        /// </summary>
        public virtual void Save()
        {
            try
            {
                LoggingUtils.LogInfo($"Сохранение данных с использованием {GetType().Name}");
                SaveInternal();
            }
            catch (Exception ex)
            {
                LoggingUtils.LogException(ex, $"Ошибка при сохранении данных в {GetType().Name}");
                throw;
            }
        }

        /// <summary>
        /// Конкретная реализация создания данных
        /// </summary>
        protected abstract void CreateInternal();

        /// <summary>
        /// Конкретная реализация загрузки данных
        /// </summary>
        protected abstract void LoadInternal();

        /// <summary>
        /// Конкретная реализация сохранения данных
        /// </summary>
        protected abstract void SaveInternal();
    }

    /// <summary>
    /// Стратегия сохранения/загрузки медведей
    /// </summary>
    private class BearsSaveLoadStrategy : BaseSaveLoadStrategy
    {
        public BearsSaveLoadStrategy(SaveAndLoad saveAndLoad) : base(saveAndLoad) { }

        protected override void CreateInternal()
        {
            try
            {
                ColonyManager colonyManager = SaveAndLoadInstance.GetComponent<ColonyManager>();
                if (colonyManager == null) throw new InvalidOperationException("ColonyManager не найден");

                // Создаем специального медведя Хром
                GameObject chromBear = GameObject.Find("BearChrom_0");
                if (chromBear != null)
                {
                    colonyManager.GenerateSpecialBear("Хром", chromBear, Traditions.Chrom);
                }
                else
                {
                    LoggingUtils.LogWarning("Объект BearChrom_0 не найден для создания специального медведя");
                }

                // Создаем медведей разных профессий
                colonyManager.GenerateNewBear(Traditions.Beekeepers);
                colonyManager.GenerateNewBear(Traditions.Programmers);
                colonyManager.GenerateNewBear(Traditions.Constructors);
                colonyManager.GenerateNewBear(Traditions.BioEngineers);

                // Создаем дронов
                for (int i = 0; i < 4; i++)
                {
                    colonyManager.GenerateNewBear(Traditions.Drone);
                }

                LoggingUtils.LogInfo("Медведи успешно созданы");
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось создать медведей", ex);
            }
        }

        protected override void LoadInternal()
        {
            try
            {
                if (SystemSaver.Singleton == null || SystemSaver.Singleton.gameSave == null)
                {
                    throw new InvalidOperationException("SystemSaver не инициализирован");
                }

                ColonyManager colonyManager = SaveAndLoadInstance.GetComponent<ColonyManager>();
                if (colonyManager == null) throw new InvalidOperationException("ColonyManager не найден");

                foreach (Bear bear in SystemSaver.Singleton.gameSave.bears)
                {
                    colonyManager.BearSpawn(bear);
                    bear.UpdateModifiers();
                }

                colonyManager.UpdateWorkersCount();
                LoggingUtils.LogInfo("Медведи успешно загружены");
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось загрузить медведей", ex);
            }
        }

        protected override void SaveInternal()
        {
            try
            {
                ColonyManager colonyManager = SaveAndLoadInstance.GetComponent<ColonyManager>();
                if (colonyManager == null) throw new InvalidOperationException("ColonyManager не найден");

                // Сохраняем данные о каждом медведе
                foreach (Bear bear in colonyManager.bearsInColony)
                {
                    if (bear.tradition != Traditions.Chrom)
                    {
                        // Сохраняем координаты медведя
                        GameObject bearObj = GameObject.Find(bear.gameName);
                        if (bearObj != null)
                        {
                            bear.x = bearObj.transform.position.x;
                            bear.z = bearObj.transform.position.z;
                        }
                    }
                }

                LoggingUtils.LogInfo("Медведи успешно сохранены");
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось сохранить медведей", ex);
            }
        }
    }

    /// <summary>
    /// Стратегия сохранения/загрузки построек
    /// </summary>
    private class BuildsSaveLoadStrategy : BaseSaveLoadStrategy
    {
        public BuildsSaveLoadStrategy(SaveAndLoad saveAndLoad) : base(saveAndLoad) { }

        protected override void CreateInternal()
        {
            try
            {
                if (BuildingSaveSystem.Singleton == null)
                {
                    throw new InvalidOperationException("BuildingSaveSystem не инициализирован");
                }

                BuildingSaveSystem.Singleton.CreateStartBuilds();
                LoggingUtils.LogInfo("Постройки успешно созданы");
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось создать постройки", ex);
            }
        }

        protected override void LoadInternal()
        {
            try
            {
                if (BuildingSaveSystem.Singleton == null)
                {
                    throw new InvalidOperationException("BuildingSaveSystem не инициализирован");
                }

                BuildingSaveSystem.Singleton.PlaceBuildFromSave();
                LoggingUtils.LogInfo("Постройки успешно загружены");
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось загрузить постройки", ex);
            }
        }

        protected override void SaveInternal()
        {
            // BuildingSaveSystem сам сохраняет свои данные через SystemSaver
            LoggingUtils.LogInfo("Постройки сохраняются через BuildingSaveSystem");
        }
    }

    /// <summary>
    /// Стратегия сохранения/загрузки настроек
    /// </summary>
    private class PreferenceSaveLoadStrategy : BaseSaveLoadStrategy
    {
        public PreferenceSaveLoadStrategy(SaveAndLoad saveAndLoad) : base(saveAndLoad) { }

        protected override void CreateInternal()
        {
            try
            {
                if (SystemSaver.Singleton == null || SystemSaver.Singleton.gameSave == null)
                {
                    throw new InvalidOperationException("SystemSaver не инициализирован");
                }

                // Устанавливаем значения настроек по умолчанию
                SystemSaver.Singleton.gameSave.PreferenceSave.sensitivity = 5f;
                SystemSaver.Singleton.gameSave.PreferenceSave.globalVolume = 100f;
                SystemSaver.Singleton.gameSave.PreferenceSave.postProcessing = true;

                LoggingUtils.LogInfo("Настройки успешно созданы");
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось создать настройки", ex);
            }
        }

        protected override void LoadInternal()
        {
            try
            {
                if (SystemSaver.Singleton == null || SystemSaver.Singleton.gameSave == null)
                {
                    throw new InvalidOperationException("SystemSaver не инициализирован");
                }

                if (Preference.Singleton == null)
                {
                    throw new InvalidOperationException("Preference не инициализирован");
                }

                // Загружаем настройки из сохранения
                Preference.Singleton.globalVolume = SystemSaver.Singleton.gameSave.PreferenceSave.globalVolume;
                Preference.Singleton.sensitivityOfCamera = SystemSaver.Singleton.gameSave.PreferenceSave.sensitivity;
                Preference.Singleton.postProcessing = SystemSaver.Singleton.gameSave.PreferenceSave.postProcessing;

                LoggingUtils.LogInfo("Настройки успешно загружены");
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось загрузить настройки", ex);
            }
        }

        protected override void SaveInternal()
        {
            try
            {
                if (SystemSaver.Singleton == null || SystemSaver.Singleton.gameSave == null)
                {
                    throw new InvalidOperationException("SystemSaver не инициализирован");
                }

                if (Preference.Singleton == null)
                {
                    throw new InvalidOperationException("Preference не инициализирован");
                }

                // Сохраняем текущие настройки
                SystemSaver.Singleton.gameSave.PreferenceSave.globalVolume = Preference.Singleton.globalVolume;
                SystemSaver.Singleton.gameSave.PreferenceSave.sensitivity = Preference.Singleton.sensitivityOfCamera;
                SystemSaver.Singleton.gameSave.PreferenceSave.postProcessing = Preference.Singleton.postProcessing;

                LoggingUtils.LogInfo("Настройки успешно сохранены");
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось сохранить настройки", ex);
            }
        }
    }

    /// <summary>
    /// Стратегия сохранения/загрузки инвентаря
    /// </summary>
    private class InventorySaveLoadStrategy : BaseSaveLoadStrategy
    {
        public InventorySaveLoadStrategy(SaveAndLoad saveAndLoad) : base(saveAndLoad) { }

        protected override void CreateInternal()
        {
            try
            {
                if (Player.Instance == null)
                {
                    throw new InvalidOperationException("Player не инициализирован");
                }

                // Создаем начальный инвентарь
                Dictionary<string, int> initInventory = new()
                {
                    {"materials", 0},
                    {"food", 10},
                    {"bioFuel", 15},
                    {"honey", 0},
                    {"materialPlus", 0},
                    {"energy", 0},
                    {"maxMaterials", 100},
                    {"maxFood", 10},
                    {"maxBioFuel", 15},
                    {"maxHoney", 0},
                    {"maxBears", 10},
                    {"maxMaterialPlus", 0},
                    {"maxEnergy", 0},
                    {"seed", Player.Instance.Seed}
                };

                // Создаем игрока в API
                if (APIClient.Instance != null)
                {
                    APIClient.Instance.CreatePlayerRequest(Player.Instance.PlayerName, initInventory)
                        .ContinueWith(task =>
                        {
                            if (task.IsFaulted)
                            {
                                LoggingUtils.LogException(task.Exception, "Ошибка при создании инвентаря в API");
                            }
                        });
                }

                LoggingUtils.LogInfo("Инвентарь успешно создан");
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось создать инвентарь", ex);
            }
        }

        protected override async void LoadInternal()
        {
            try
            {
                if (Player.Instance == null || string.IsNullOrEmpty(Player.Instance.PlayerName))
                {
                    throw new InvalidOperationException("Player не инициализирован или имя не задано");
                }

                if (APIClient.Instance == null)
                {
                    throw new InvalidOperationException("APIClient не инициализирован");
                }

                // Загружаем инвентарь из API
                APIClient.UserInventory inventory = await APIClient.Instance.GetUserInventoryRequest(Player.Instance.PlayerName);

                if (inventory == null)
                {
                    LoggingUtils.LogWarning("Получен пустой инвентарь из API");
                    return;
                }

                if (ColonyManager.Singleton != null)
                {
                    ColonyManager.Singleton.SetInventory(inventory);
                    LoggingUtils.LogInfo($"Инвентарь игрока {Player.Instance.PlayerName} успешно загружен");
                }
                else
                {
                    throw new InvalidOperationException("ColonyManager не инициализирован");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось загрузить инвентарь", ex);
            }
        }

        protected override void SaveInternal()
        {
            // Инвентарь сохраняется через API и не требует дополнительных действий при сохранении
            LoggingUtils.LogInfo("Инвентарь сохраняется через API");
        }
    }

    /// <summary>
    /// Стратегия сохранения/загрузки игровых событий
    /// </summary>
    private class GameEventsSaveLoadStrategy : BaseSaveLoadStrategy
    {
        public GameEventsSaveLoadStrategy(SaveAndLoad saveAndLoad) : base(saveAndLoad) { }

        protected override void CreateInternal()
        {
            try
            {
                if (GameEventsManager.Singleton == null)
                {
                    throw new InvalidOperationException("GameEventsManager не инициализирован");
                }

                // Устанавливаем начальные значения времени и температуры
                GameEventsManager.Singleton.worldHours = 0;
                GameEventsManager.Singleton.worldMinuts = 0;
                GameEventsManager.Singleton.WorldTemperature = 0f;

                LoggingUtils.LogInfo("Игровые события успешно инициализированы");
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось инициализировать игровые события", ex);
            }
        }

        protected override void LoadInternal()
        {
            try
            {
                if (SystemSaver.Singleton == null || SystemSaver.Singleton.gameSave == null)
                {
                    throw new InvalidOperationException("SystemSaver не инициализирован");
                }

                if (GameEventsManager.Singleton == null)
                {
                    throw new InvalidOperationException("GameEventsManager не инициализирован");
                }

                // Загружаем время и температуру из сохранения
                GameEventsManager.Singleton.WorldTemperature = SystemSaver.Singleton.gameSave.temperature;
                GameEventsManager.Singleton.worldHours = SystemSaver.Singleton.gameSave.hours;
                GameEventsManager.Singleton.worldMinuts = SystemSaver.Singleton.gameSave.minutes;

                LoggingUtils.LogInfo("Игровые события успешно загружены");
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось загрузить игровые события", ex);
            }
        }

        protected override void SaveInternal()
        {
            try
            {
                if (SystemSaver.Singleton == null || SystemSaver.Singleton.gameSave == null)
                {
                    throw new InvalidOperationException("SystemSaver не инициализирован");
                }

                if (GameEventsManager.Singleton == null)
                {
                    throw new InvalidOperationException("GameEventsManager не инициализирован");
                }

                // Сохраняем текущее время и температуру
                SystemSaver.Singleton.gameSave.temperature = GameEventsManager.Singleton.WorldTemperature;
                SystemSaver.Singleton.gameSave.hours = GameEventsManager.Singleton.worldHours;
                SystemSaver.Singleton.gameSave.minutes = GameEventsManager.Singleton.worldMinuts;

                LoggingUtils.LogInfo("Игровые события успешно сохранены");
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось сохранить игровые события", ex);
            }
        }
    }

    #endregion

    # region load

    /// <summary>
    /// Загрузит медведей из json'а
    /// </summary>
    private void LoadBears()
    {
        SystemSaver systemSaver = gameObject.GetComponent<SystemSaver>();
        ColonyManager colonyManager = gameObject.GetComponent<ColonyManager>();

        foreach (Bear bear in systemSaver.gameSave.bears)
        {
            colonyManager.BearSpawn(bear);
            bear.UpdateModifiers();
        }
        colonyManager.UpdateWorkersCount();
    }

    /// <summary>
    /// Подргрузка тасков из json
    /// </summary>
    [Obsolete("Таски больше не сохраняются")]
    private void LoadTasks()
    {
        SystemSaver systemSaver = gameObject.GetComponent<SystemSaver>();
        ColonyManager colonyManager = gameObject.GetComponent<ColonyManager>();

        // Проходимся по всем сохраненным таскам
        foreach (Dictionary<string, object> task in systemSaver.gameSave.tasksSaves)
        {
            // Инициализируем все нужные переменные для создания задачи
            TasksMode tasksMode = (TasksMode)Enum.Parse(typeof(TasksMode), (string)task["taskMode"]);
            GameObject objectOfTask = GameObject.Find((string)task["ObjectOfTaskName"]);
            Bear bearObject = null;

            foreach (Bear bear in colonyManager.bearsInColony) // Ищем медведя
            {
                if (bear.gameName == (string)task["BearGameName"])
                    bearObject = bear;
            }

            if (bearObject == null) // Если медведь не найден
                throw new ArgumentException("Invalid bear game name: " + task["BearGameName"]);

            // Инициализируем задачу
            BearTask bearTask =
                new BearTask(tasksMode, objectOfTask, Traditions.None, (float)(double)task["needSteps"]);
            bearTask.selectedBear = bearObject;
            bearTask.totalSteps = (float)(double)task["totalSteps"];
        }
    }


    /// <summary>
    /// Загрузить постройки из json
    /// </summary>
    private void LoadBuilds()
    {
        BuildingSaveSystem buildingSaveSystem = gameObject.GetComponent<BuildingSaveSystem>();
        buildingSaveSystem.PlaceBuildFromSave();
    }


    /// <summary>
    /// Загрузить настройки из json
    /// </summary>
    private void LoadPreference()
    {
        Preference.Singleton.globalVolume = SystemSaver.Singleton.gameSave.PreferenceSave.globalVolume;
        Preference.Singleton.sensitivityOfCamera = SystemSaver.Singleton.gameSave.PreferenceSave.sensitivity;
        Preference.Singleton.postProcessing = SystemSaver.Singleton.gameSave.PreferenceSave.postProcessing;
    }


    /// <summary>
    /// Загружает инвентарь игрока из API
    /// </summary>
    /// <returns>Задача загрузки инвентаря</returns>
    private async Task LoadInventory()
    {
        try
        {
            if (Player.Instance == null || string.IsNullOrEmpty(Player.Instance.playerName))
            {
                Debug.LogError("Не удалось загрузить инвентарь: имя игрока не задано");
                return;
            }

            APIClient.UserInventory inventory =
                await APIClient.Instance.GetUserInventoryRequest(Player.Instance.playerName);

            if (inventory == null)
            {
                Debug.LogWarning("Получен пустой инвентарь из API");
                return;
            }

            ColonyManager.Singleton.SetInventory(inventory);
            Debug.Log($"Инвентарь игрока {Player.Instance.playerName} успешно загружен");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Ошибка при загрузке инвентаря: {ex.Message}");
        }
    }

    /// <summary>
    /// Загрузка GameEventsManager (время и температура)
    /// </summary>
    private void LoadGameEvents()
    {
        GameEventsManager.Singleton.WorldTemperature = SystemSaver.Singleton.gameSave.temperature;
        GameEventsManager.Singleton.worldHours = SystemSaver.Singleton.gameSave.hours;
        GameEventsManager.Singleton.worldMinuts = SystemSaver.Singleton.gameSave.minutes;
    }

    #endregion

    #region save

    /// <summary>
    /// Сохранить медведей, сведения о них, например позиции
    /// </summary>
    private void SaveBears()
    {
        ColonyManager colonyManager = gameObject.GetComponent<ColonyManager>();
        // Рассчитывается на то, что каждый медведь из bearsInColony, соответствует индексу в json 

        for (int i = 0; i < colonyManager.bearsInColony.Count; i++)

            // Получаем медведей медведя и сейв медведя из списков
            foreach (Bear bear in colonyManager.bearsInColony)
            {
                if (bear.tradition != Traditions.Chrom)
                {
                    // Сейвим координаты
                    GameObject bearObj = GameObject.Find(bear.gameName);
                    bear.x = bearObj.transform.position.x;
                    bear.z = bearObj.transform.position.z;
                }
            }
    }

    /// <summary>
    /// Сохранить задачи медведя в json
    /// </summary>
    [Obsolete("Таски больше не сохраняются")]
    private void SaveTasks()
    {
        ColonyManager colonyManager = gameObject.GetComponent<ColonyManager>();
        SystemSaver systemSaver = gameObject.GetComponent<SystemSaver>();

        if (colonyManager.bearTasks.Count == 0) return;

        Type type = colonyManager.bearTasks[0].GetType();
        foreach (BearTask bearTask in colonyManager.bearTasks) // Проходимся по всем медведям
        {
            Dictionary<string, object> saveData = new Dictionary<string, object>();
            // Проходимся по всем публичным филдам, чтобы найти помеченные аттрибутом
            foreach (var field in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                JsonPropertyAttribute attribute = field.GetCustomAttribute<JsonPropertyAttribute>();
                if (attribute != null) // Если у поля есть такой аттрибут, достаем из него значение
                {
                    saveData[attribute.PropertyName] = field.GetValue(bearTask);
                }
            }

            systemSaver.gameSave.tasksSaves.Add(saveData);
        }
    }

    #endregion
}