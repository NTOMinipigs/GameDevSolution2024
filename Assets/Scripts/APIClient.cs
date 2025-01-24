using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

/// <summary>
///     Базируется на паттерне Singleton (Одиночка)
///     Предоставляет удобный доступ к API, берет на себя полную ответственность за маршрутизацию запросов
/// </summary>
public class APIClient : MonoBehaviour
{
    
    // HttpClient block

    /// <summary>
    /// UUID игры
    /// </summary>
    readonly string _uuid = Config.ConfigManager.Instance.config.api_key;

    /// <summary>
    /// Базовый урл для всех запросов
    /// </summary>
    private readonly string _baseUri;
    

    // Singleton block

    /// <summary>
    /// Инстанция объекта, часть Singleton паттерна
    /// </summary>
    public static APIClient Instance = new();

    /// <summary>
    /// Приватим конструктор, так как это требует паттерн
    /// </summary>
    private APIClient()
    {
        _baseUri = "https://2025.nti-gamedev.ru/api/games/" + _uuid + "/";
    }
    
    public void Update()
    {
        // Проверим подключение к интернету
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            IfNoInternetConnection();
        }
    }


    // records block
    // Все эти record'ы, это удобное представление Http ответов

    // 1. Инвентарь пользователя
    public class UserInventory
    {
        public string Name { get; }
        public Dictionary<string, int> Resources { get; }

        public UserInventory(string name, Dictionary<string, int> resources)
        {
            Name = name;
            Resources = resources;
        }
    }

    // 4. Resourses change
    public class ChangeResources
    {
        public Dictionary<string, int> ResourcesChange { get; }

        public ChangeResources(Dictionary<string, int> resourcesChange)
        {
            ResourcesChange = resourcesChange;
        }
    }

    // 5. send player logs
    public class SendResourcesLog
    {
        public string Comment { get; }
        public string PlayerName { get; }
        public Dictionary<string, int> ResourcesChanged { get; }

        public SendResourcesLog(string comment, string playerName, Dictionary<string, int> resourcesChanged)
        {
            Comment = comment;
            PlayerName = playerName;
            ResourcesChanged = resourcesChanged;
        }
    }

    // 6. player logs
    public class ResourcesLog
    {
        public string Comment { get; }
        public string PlayerName { get; }
        public string ShopName { get; }
        public Dictionary<string, string> ResourcesChanged { get; }

        public ResourcesLog(string comment, string playerName, string shopName, Dictionary<string, string> resourcesChanged)
        {
            Comment = comment;
            PlayerName = playerName;
            ShopName = shopName;
            ResourcesChanged = resourcesChanged;
        }
    }

    // 7, 8, 9. ShopInfo
    public class ShopInfo
    {
        public string Name { get; }
        public Dictionary<string, int> Resources { get; }

        public ShopInfo(string name, Dictionary<string, int> resources)
        {
            Name = name;
            Resources = resources;
        }
    }
    // 10. Shop resources after a transaction
    public class ShopResourcesUpdate
    {
        public Dictionary<string, int> Resources { get; }

        public ShopResourcesUpdate(Dictionary<string, int> resources)
        {
            Resources = resources;
        }
    }
    // Стракт для логов шопа
    public class ShopLogs
    {
        public string Comment { get; }
        public string PlayerName { get; }
        public string ShopName { get; }
        public Dictionary<string, int> ResourcesChanged { get; }

        public ShopLogs(string comment, string playerName, string shopName, Dictionary<string, int> resourcesChanged)
        {
            Comment = comment;
            PlayerName = playerName;
            ShopName = shopName;
            ResourcesChanged = resourcesChanged;
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    /// <summary>
    /// Обертка для UnityWebRequest в асинхронный метод.
    /// Используется как обертка во всех запросах кроме DELETE (Почему? Описано в документации метода SendDeleteAsync)
    /// Позволяет удобно завернуть запрос в awaitable и оставить часть формирования запроса на вызывающий метод
    /// </summary>
    /// <typeparam name="T">Класс в объект которого нужно сериализовать данные</typeparam>
    /// <param name="unityWebRequest">UnityWebRequest объект</param>
    /// <returns>null или сериализованный объект класса T</returns>
    /// <exception cref="HttpRequestException">В случае если запрос обвалился</exception>
    private async Task<T?> UnityWebRequestWrapper<T>(UnityWebRequest unityWebRequest) where T : class
    {
        UnityWebRequestAsyncOperation operation = unityWebRequest.SendWebRequest();
        while (!operation.isDone)
        {
            await Task.Yield();
        }

        if (unityWebRequest.result == UnityWebRequest.Result.Success)
        {
            return JsonConvert.DeserializeObject<T>(unityWebRequest.downloadHandler.text);
        }
        
        string errorMessage = "http request error: " + unityWebRequest.responseCode + " json: " + unityWebRequest.downloadHandler;
        Debug.LogError(errorMessage);
        throw new HttpRequestException(errorMessage);
    }

    /// <summary>
    /// Вызывается в случае, если есть подозрение на то, что у человека нет подключения к интернету в данный момент
    /// </summary>
    private void IfNoInternetConnection()
    {
        SaveAndLoad saveAndLoad = gameObject.GetComponent<SaveAndLoad>();
        saveAndLoad.SaveGame();
        SceneManager.LoadScene("Menu");
    }
    
    /// <summary>
    /// Отправить get запрос на сервер
    /// </summary>
    /// <typeparam name="T">Запись (record class) в который нужно сериализовать ответ</typeparam>
    /// <param name="uri">uri на который нужно отправить запрос</param>
    /// <returns>Сериализованный record class (T)</returns>
    private async Task<T> SendGetAsync<T>(string uri) where T : class
    {
        using (UnityWebRequest unityWebRequest = new UnityWebRequest(_baseUri + uri, "GET")) {
            unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
            return await UnityWebRequestWrapper<T>(unityWebRequest);
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    /// <summary>
    /// Post запрос на сервер
    /// </summary>
    /// <typeparam name="T">Запись (record class) в который нужно сериализовать ответ</typeparam>
    /// <param name="uri">uri на который нужно отправить запрос</param>
    /// <param name="requestBody">Данные отправляемые на сервер</param>
    /// <returns>Сериализованный json в формате record class</returns>
    private async Task<T> SendPostAsync<T>(string uri, object requestBody) where T : class
    {
        string jsonString = JsonConvert.SerializeObject(requestBody);
        Debug.Log(jsonString);
        using (UnityWebRequest unityWebRequest = new UnityWebRequest(_baseUri + uri, "POST"))
        {
            byte[] jsonInBytes = new UTF8Encoding().GetBytes(jsonString);
            unityWebRequest.uploadHandler = new UploadHandlerRaw(jsonInBytes);
            unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
            unityWebRequest.SetRequestHeader("Content-Type", "application/json");
            return await UnityWebRequestWrapper<T>(unityWebRequest);
        }
    }

    /// <summary>
    /// Put запрос на сервер
    /// </summary>
    /// <typeparam name="T">Запись (record class) в который нужно сериализовать ответ</typeparam>
    /// <param name="uri">uri на который нужно отправить запрос</param>
    /// <param name="requestBody">Данные отправляемые на сервер</param>
    /// <returns>Сериализованный json в формате record class</returns>
    private async Task<T> SendPutAsync<T>(string uri, object? requestBody) where T : class
    {
        string jsonString = JsonConvert.SerializeObject(requestBody);
        using (UnityWebRequest unityWebRequest = new UnityWebRequest(_baseUri + uri, "PUT"))
        {
            byte[] jsonInBytes = new UTF8Encoding().GetBytes(jsonString);
            unityWebRequest.uploadHandler = new UploadHandlerRaw(jsonInBytes);
            unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
            unityWebRequest.SetRequestHeader("Content-Type", "application/json");
            return await UnityWebRequestWrapper<T>(unityWebRequest);
        }
    }

    /// <summary>
    /// Delete запрос на сервер
    /// Здесь не используется враппер для UnityWebRequest т.к. здесь не возвращается никакое значение в случае успешного запроса, а в случае ошибки вызывается ошибка :pig:
    /// </summary>
    /// <param name="uri">url куда отправляется запрос</param>
    /// <returns>null</returns>
    private async Task SendDeleteAsync(string uri)
    {
        using (UnityWebRequest unityWebRequest = new UnityWebRequest(_baseUri + uri, "DELETE"))
        {
            UnityWebRequestAsyncOperation operation =  unityWebRequest.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (unityWebRequest.result != UnityWebRequest.Result.Success)
            {
                string errorMessage = "http request error: " + unityWebRequest.responseCode + " json: " + unityWebRequest.downloadHandler.text;
                Debug.LogError(errorMessage);
                throw new HttpRequestException(errorMessage);
            }
        }
    }


    // Блок конкретных запросов
    // Для подробностей читай https://docs.google.com/document/d/1LhPn4tFzhWh_xtoy7fUtbEBHVIFKYIKzhLgfu84Tf-M/edit?tab=t.0
    /// <summary>
    /// Отправляет запрос на создание игрока (см. пункт 1)
    /// </summary>
    /// <param name="requestLogin">Логин игрока</param>
    /// <param name="requestResourses">Ресурсы игрока</param>
    /// <returns></returns>
    public async Task<UserInventory?> CreatePlayerRequest(string requestLogin, Dictionary<string, int>? requestResourses)
    {
        object requestBody;

        // Формируем тело запроса
        if (requestResourses == null) {
            requestBody = new {name = requestLogin};
        }

        else
        {
            requestBody = new {name = requestLogin, resources = requestResourses};
        }

        return await SendPostAsync<UserInventory>("players/", requestBody); // Отправляем запрос
    }

    /// <summary>
    /// Получить список всех пользователей
    /// </summary>
    /// <returns>Список пользователей либо пустой список</returns>
    public async Task<List<UserInventory>> GetUsersListRequest()
    {
        List<UserInventory>? response = await SendGetAsync<List<UserInventory>>("players/");

        // Если ничего не нашли, вернем хотя бы пустой лист
        if (response == null) {
            return new List<UserInventory>();
        }

        return response;
    }

    /// <summary>
    /// Получить инвентарь игрока
    /// </summary>
    /// <param name="login">Логин игрока</param>
    /// <returns>Инвентарь игрока</returns>
    public async Task<UserInventory?> GetUserInventoryRequest(string login)
    {
        return await SendGetAsync<UserInventory>("players/" + login + "/");
    }
    
    /// <summary>
    /// Обновление инвентаря на сервере
    /// </summary>
    /// <param name="login">никнейм игрока на сервере</param>
    /// <param name="inventory">инвентарь игрока</param>
    /// <returns>ChangeResoures record</returns>
    public async Task<ChangeResources?> SetUserInventoryRequest(string login, Dictionary<string, int> inventory)
    {
        return await SendPutAsync<ChangeResources>("players/" + login + "/", new {name = login, resources = inventory});
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
    public async Task<SendResourcesLog?> CreateLogRequest(string requestComment, string requestLogin, Dictionary<string, string> requestResources)
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
        List<SendResourcesLog>? response = await SendGetAsync<List<SendResourcesLog>>("players/" + requestLogin + "/logs/");

        if (response == null) {
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
    public async Task<ShopInfo?> CreateShopRequest(string requestLogin, string requestShopname, Dictionary<string, byte>? requestResourses)
    {
        object resoursesObject;

        if (requestResourses == null)
        {
            resoursesObject = new {name = requestShopname};
        }

        else
        {
            resoursesObject = new {name = requestShopname, resourses = requestResourses};
        }

        return  await SendPostAsync<ShopInfo>("players/" + requestLogin + "/shops/", resoursesObject);
    }

    /// <summary>
    /// Получить список магазинов игрока
    /// </summary>
    /// <param name="requestLogin">Запрашиваемый логин</param>
    /// <returns>Лист из магазинов либо пустой лист</returns>
    public async Task<List<ShopInfo>> GetShopsRequest(string requestLogin)
    {
        List<ShopInfo>? response = await SendGetAsync<List<ShopInfo>>("players/" + requestLogin + "/shops/");

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
    public async Task<ShopInfo?> GetShopResoursesRequest(string login, string shopname)
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
    public async Task<ShopResourcesUpdate?> SetShopResoursesRequest(string login, string shopname, Dictionary<string, int> requestResourses)
    {
        return await SendPutAsync<ShopResourcesUpdate>("players/" + login + "/shops/" + shopname + "/", new {resourses = requestResourses});
    }

    /// <summary>
    /// Удалить магазин (Метод был не обязателен для реализации, но почему нет)
    /// </summary>
    /// <param name="login">логин игрока</param>
    /// <param name="shopname">название магазина</param>
    /// <returns>void</returns>
    public async Task DeleteShop(string login, string shopname) {
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
    public async Task<ShopLogs?> CreateShopLogs(string requestComment, string requestLogin, string requestShopname, Dictionary<string, int> requestResorses) {
        return await SendPostAsync<ShopLogs>("logs/", new {
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
    public async Task<List<ShopLogs>?> GetShopLogs(string requestLogin, string requestShopname) {
        return await SendGetAsync<List<ShopLogs>>("players/" + requestLogin + "/shops/" + requestShopname + "/logs/");
    }

    /// <summary>
    /// Посмотреть все логи магазинов внезависимости от игрока и тд
    /// </summary>
    /// <returns>получить логи игры магазина</returns>
    public async Task<List<ShopLogs>?> GetGameLogs() {
        return await SendGetAsync<List<ShopLogs>>("logs/");
    }
}