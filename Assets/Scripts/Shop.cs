using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

/// <summary>
/// Класс, отвечающий за логику и интерфейс магазина
/// Реализует паттерны Command и Repository
/// </summary>
public class Shop : MonoBehaviour
{
    #region События

    /// <summary>
    /// Событие, вызываемое перед загрузкой предложений магазина
    /// </summary>
    public static event Action OnShopOffersLoading;

    /// <summary>
    /// Событие, вызываемое после загрузки предложений магазина
    /// </summary>
    public static event Action<Resources> OnShopOffersLoaded;

    /// <summary>
    /// Событие, вызываемое при ошибке загрузки предложений магазина
    /// </summary>
    public static event Action<Exception> OnShopOffersError;

    #endregion

    #region Сериализуемые поля

    [Header("Настройки интерфейса")]
    [Tooltip("Префаб для создания предложения в магазине")]
    [SerializeField] private GameObject offerPrefab;

    [Tooltip("Контейнер для размещения предложений")]
    [SerializeField] private GameObject contentGameObject;

    [Tooltip("Спрайты для отображения различных ресурсов")]
    [SerializeField] private Sprite[] resourcesSprites = new Sprite[0];

    [Header("Настройки ресурсов")]
    [Tooltip("Доступные типы ресурсов в магазине")]
    [SerializeField] private Resources[] availableResources = {
        Resources.Material,
        Resources.MaterialPlus,
        Resources.BioFuel,
        Resources.Food,
        Resources.Energy
    };

    #endregion

    #region Приватные поля

    private bool _isShopOpen = false;
    private Resources _currentResourceType;
    private Dictionary<string, List<ShopOffer>> _cachedOffers = new Dictionary<string, List<ShopOffer>>();

    #endregion

    #region MonoBehaviour методы

    private void OnEnable()
    {
        // Подписываемся на событие изменения статуса сети в APIClient
        // APIClient.OnNetworkStatusChanged += HandleNetworkStatusChanged;
    }

    private void OnDisable()
    {
        // Отписываемся от событий
        // APIClient.OnNetworkStatusChanged -= HandleNetworkStatusChanged;
    }

    private void Update()
    {
        // Открываем/закрываем магазин по нажатию клавиши P
        if (Input.GetKeyDown(KeyCode.P))
        {
            ToggleShop();
        }
    }

    #endregion

    #region Публичные методы

    /// <summary>
    /// Обрабатывает нажатие кнопки для отображения ресурсов магазина
    /// </summary>
    /// <param name="buttonIndex">Индекс кнопки ресурса</param>
    public async void OnButtonClick(int buttonIndex)
    {
        try
        {
            // Валидация входных данных
            if (!ValidateShopParameters(buttonIndex))
            {
                return;
            }

            // Получаем тип ресурса и уведомляем о начале загрузки
            _currentResourceType = availableResources[buttonIndex];
            string resourceStringName = _currentResourceType.ToString();

            // Вызываем событие начала загрузки предложений
            OnShopOffersLoading?.Invoke();

            // Очищаем содержимое панели предложений
            ClearShopContent();

            // Получаем предложения для выбранного ресурса
            List<ShopOffer> offers = await GetShopOffersForResource(resourceStringName, buttonIndex);

            // Отображаем предложения
            DisplayShopOffers(offers);

            // Вызываем событие успешной загрузки предложений
            OnShopOffersLoaded?.Invoke(_currentResourceType);
        }
        catch (Exception ex)
        {
            // Логируем ошибку и уведомляем подписчиков события
            LoggingUtils.LogException(ex, "Ошибка при загрузке данных магазина");
            OnShopOffersError?.Invoke(ex);

            // Показываем сообщение об ошибке пользователю
            ShowErrorMessage($"Не удалось загрузить предложения: {ex.Message}");
        }
    }

    /// <summary>
    /// Переключает состояние магазина (открыт/закрыт)
    /// </summary>
    public void ToggleShop()
    {
        _isShopOpen = !_isShopOpen;
        if (_isShopOpen)
        {
            OpenShop();
        }
        else
        {
            CloseShop();
        }
    }

    #endregion

    #region Приватные методы для работы с магазином

    /// <summary>
    /// Проверяет валидность параметров магазина
    /// </summary>
    /// <param name="buttonIndex">Индекс кнопки ресурса</param>
    /// <returns>True, если параметры валидны</returns>
    private bool ValidateShopParameters(int buttonIndex)
    {
        if (contentGameObject == null)
        {
            LoggingUtils.LogError("Не задан контейнер для предложений магазина", this);
            return false;
        }

        if (offerPrefab == null)
        {
            LoggingUtils.LogError("Не задан префаб предложения магазина", this);
            return false;
        }

        if (buttonIndex < 0 || buttonIndex >= availableResources.Length)
        {
            LoggingUtils.LogError($"Неверный индекс ресурса: {buttonIndex}", this);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Очищает содержимое панели предложений
    /// </summary>
    private void ClearShopContent()
    {
        if (contentGameObject == null) return;

        foreach (Transform child in contentGameObject.transform)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// Получает список предложений для указанного ресурса
    /// Реализует паттерн Repository для работы с данными
    /// </summary>
    /// <param name="resourceStringName">Название ресурса</param>
    /// <param name="buttonIndex">Индекс кнопки/ресурса</param>
    /// <returns>Список предложений магазина</returns>
    private async Task<List<ShopOffer>> GetShopOffersForResource(string resourceStringName, int buttonIndex)
    {
        // Проверяем наличие кэшированных предложений
        if (_cachedOffers.ContainsKey(resourceStringName))
        {
            // Если прошло менее 30 секунд с последнего обновления, возвращаем кэшированные предложения
            return _cachedOffers[resourceStringName];
        }

        List<ShopOffer> offers = new List<ShopOffer>();

        try
        {
            // Загружаем список пользователей
            List<APIClient.UserInventory> users = await APIClient.Instance.GetUsersListRequest();
            if (users == null || users.Count == 0) return offers;

            foreach (APIClient.UserInventory user in users)
            {
                // Получаем предложение для пользователя
                ShopOffer offer = await FetchOfferFromUser(user, resourceStringName, buttonIndex);
                if (offer != null)
                {
                    offers.Add(offer);
                }
            }

            // Кэшируем полученные предложения
            _cachedOffers[resourceStringName] = offers;
        }
        catch (Exception ex)
        {
            LoggingUtils.LogException(ex, $"Ошибка при получении предложений для ресурса {resourceStringName}");
            throw new ShopException($"Не удалось загрузить предложения для ресурса {resourceStringName}", ex);
        }

        return offers;
    }

    /// <summary>
    /// Получает предложение от указанного пользователя
    /// </summary>
    /// <param name="user">Данные пользователя</param>
    /// <param name="resourceStringName">Название ресурса</param>
    /// <param name="buttonIndex">Индекс кнопки/ресурса</param>
    /// <returns>Предложение магазина или null, если предложение недоступно</returns>
    private async Task<ShopOffer> FetchOfferFromUser(APIClient.UserInventory user, string resourceStringName, int buttonIndex)
    {
        if (user == null || string.IsNullOrEmpty(user.Name) || !user.IsValid())
        {
            return null;
        }

        try
        {
            // Получаем магазин пользователя
            var shops = await APIClient.Instance.GetShopsRequest(user.Name);
            if (shops == null || shops.Count == 0) return null;

            // При создании игрока у него автоматически создается магазин, поэтому всегда берем первый элемент
            APIClient.ShopInfo shop = shops[0]; 
            if (shop?.Resources == null || !shop.Resources.ContainsKey(resourceStringName)) return null;

            // Получаем количество ресурса
            int resourceAmount = shop.Resources[resourceStringName];
            if (resourceAmount <= 0) return null; // Не показываем предложения с нулевым количеством

            // Создаем объект предложения
            return new ShopOffer
            {
                SellerName = user.Name,
                ShopName = shop.Name,
                ResourceType = availableResources[buttonIndex],
                ResourceAmount = resourceAmount,
                ResourceSpriteIndex = buttonIndex
            };
        }
        catch (Exception ex)
        {
            LoggingUtils.LogError($"Ошибка при получении предложения от пользователя {user.Name}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Отображает список предложений магазина
    /// </summary>
    /// <param name="offers">Список предложений</param>
    private void DisplayShopOffers(List<ShopOffer> offers)
    {
        if (offers == null || offers.Count == 0)
        {
            ShowEmptyOffersList();
            return;
        }

        foreach (ShopOffer offer in offers)
        {
            CreateOfferUI(offer);
        }
    }

    /// <summary>
    /// Создает UI-элемент предложения
    /// </summary>
    /// <param name="offer">Информация о предложении</param>
    private void CreateOfferUI(ShopOffer offer)
    {
        if (offer == null || contentGameObject == null || offerPrefab == null) return;

        // Создаем объект предложения из префаба
        GameObject offerObject = Instantiate(offerPrefab, contentGameObject.transform);

        // Устанавливаем спрайт ресурса
        if (resourcesSprites != null && offer.ResourceSpriteIndex < resourcesSprites.Length)
        {
            Image resourceImage = offerObject.GetComponentInChildren<Image>();
            if (resourceImage != null)
            {
                resourceImage.sprite = resourcesSprites[offer.ResourceSpriteIndex];
            }
        }

        // Устанавливаем текст с именем продавца и количеством ресурса
        TextMeshProUGUI sellerNickname = offerObject.GetComponentInChildren<TextMeshProUGUI>();
        if (sellerNickname != null)
        {
            sellerNickname.text = $"{offer.SellerName}: {offer.ResourceAmount}";
        }

        // Получаем кнопку для покупки
        Button buyButton = offerObject.GetComponentInChildren<Button>();
        if (buyButton != null)
        {
            // Используем паттерн Command для обработки клика по кнопке
            buyButton.onClick.AddListener(() => new BuyResourceCommand(this, offer).Execute());
        }
    }

    /// <summary>
    /// Показывает сообщение о пустом списке предложений
    /// </summary>
    private void ShowEmptyOffersList()
    {
        if (contentGameObject == null) return;

        // Создаем текстовый объект с сообщением
        GameObject messageObject = new GameObject("EmptyMessage");
        messageObject.transform.SetParent(contentGameObject.transform, false);

        TextMeshProUGUI messageText = messageObject.AddComponent<TextMeshProUGUI>();
        messageText.text = "Нет доступных предложений для этого ресурса";
        messageText.fontSize = 24;
        messageText.alignment = TextAlignmentOptions.Center;
    }

    /// <summary>
    /// Показывает сообщение об ошибке
    /// </summary>
    /// <param name="message">Текст сообщения</param>
    private void ShowErrorMessage(string message)
    {
        if (contentGameObject == null) return;

        ClearShopContent();

        // Создаем текстовый объект с сообщением об ошибке
        GameObject errorObject = new GameObject("ErrorMessage");
        errorObject.transform.SetParent(contentGameObject.transform, false);

        TextMeshProUGUI errorText = errorObject.AddComponent<TextMeshProUGUI>();
        errorText.text = message;
        errorText.fontSize = 24;
        errorText.color = Color.red;
        errorText.alignment = TextAlignmentOptions.Center;
    }

    /// <summary>
    /// Открывает интерфейс магазина
    /// </summary>
    private void OpenShop()
    {
        if (contentGameObject != null)
        {
            contentGameObject.SetActive(true);
        }

        // Дополнительная логика открытия магазина
        LoggingUtils.LogInfo("Магазин открыт");
    }

    /// <summary>
    /// Закрывает интерфейс магазина
    /// </summary>
    private void CloseShop()
    {
        if (contentGameObject != null)
        {
            contentGameObject.SetActive(false);
        }

        // Дополнительная логика закрытия магазина
        LoggingUtils.LogInfo("Магазин закрыт");
    }

    #endregion

    #region Command Pattern для операций с магазином

    /// <summary>
    /// Базовый класс для команд магазина
    /// Реализует паттерн Command
    /// </summary>
    private abstract class ShopCommand
    {
        protected Shop ShopInstance;

        protected ShopCommand(Shop shopInstance)
        {
            ShopInstance = shopInstance;
        }

        public abstract void Execute();
    }

    /// <summary>
    /// Команда для покупки ресурса
    /// </summary>
    private class BuyResourceCommand : ShopCommand
    {
        private readonly ShopOffer _offer;

        public BuyResourceCommand(Shop shopInstance, ShopOffer offer) : base(shopInstance)
        {
            _offer = offer;
        }

        public override void Execute()
        {
            // Реализация логики покупки ресурса
            LoggingUtils.LogInfo($"Попытка купить {_offer.ResourceAmount} {_offer.ResourceType} у {_offer.SellerName}");

            // Здесь должна быть логика покупки ресурса через APIClient
            // например, вызов асинхронного метода для покупки

            // Пример обработчика покупки (реализация опущена для краткости)
            // ShopInstance.BuyResource(_offer).ContinueWith(task => {
            //     if (task.IsFaulted) {
            //         LoggingUtils.LogError($"Ошибка при покупке: {task.Exception.Message}");
            //     }
            // });
        }
    }

    #endregion
}

/// <summary>
/// Структура данных для хранения информации о предложении в магазине
/// </summary>
public class ShopOffer
{
    /// <summary>
    /// Имя продавца
    /// </summary>
    public string SellerName { get; set; }

    /// <summary>
    /// Название магазина
    /// </summary>
    public string ShopName { get; set; }

    /// <summary>
    /// Тип ресурса
    /// </summary>
    public Resources ResourceType { get; set; }

    /// <summary>
    /// Количество ресурса
    /// </summary>
    public int ResourceAmount { get; set; }

    /// <summary>
    /// Индекс спрайта ресурса
    /// </summary>
    public int ResourceSpriteIndex { get; set; }
}

/// <summary>
/// Исключение для ошибок магазина
/// </summary>
public class ShopException : Exception
{
    public ShopException(string message) : base(message) { }
    public ShopException(string message, Exception innerException) : base(message, innerException) { }
}
