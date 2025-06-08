using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Alerts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Utils;

/// <summary>
/// Предоставляет удобный фасад для доступа к API-интерфейсу игры.
/// Реализует паттерны Singleton (для глобального доступа) и Facade (для упрощения взаимодействия с API).
/// </summary>
[SuppressMessage("ReSharper", "MissingXmlDoc")] // I suppress xmldocwarn because many fields don't need it
public class APIClient : MonoBehaviour
{
    #region Константы и настройки

    private const float DEFAULT_REQUEST_TIMEOUT = 10f;
    private const int PING_INTERVAL_MS = 5000;
    private const string CONTENT_TYPE_JSON = "application/json";

    #endregion

    #region Приватные поля

    /// <summary>
    /// UUID игры из конфигурации
    /// </summary>
    private string _uuid;

    /// <summary>
    /// Базовый URL для всех запросов к API
    /// </summary>
    private string _baseUri;

    /// <summary>
    /// Хост API-сервера
    /// </summary>
    [SerializeField] private string _host = "final.2025.nti-gamedev.ru";
    //private string _host = "gamedev.justiks.ru";

    /// <summary>
    /// Токен для отмены длительных операций
    /// </summary>
    private CancellationTokenSource _pingCancellationTokenSource;

    /// <summary>
    /// Флаг, указывающий на доступность сети
    /// </summary>
    private bool _isNetworkAvailable = true;

    #endregion

    #region Singleton реализация

    private static APIClient _instance;
    private static readonly object _lock = new object();

    /// <summary>
    /// Экземпляр класса APIClient (Singleton паттерн)
    /// Потокобезопасная реализация
    /// </summary>
    public static APIClient Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                Debug.LogWarning("[Singleton] Экземпляр APIClient уже уничтожен при выходе из приложения.");
                return null;
            }

            lock (_lock)
            {
                return _instance;
            }
        }
    }

    /// <summary>
    /// Флаг, указывающий на завершение работы приложения
    /// Предотвращает обращения к уничтоженному объекту
    /// </summary>
    private static bool applicationIsQuitting = false;

    /// <summary>
    /// Инициализация Singleton при создании объекта
    /// </summary>
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            StartInitialization();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// При уничтожении объекта устанавливаем флаг завершения приложения
    /// </summary>
    private void OnDestroy()
    {
        if (_instance == this)
        {
            StopPingServer();
            applicationIsQuitting = true;
        }
    }

    /// <summary>
    /// Запускает асинхронную инициализацию API клиента
    /// </summary>
    private void StartInitialization()
    {
        try
        {
            InitializeAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    LoggingUtils.LogException(task.Exception, "Ошибка при инициализации APIClient", this);
                    _isNetworkAvailable = false;
                }
            });
        }
        catch (Exception ex)
        {
            LoggingUtils.LogException(ex, "Критическая ошибка при инициализации APIClient", this);
        }
    }

    /// <summary>
    /// Асинхронная инициализация API клиента
    /// </summary>
    private async Task InitializeAsync()
    {
        try
        {
            // Проверяем доступность ConfigManager
            if (Config.ConfigManager.Instance == null || Config.ConfigManager.Instance.config == null)
            {
                throw new InvalidOperationException("ConfigManager не инициализирован или отсутствует конфигурация");
            }

            // Получаем API ключ из конфигурации
            _uuid = Config.ConfigManager.Instance.config.api_key;
            if (string.IsNullOrEmpty(_uuid))
            {
                throw new InvalidOperationException("API ключ не задан в конфигурации");
            }

            // Формируем базовый URL
            _baseUri = $"https://{_host}/api/games/{_uuid}/";

            // Запускаем пинг сервера
            StartPingServer();

            // Проверяем соединение с сервером
            _isNetworkAvailable = await NetworkUtils.CheckInternetConnection(_baseUri, DEFAULT_REQUEST_TIMEOUT);
            if (!_isNetworkAvailable)
            {
                Debug.LogWarning("Не удалось установить соединение с сервером API");
            }
        }
        catch (Exception ex)
        {
            _isNetworkAvailable = false;
            throw new InvalidOperationException($"Ошибка инициализации APIClient: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Запускает задачу пинга сервера
    /// </summary>
    private void StartPingServer()
    {
        // Останавливаем предыдущую задачу пинга, если она была запущена
        StopPingServer();

        // Создаем новый токен отмены
        _pingCancellationTokenSource = new CancellationTokenSource();

        // Запускаем пинг в отдельной задаче
        _ = PingServerAsync(_pingCancellationTokenSource.Token);
    }

    /// <summary>
    /// Останавливает задачу пинга сервера
    /// </summary>
    private void StopPingServer()
    {
        if (_pingCancellationTokenSource != null)
        {
            if (!_pingCancellationTokenSource.IsCancellationRequested)
            {
                _pingCancellationTokenSource.Cancel();
            }
            _pingCancellationTokenSource.Dispose();
            _pingCancellationTokenSource = null;
        }
    }

    /// <summary>
    /// Пингует сервер с заданным интервалом для проверки доступности
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    private async Task PingServerAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                bool isAvailable = await NetworkUtils.CheckInternetConnection(_baseUri, DEFAULT_REQUEST_TIMEOUT);

                // Если статус соединения изменился
                if (isAvailable != _isNetworkAvailable)
                {
                    _isNetworkAvailable = isAvailable;

                    if (!_isNetworkAvailable)
                    {
                        OnNetworkUnavailable();
                    }
                    else
                    {
                        OnNetworkRestored();
                    }
                }
            }
            catch (Exception ex)
            {
                LoggingUtils.LogError($"Ошибка при пинге сервера: {ex.Message}");
            }

            try
            {
                // Ждем указанный интервал перед следующим пингом
                await Task.Delay(PING_INTERVAL_MS, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                // Задача была отменена, прерываем цикл
                break;
            }
        }
    }

    /// <summary>
    /// Вызывается при обнаружении потери соединения с сервером
    /// </summary>
    private void OnNetworkUnavailable()
    {
        try
        {
            LoggingUtils.LogWarning("Обнаружена потеря соединения с сервером", this);

            // Сохраняем игровые данные перед возможным выходом в меню
            SaveGameProgress();

            // Уведомляем пользователя о проблеме
            ShowNetworkErrorAlert();

            // Можно добавить события для других компонентов игры
            // OnNetworkStatusChanged?.Invoke(false);
        }
        catch (Exception ex)
        {
            LoggingUtils.LogException(ex, "Ошибка в обработчике потери соединения", this);
        }
    }

    /// <summary>
    /// Вызывается при восстановлении соединения с сервером
    /// </summary>
    private void OnNetworkRestored()
    {
        try
        {
            LoggingUtils.LogInfo("Соединение с сервером восстановлено", this);

            // Можно добавить логику синхронизации данных с сервером
            // SyncDataWithServer();

            // Можно добавить события для других компонентов игры
            // OnNetworkStatusChanged?.Invoke(true);

            // Уведомляем пользователя о восстановлении соединения
            AlertsManager.Singleton?.ShowAlert("Соединение с сервером восстановлено");
        }
        catch (Exception ex)
        {
            LoggingUtils.LogException(ex, "Ошибка в обработчике восстановления соединения", this);
        }
    }

    /// <summary>
    /// Сохраняет прогресс игры
    /// </summary>
    private void SaveGameProgress()
    {
        try
        {
            SaveAndLoad saveAndLoad = GetComponent<SaveAndLoad>();
            if (saveAndLoad != null)
            {
                saveAndLoad.SaveGame();
                LoggingUtils.LogInfo("Игровой прогресс сохранен из-за проблем с сетью", this);
            }
            else
            {
                LoggingUtils.LogWarning("Не удалось найти компонент SaveAndLoad для сохранения", this);
            }
        }
        catch (Exception ex)
        {
            LoggingUtils.LogException(ex, "Не удалось сохранить прогресс игры", this);
        }
    }

    /// <summary>
    /// Показывает уведомление об ошибке сети
    /// </summary>
    private void ShowNetworkErrorAlert()
    {
        try
        {
            // Проверяем существование AlertsManager
            if (AlertsManager.Singleton != null)
            {
                AlertsManager.Singleton.ShowAlert("Нет подключения к интернету!");

                // Возвращаемся в меню только если не находимся там
                if (SceneManager.GetActiveScene().name != "Menu")
                {
                    SceneManager.LoadScene("Menu");
                }
            }
            else
            {
                Debug.LogError("AlertsManager не инициализирован, не удалось показать уведомление");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Ошибка при отображении уведомления о сетевой ошибке: {ex.Message}");
        }
    }

    #region DTO (Data Transfer Objects)
    // Классы для передачи данных между клиентом и сервером

    /// <summary>
    /// Базовый интерфейс для всех DTO
    /// </summary>
    public interface IDto
    {
        /// <summary>
        /// Проверяет валидность объекта DTO
        /// </summary>
        /// <returns>True, если объект валиден</returns>
        bool IsValid();
    }

    #region Inventory

    /// <summary>
    /// Представляет инвентарь пользователя
    /// </summary>
    public class UserInventory : IDto
    {
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Словарь ресурсов пользователя
        /// </summary>
        public Dictionary<string, int> Resources { get; }

        /// <summary>
        /// Создает новый экземпляр инвентаря пользователя
        /// </summary>
        /// <param name="name">Имя пользователя</param>
        /// <param name="resources">Ресурсы пользователя</param>
        /// <param name="seed">Значение seed (не используется в текущей версии)</param>
        public UserInventory(string name, Dictionary<string, int> resources, int seed = 0)
        {
            Name = name ?? string.Empty;
            Resources = resources ?? new Dictionary<string, int>();
        }

        /// <summary>
        /// Проверяет валидность объекта
        /// </summary>
        public bool IsValid() => !string.IsNullOrEmpty(Name) && Resources != null;

        /// <summary>
        /// Проверяет наличие ресурса в инвентаре
        /// </summary>
        /// <param name="resourceName">Название ресурса</param>
        public bool HasResource(string resourceName) => 
            !string.IsNullOrEmpty(resourceName) && 
            Resources != null && 
            Resources.ContainsKey(resourceName);

        /// <summary>
        /// Получает количество ресурса из инвентаря
        /// </summary>
        /// <param name="resourceName">Название ресурса</param>
        /// <param name="defaultValue">Значение по умолчанию, если ресурс не найден</param>
        /// <returns>Количество ресурса или значение по умолчанию</returns>
        public int GetResourceAmount(string resourceName, int defaultValue = 0) =>
            HasResource(resourceName) ? Resources[resourceName] : defaultValue;
    }

    /// <summary>
    /// Построитель для UserInventory с использованием паттерна Builder
    /// </summary>
    public class UserInventoryBuilder
    {
        private string _name;
        private Dictionary<string, int> _resources = new Dictionary<string, int>();
        private int _seed = 0;

        /// <summary>
        /// Устанавливает имя пользователя
        /// </summary>
        public UserInventoryBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        /// <summary>
        /// Добавляет ресурс в инвентарь
        /// </summary>
        public UserInventoryBuilder WithResource(string resourceName, int amount)
        {
            if (!string.IsNullOrEmpty(resourceName))
            {
                _resources[resourceName] = amount;
            }
            return this;
        }

        /// <summary>
        /// Устанавливает словарь ресурсов
        /// </summary>
        public UserInventoryBuilder WithResources(Dictionary<string, int> resources)
        {
            if (resources != null)
            {
                _resources = new Dictionary<string, int>(resources);
            }
            return this;
        }

        /// <summary>
        /// Устанавливает значение seed
        /// </summary>
        public UserInventoryBuilder WithSeed(int seed)
        {
            _seed = seed;
            return this;
        }

        /// <summary>
        /// Создает новый экземпляр UserInventory
        /// </summary>
        public UserInventory Build()
        {
            return new UserInventory(_name, _resources, _seed);
        }
    }

    #endregion

    #region Resources

    /// <summary>
    /// Представляет изменение ресурсов
    /// </summary>
    public class ChangeResources : IDto
    {
        /// <summary>
        /// Словарь изменений ресурсов
        /// </summary>
        [JsonProperty("resourcesChange")]
        public Dictionary<string, int> ResourcesChange { get; }

        /// <summary>
        /// Создает новый экземпляр изменения ресурсов
        /// </summary>
        /// <param name="resourcesChange">Словарь изменений ресурсов</param>
        public ChangeResources(Dictionary<string, int> resourcesChange)
        {
            ResourcesChange = resourcesChange ?? new Dictionary<string, int>();
        }

        /// <summary>
        /// Проверяет валидность объекта
        /// </summary>
        public bool IsValid() => ResourcesChange != null;
    }

    #endregion

    #region Logs

    /// <summary>
    /// Представляет лог изменения ресурсов игрока
    /// </summary>
    public class SendResourcesLog : IDto
    {
        /// <summary>
        /// Комментарий к изменению
        /// </summary>
        [JsonProperty("comment")]
        public string Comment { get; }

        /// <summary>
        /// Имя игрока
        /// </summary>
        [JsonProperty("playerName")]
        public string PlayerName { get; }

        /// <summary>
        /// Словарь изменений ресурсов
        /// </summary>
        [JsonProperty("resourcesChanged")]
        public Dictionary<string, int> ResourcesChanged { get; }

        /// <summary>
        /// Создает новый экземпляр лога изменения ресурсов
        /// </summary>
        /// <param name="comment">Комментарий</param>
        /// <param name="playerName">Имя игрока</param>
        /// <param name="resourcesChanged">Изменения ресурсов</param>
        public SendResourcesLog(string comment, string playerName, Dictionary<string, int> resourcesChanged)
        {
            Comment = comment ?? string.Empty;
            PlayerName = playerName ?? string.Empty;
            ResourcesChanged = resourcesChanged ?? new Dictionary<string, int>();
        }

        /// <summary>
        /// Проверяет валидность объекта
        /// </summary>
        public bool IsValid() => 
            !string.IsNullOrEmpty(PlayerName) && 
            ResourcesChanged != null;
    }

    /// <summary>
    /// Представляет лог ресурсов
    /// </summary>
    public class ResourcesLog : IDto
    {
        /// <summary>
        /// Комментарий к логу
        /// </summary>
        [JsonProperty("comment")]
        public string Comment { get; }

        /// <summary>
        /// Имя игрока
        /// </summary>
        [JsonProperty("playerName")]
        public string PlayerName { get; }

        /// <summary>
        /// Имя магазина
        /// </summary>
        [JsonProperty("shopName")]
        public string ShopName { get; }

        /// <summary>
        /// Словарь изменений ресурсов
        /// </summary>
        [JsonProperty("resourcesChanged")]
        public Dictionary<string, string> ResourcesChanged { get; }

        /// <summary>
        /// Создает новый экземпляр лога ресурсов
        /// </summary>
        /// <param name="comment">Комментарий</param>
        /// <param name="playerName">Имя игрока</param>
        /// <param name="shopName">Имя магазина</param>
        /// <param name="resourcesChanged">Изменения ресурсов</param>
        public ResourcesLog(string comment, string playerName, string shopName,
            Dictionary<string, string> resourcesChanged)
        {
            Comment = comment ?? string.Empty;
            PlayerName = playerName ?? string.Empty;
            ShopName = shopName ?? string.Empty;
            ResourcesChanged = resourcesChanged ?? new Dictionary<string, string>();
        }

        /// <summary>
        /// Проверяет валидность объекта
        /// </summary>
        public bool IsValid() => 
            !string.IsNullOrEmpty(PlayerName) && 
            !string.IsNullOrEmpty(ShopName) && 
            ResourcesChanged != null;
    }

    #endregion

    #region Shop

    /// <summary>
    /// Представляет информацию о магазине
    /// </summary>
    public class ShopInfo : IDto
    {
        /// <summary>
        /// Имя магазина
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; }

        /// <summary>
        /// Словарь ресурсов магазина
        /// </summary>
        [JsonProperty("resources")]
        public Dictionary<string, int> Resources { get; }

        /// <summary>
        /// Создает новый экземпляр информации о магазине
        /// </summary>
        /// <param name="name">Имя магазина</param>
        /// <param name="resources">Ресурсы магазина</param>
        public ShopInfo(string name, Dictionary<string, int> resources)
        {
            Name = name ?? string.Empty;
            Resources = resources ?? new Dictionary<string, int>();
        }

        /// <summary>
        /// Проверяет валидность объекта
        /// </summary>
        public bool IsValid() => !string.IsNullOrEmpty(Name) && Resources != null;

        /// <summary>
        /// Проверяет наличие ресурса в магазине
        /// </summary>
        /// <param name="resourceName">Название ресурса</param>
        public bool HasResource(string resourceName) => 
            !string.IsNullOrEmpty(resourceName) && 
            Resources != null && 
            Resources.ContainsKey(resourceName);
    }

    /// <summary>
    /// Представляет обновление ресурсов магазина
    /// </summary>
    public class ShopResourcesUpdate : IDto
    {
        /// <summary>
        /// Словарь ресурсов магазина
        /// </summary>
        [JsonProperty("resources")]
        public Dictionary<string, int> Resources { get; }

        /// <summary>
        /// Создает новый экземпляр обновления ресурсов магазина
        /// </summary>
        /// <param name="resources">Ресурсы магазина</param>
        public ShopResourcesUpdate(Dictionary<string, int> resources)
        {
            Resources = resources ?? new Dictionary<string, int>();
        }

        /// <summary>
        /// Проверяет валидность объекта
        /// </summary>
        public bool IsValid() => Resources != null;
    }

    /// <summary>
    /// Представляет логи магазина
    /// </summary>
    public class ShopLogs : IDto
    {
        /// <summary>
        /// Комментарий к логу
        /// </summary>
        [JsonProperty("comment")]
        public string Comment { get; }

        /// <summary>
        /// Имя игрока
        /// </summary>
        [JsonProperty("playerName")]
        public string PlayerName { get; }

        /// <summary>
        /// Имя магазина
        /// </summary>
        [JsonProperty("shopName")]
        public string ShopName { get; }

        /// <summary>
        /// Словарь изменений ресурсов
        /// </summary>
        [JsonProperty("resourcesChanged")]
        public Dictionary<string, int> ResourcesChanged { get; }

        /// <summary>
        /// Создает новый экземпляр лога магазина
        /// </summary>
        /// <param name="comment">Комментарий</param>
        /// <param name="playerName">Имя игрока</param>
        /// <param name="shopName">Имя магазина</param>
        /// <param name="resourcesChanged">Изменения ресурсов</param>
        public ShopLogs(string comment, string playerName, string shopName, Dictionary<string, int> resourcesChanged)
        {
            Comment = comment ?? string.Empty;
            PlayerName = playerName ?? string.Empty;
            ShopName = shopName ?? string.Empty;
            ResourcesChanged = resourcesChanged ?? new Dictionary<string, int>();
        }

        /// <summary>
        /// Проверяет валидность объекта
        /// </summary>
        public bool IsValid() => 
            !string.IsNullOrEmpty(PlayerName) && 
            !string.IsNullOrEmpty(ShopName) && 
            ResourcesChanged != null;
    }

    #endregion

    #endregion

    #region HTTP клиент

    /// <summary>
    /// Интерфейс для стратегий HTTP запросов
    /// Реализует паттерн Strategy для различных типов запросов
    /// </summary>
    /// <typeparam name="T">Тип возвращаемого результата</typeparam>
    private interface IHttpRequestStrategy<T> where T : class
    {
        /// <summary>
        /// Создает и настраивает объект запроса
        /// </summary>
        /// <param name="url">URL для запроса</param>
        /// <param name="requestBody">Тело запроса (может быть null)</param>
        /// <returns>Подготовленный объект запроса</returns>
        UnityWebRequest CreateRequest(string url, object requestBody);

        /// <summary>
        /// Обрабатывает результат запроса
        /// </summary>
        /// <param name="request">Выполненный запрос</param>
        /// <returns>Результат запроса</returns>
        Task<T> ProcessResponse(UnityWebRequest request);
    }

    /// <summary>
    /// Базовая реализация стратегии HTTP запросов
    /// </summary>
    /// <typeparam name="T">Тип возвращаемого результата</typeparam>
    private abstract class BaseHttpRequestStrategy<T> : IHttpRequestStrategy<T> where T : class
    {
        protected readonly string Method;

        protected BaseHttpRequestStrategy(string method)
        {
            Method = method;
        }

        /// <summary>
        /// Создает базовый запрос с общими настройками
        /// </summary>
        public virtual UnityWebRequest CreateRequest(string url, object requestBody)
        {
            var request = new UnityWebRequest(url, Method);
            request.downloadHandler = new DownloadHandlerBuffer();
            return request;
        }

        /// <summary>
        /// Обрабатывает ответ, проверяя на ошибки и десериализуя результат
        /// </summary>
        public virtual Task<T> ProcessResponse(UnityWebRequest request)
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    return Task.FromResult(JsonConvert.DeserializeObject<T>(request.downloadHandler.text));
                }
                catch (JsonException ex)
                {
                    string errorMessage = $"Ошибка десериализации: {ex.Message}";
                    LoggingUtils.LogException(ex, errorMessage);
                    throw new InvalidOperationException(errorMessage, ex);
                }
            }

            string httpErrorMessage = $"Ошибка HTTP запроса ({request.responseCode}): {request.error}\nОтвет: {request.downloadHandler?.text}";
            LoggingUtils.LogError(httpErrorMessage);
            throw new HttpRequestException(httpErrorMessage);
        }
    }

    /// <summary>
    /// Стратегия для GET запросов
    /// </summary>
    private class GetRequestStrategy<T> : BaseHttpRequestStrategy<T> where T : class
    {
        public GetRequestStrategy() : base("GET") { }
    }

    /// <summary>
    /// Стратегия для запросов с телом (POST, PUT)
    /// </summary>
    private class BodyRequestStrategy<T> : BaseHttpRequestStrategy<T> where T : class
    {
        public BodyRequestStrategy(string method) : base(method) { }

        public override UnityWebRequest CreateRequest(string url, object requestBody)
        {
            var request = base.CreateRequest(url, requestBody);

            if (requestBody != null)
            {
                string jsonString = JsonConvert.SerializeObject(requestBody);
                byte[] jsonInBytes = Encoding.UTF8.GetBytes(jsonString);
                request.uploadHandler = new UploadHandlerRaw(jsonInBytes);
                request.SetRequestHeader("Content-Type", CONTENT_TYPE_JSON);

                if (Method == "POST")
                {
                    LoggingUtils.LogInfo($"Отправка POST запроса на {url} с телом: {jsonString}");
                }
            }

            return request;
        }
    }

    /// <summary>
    /// Стратегия для POST запросов
    /// </summary>
    private class PostRequestStrategy<T> : BodyRequestStrategy<T> where T : class
    {
        public PostRequestStrategy() : base("POST") { }
    }

    /// <summary>
    /// Стратегия для PUT запросов
    /// </summary>
    private class PutRequestStrategy<T> : BodyRequestStrategy<T> where T : class
    {
        public PutRequestStrategy() : base("PUT") { }
    }

    /// <summary>
    /// Стратегия для DELETE запросов
    /// </summary>
    private class DeleteRequestStrategy : BaseHttpRequestStrategy<object>
    {
        public DeleteRequestStrategy() : base("DELETE") { }
    }

    /// <summary>
    /// Отправляет HTTP запрос с использованием соответствующей стратегии
    /// </summary>
    /// <typeparam name="T">Тип результата запроса</typeparam>
    /// <param name="strategy">Стратегия запроса</param>
    /// <param name="endpoint">Конечная точка API (без базового URL)</param>
    /// <param name="body">Тело запроса (для POST и PUT)</param>
    /// <param name="timeout">Таймаут запроса в секундах</param>
    /// <returns>Результат запроса или null в случае ошибки</returns>
    private async Task<T> SendRequestAsync<T>(IHttpRequestStrategy<T> strategy, string endpoint, object body = null, float timeout = DEFAULT_REQUEST_TIMEOUT) where T : class
    {
        if (string.IsNullOrEmpty(endpoint))
        {
            throw new ArgumentException("Endpoint не может быть пустым", nameof(endpoint));
        }

        if (!_isNetworkAvailable)
        {
            LoggingUtils.LogWarning("Попытка выполнить запрос при отсутствии соединения");
            OnNetworkUnavailable();
            return null;
        }

        string url = _baseUri + endpoint;

        try
        {
            using (UnityWebRequest request = strategy.CreateRequest(url, body))
            {
                request.timeout = (int)timeout;
                UnityWebRequestAsyncOperation operation = request.SendWebRequest();

                float startTime = Time.time;
                while (!operation.isDone)
                {
                    if (Time.time - startTime > timeout)
                    {
                        request.Abort();
                        throw new TimeoutException($"Запрос к {url} превысил таймаут {timeout} секунд");
                    }

                    await Task.Yield();
                }

                return await strategy.ProcessResponse(request);
            }
        }
        catch (HttpRequestException ex)
        {
            LoggingUtils.LogException(ex, $"HTTP ошибка при запросе к {url}");
            throw;
        }
        catch (TimeoutException ex)
        {
            LoggingUtils.LogException(ex, $"Таймаут запроса к {url}");
            _isNetworkAvailable = false;
            OnNetworkUnavailable();
            throw;
        }
        catch (Exception ex)
        {
            LoggingUtils.LogException(ex, $"Неожиданная ошибка при запросе к {url}");
            throw;
        }
    }

    /// <summary>
    /// Отправляет GET запрос
    /// </summary>
    /// <typeparam name="T">Тип результата</typeparam>
    /// <param name="endpoint">Конечная точка API</param>
    /// <param name="timeout">Таймаут в секундах</param>
    private async Task<T> SendGetAsync<T>(string endpoint, float timeout = DEFAULT_REQUEST_TIMEOUT) where T : class
    {
        return await SendRequestAsync(new GetRequestStrategy<T>(), endpoint, null, timeout);
    }

    /// <summary>
    /// Отправляет POST запрос
    /// </summary>
    /// <typeparam name="T">Тип результата</typeparam>
    /// <param name="endpoint">Конечная точка API</param>
    /// <param name="requestBody">Тело запроса</param>
    /// <param name="timeout">Таймаут в секундах</param>
    private async Task<T> SendPostAsync<T>(string endpoint, object requestBody, float timeout = DEFAULT_REQUEST_TIMEOUT) where T : class
    {
        return await SendRequestAsync(new PostRequestStrategy<T>(), endpoint, requestBody, timeout);
    }

    /// <summary>
    /// Отправляет PUT запрос
    /// </summary>
    /// <typeparam name="T">Тип результата</typeparam>
    /// <param name="endpoint">Конечная точка API</param>
    /// <param name="requestBody">Тело запроса</param>
    /// <param name="timeout">Таймаут в секундах</param>
    private async Task<T> SendPutAsync<T>(string endpoint, object requestBody, float timeout = DEFAULT_REQUEST_TIMEOUT) where T : class
    {
        return await SendRequestAsync(new PutRequestStrategy<T>(), endpoint, requestBody, timeout);
    }

    /// <summary>
    /// Отправляет DELETE запрос
    /// </summary>
    /// <param name="endpoint">Конечная точка API</param>
    /// <param name="timeout">Таймаут в секундах</param>
    private async Task SendDeleteAsync(string endpoint, float timeout = DEFAULT_REQUEST_TIMEOUT)
    {
        await SendRequestAsync(new DeleteRequestStrategy(), endpoint, null, timeout);
    }

    #endregion


    // Блок конкретных запросов
    // Для подробностей читай https://docs.google.com/document/d/1LhPn4tFzhWh_xtoy7fUtbEBHVIFKYIKzhLgfu84Tf-M/edit?tab=t.0
    /// <summary>
    /// Отправляет запрос на создание игрока (см. пункт 1)
    /// </summary>
    /// <param name="requestLogin">Логин игрока</param>
    /// <param name="requestResourses">Ресурсы игрока</param>
    /// <returns></returns>
    public async Task<UserInventory> CreatePlayerRequest(string requestLogin, Dictionary<string, int> requestResourses)
    {
        object requestBody;

        // Формируем тело запроса
        if (requestResourses == null)
            requestBody = new { name = requestLogin};

        else
            requestBody = new { name = requestLogin, resources = requestResourses};

        return await SendPostAsync<UserInventory>("players/", requestBody); // Отправляем запрос
    }

    /// <summary>
    /// Получить список всех пользователей
    /// </summary>
    /// <returns>Список пользователей либо пустой список</returns>
    public async Task<List<UserInventory>> GetUsersListRequest()
    {
        List<UserInventory> response = await SendGetAsync<List<UserInventory>>("players/");

        // Если ничего не нашли, вернем хотя бы пустой лист
        if (response == null)
        {
            return new List<UserInventory>();
        }

        return response;
    }

    /// <summary>
    /// Получить инвентарь игрока
    /// </summary>
    /// <param name="login">Логин игрока</param>
    /// <returns>Инвентарь игрока</returns>
    public async Task<UserInventory> GetUserInventoryRequest(string login)
    {
        return await SendGetAsync<UserInventory>("players/" + login + "/");
    }

    /// <summary>
    /// Обновление инвентаря на сервере
    /// </summary>
    /// <param name="login">никнейм игрока на сервере</param>
    /// <param name="inventory">инвентарь игрока</param>
    /// <returns>ChangeResoures record</returns>
    public async Task<ChangeResources> SetUserInventoryRequest(string login, Dictionary<string, int> inventory)
    {
        return await SendPutAsync<ChangeResources>("players/" + login + "/",
            new { name = login, resources = inventory });
    }

    /// <summary>
    /// Удаляет игрока
    /// </summary>
    /// <param name="login">никнейм игрока</param>
    /// <returns>null</returns>
    public async Task DeletePlayerRequest(string login)
    {
        await SendDeleteAsync("players/" + login);
    }

    /// <summary>
    /// Отправляет запрос на создание логов
    /// </summary>
    /// <param name="requestComment">Комментарий к логу</param>
    /// <param name="requestLogin">Запрашиваемый юзернейм</param>
    /// <param name="requestResources">Изменения в ивентаре</param>
    /// <returns>SendResoursesLog record</returns>
    public async Task<SendResourcesLog> CreateLogRequest(string requestComment, string requestLogin,
        Dictionary<string, string> requestResources)
    {
        return await SendPostAsync<SendResourcesLog>(
            "logs/",
            new
            {
                comment = requestComment,
                player_name = requestLogin,
                resources_changed = requestResources
            }
        );
    }

    /// <summary>
    /// читать логи
    /// </summary>
    /// <param name="requestLogin">Запрашиваемый юзернейм</param>
    /// <returns>Если есть логи, вернет логи, если нет логов, вернет пустой массив</returns>
    public async Task<List<SendResourcesLog>> ReadLogsRequest(string requestLogin)
    {
        List<SendResourcesLog> response =
            await SendGetAsync<List<SendResourcesLog>>("players/" + requestLogin + "/logs/");

        if (response == null)
        {
            return new List<SendResourcesLog>();
        }

        return response;
    }

    /// <summary>
    /// Создать магазин
    /// </summary>
    /// <param name="requestLogin">Юзернейм игрока</param>
    /// <param name="requestShopname">Название магазина</param>
    /// <param name="requestResourses">Ресурсы в магазине</param>
    /// <returns>ShopInfo record</returns>
    public async Task<ShopInfo> CreateShopRequest(string requestLogin, string requestShopname,
        Dictionary<string, byte> requestResourses)
    {
        object resoursesObject;

        if (requestResourses == null)
        {
            resoursesObject = new { name = requestShopname };
        }

        else
        {
            resoursesObject = new { name = requestShopname, resourses = requestResourses };
        }

        return await SendPostAsync<ShopInfo>("players/" + requestLogin + "/shops/", resoursesObject);
    }

    /// <summary>
    /// Получить список магазинов игрока
    /// </summary>
    /// <param name="requestLogin">Запрашиваемый логин</param>
    /// <returns>Лист из магазинов либо пустой лист</returns>
    public async Task<List<ShopInfo>> GetShopsRequest(string requestLogin)
    {
        List<ShopInfo> response = await SendGetAsync<List<ShopInfo>>("players/" + requestLogin + "/shops/");

        if (response == null)
        {
            return new List<ShopInfo>();
        }

        return response;
    }

    /// <summary>
    /// Получить ресурсы магазина
    /// </summary>
    /// <param name="login">Ник человека</param>
    /// <param name="shopname">Имя магазина</param>
    /// <returns>ShopInfo record</returns>
    public async Task<ShopInfo> GetShopResoursesRequest(string login, string shopname)
    {
        return await SendGetAsync<ShopInfo>("players/" + login + "/shops/" + shopname + "/");
    }

    /// <summary>
    /// Обновить ресурсы магазина
    /// </summary>
    /// <param name="login">Юзернейм игрока</param>
    /// <param name="shopname">название магазина</param>
    /// <param name="requestResourses">новые обновленные ресурсы</param>
    /// <returns>Ресурсы магазина</returns>
    public async Task<ShopResourcesUpdate> SetShopResoursesRequest(string login, string shopname,
        Dictionary<string, int> requestResourses)
    {
        return await SendPutAsync<ShopResourcesUpdate>("players/" + login + "/shops/" + shopname + "/",
            new { resourses = requestResourses });
    }

    /// <summary>
    /// Удалить магазин (Метод был не обязателен для реализации, но почему нет)
    /// </summary>
    /// <param name="login">логин игрока</param>
    /// <param name="shopname">название магазина</param>
    /// <returns>void</returns>
    public async Task DeleteShop(string login, string shopname)
    {
        await SendDeleteAsync("players/" + login + "/shops/" + shopname + "/");
    }

    /// <summary>
    /// Создать логи магазина
    /// </summary>
    /// <param name="requestComment">комментарий</param>
    /// <param name="requestLogin">юзернейм игрока</param>
    /// <param name="requestShopname">название магазина</param>
    /// <param name="requestResorses">изменения ресурсов магазина</param>
    /// <returns>отправленное тело</returns>
    public async Task<ShopLogs> CreateShopLogs(string requestComment, string requestLogin, string requestShopname,
        Dictionary<string, int> requestResorses)
    {
        return await SendPostAsync<ShopLogs>("logs/", new
        {
            comment = requestComment,
            player_name = requestLogin,
            shop_name = requestShopname,
            resources_changed = requestResorses
        });
    }

    /// <summary>
    /// Посмотреть логи магазина
    /// </summary>
    /// <param name="requestLogin">юзернейм игрока</param>
    /// <param name="requestShopname">название магазина</param>
    /// <returns>получить логи магазина</returns>
    public async Task<List<ShopLogs>> GetShopLogs(string requestLogin, string requestShopname)
    {
        return await SendGetAsync<List<ShopLogs>>("players/" + requestLogin + "/shops/" + requestShopname + "/logs/");
    }

    /// <summary>
    /// Посмотреть все логи магазинов внезависимости от игрока и тд
    /// </summary>
    /// <returns>получить логи игры магазина</returns>
    public async Task<List<ShopLogs>> GetGameLogs()
    {
        return await SendGetAsync<List<ShopLogs>>("logs/");
    }
}