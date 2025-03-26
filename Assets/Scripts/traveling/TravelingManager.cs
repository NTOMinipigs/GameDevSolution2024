using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public class TravelingManager : MonoBehaviour
{
    public static TravelingManager Singleton { get; private set; }
    private PlaceOfTravel[,] placesPos = new PlaceOfTravel[10, 8];
    private PlaceOfTravel _activatedPlace; // Место, куда уже отправлены медведи
    private PlaceOfTravel _selectedPlace; // Выбранное место на карте
    private int _maxPlaces;
    private float _timeElapsed;
    private int homeX, homeY;

    [Header("Generate place of travel")]
    [SerializeField] private PlaceOfTravel[] allPlaces = new PlaceOfTravel[0]; // "Шаблоны" мест
    [SerializeField] private Reward[] rewardsInPlaces = new Reward[0];
    [SerializeField] private Sprite[] placesCellSprites = new Sprite[0];
    [SerializeField] private Sprite spriteHome;

    [Header("UI")]
    public GameObject travelMenu;
    [SerializeField] private GameObject cellContainer, cellPrefab;
    private GameObject _infoOfPlaceMenu, _blockOfTravel;
    private TextMeshProUGUI _textNameLocation, _textDifficultLocation, _textDescriptionLocation, _precentOfKnowPlanet;
    private Button _travelButton;

    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        _maxPlaces = allPlaces.Length;

        // Инициализация
        _infoOfPlaceMenu = travelMenu.transform.Find("panelPlaceInfo")?.gameObject;

        _textNameLocation = _infoOfPlaceMenu?.transform.Find("TextLocationName")?.GetComponent<TextMeshProUGUI>();
        _textDescriptionLocation =
            _infoOfPlaceMenu?.transform.Find("TextLocationDescription")?.GetComponent<TextMeshProUGUI>();
        _textDifficultLocation =
            _infoOfPlaceMenu?.transform.Find("TextLocationDifficult")?.GetComponent<TextMeshProUGUI>();
        _precentOfKnowPlanet = _infoOfPlaceMenu?.transform.Find("TextPrecent")?.GetComponent<TextMeshProUGUI>();
        _blockOfTravel = _infoOfPlaceMenu?.transform.Find("StartTravelBlock")?.gameObject;
        _travelButton = _blockOfTravel?.transform.Find("Button")?.GetComponent<Button>();
    }

    public void CreateNewMap(int seed)
    {
        Random.InitState(seed);
        bool homeSelected = false;

        // Назначаем дом
        homeX = Mathf.RoundToInt(placesPos.GetLength(0) / 2f) - 1;
        homeY = Mathf.RoundToInt(placesPos.GetLength(1) / 2f) - 1;
        placesPos[homeX, homeY] = new PlaceOfTravel("Ваше поселение", true);
        placesPos[homeX, homeY].placeCellIcon = spriteHome;

        // Текущая выбранная клетка. Начинаем с дома
        for (int d = 0; d < 6; d++)
        {
            int selectedX = homeX;
            int selectedY = homeY;

            for (int i = 0; i < 10; i++)
            {
                PlaceOfTravel selectedPlace = allPlaces[Random.Range(0, allPlaces.Length)];
                PlaceOfTravel newPlace = new PlaceOfTravel(selectedPlace.nameOfPlace, selectedPlace.description, selectedPlace.timeToGoing);
                newPlace.placeCellIcon = placesCellSprites[Random.Range(0, placesCellSprites.Length - 1)];
                int newX = selectedX + Random.Range(-1, 2);
                int newY = selectedY + Random.Range(-1, 2);

                if ((newX >= 0 && newX < placesPos.GetLength(0)) && (newY >= 0 && newY < placesPos.GetLength(1)))
                {
                    if (placesPos[newX, newY] == null)
                    {
                        placesPos[newX, newY] = newPlace;

                        // Задаем сложность.
                        int distance = Mathf.Max(Mathf.Abs(newX - homeX), Mathf.Abs(newY - homeY));
                        placesPos[newX, newY].difficulty = distance;

                        // Возможные награды за экспедицию
                        for (int rewardsCount = 0; rewardsCount < Random.Range(1, 2 * placesPos[newX, newY].difficulty); rewardsCount++)
                            placesPos[newX, newY].rewards.Add(rewardsInPlaces[Random.Range(0, rewardsInPlaces.Length - 1)]);

                        selectedX = newX;
                        selectedY = newY;
                    }
                    else // Просто выкидываем рандомно
                    {
                        selectedX = homeX + Random.Range(-3, 3);
                        selectedY = homeY + Random.Range(-3, 3);
                    }
                }
                else
                {
                    selectedX = homeX + Random.Range(-3, 3);
                    selectedY = homeY + Random.Range(-3, 3);
                }
            }
        }
    }

    /// <summary>
    /// Открытие меню путешествий
    /// </summary>
    /// <param name="canTravel"></param>
    private void OpenTravelMenu(bool canTravel = false)
    {
        // Если какая-то менюха уже открыта
        if (GameMenuManager.Singleton.CheckOpenedWindows(!travelMenu.activeSelf)) return;
        travelMenu.gameObject.SetActive(!travelMenu.activeSelf);
        _infoOfPlaceMenu.gameObject.SetActive(false); // Закрытие информации о месте, если была открыта
        if (!travelMenu.activeSelf) return;

        UpdateMap();
        _blockOfTravel.gameObject.SetActive(canTravel);
        //if (_blockOfTravel.activeSelf)
        //_travelButton.interactable =
        //(_activatedPlace.gameName == "" &&
        //ColonyManager.Singleton.Food >= _activatedPlace.foodNeed); // Если исследование не идет
    }

    /// <summary>
    /// Начать экспедицию
    /// </summary>
    public void StartExpedition()
    {
        //_activatedPlace = _selectedPlace;
        //travelMenu.gameObject.SetActive(!travelMenu.activeSelf);
        _selectedPlace.placeIsChecked = true;
        UpdateMap();
    }

    /// <summary>
    /// Выбрать какое-либо место для путешествия
    /// </summary>
    /// <param name="placeObj"></param>
    public void ChoicePlace(PlaceOfTravel placeOfTravel)
    {
        _selectedPlace = placeOfTravel;
        _infoOfPlaceMenu.gameObject.SetActive(true);
        _textNameLocation.text = _selectedPlace.nameOfPlace;
        _textDifficultLocation.text = "Сложность: " + _selectedPlace.difficulty;
        string rewards = "";
        foreach (Reward reward in _selectedPlace.rewards)
            rewards += " +" + reward.typeOfReward.GetString() + " x" + reward.count + "\n";
        _textDescriptionLocation.text = _selectedPlace.description + "Награды: \n" + rewards;
        if (_blockOfTravel.activeSelf)
        {
            TextMeshProUGUI travelInfo =
                _blockOfTravel.transform.Find("TextTravelInfo").GetComponent<TextMeshProUGUI>();

            if (_activatedPlace != null)
                travelInfo.text = _selectedPlace.timeToGoing / 60 + " мин/-" + _selectedPlace.foodNeed + " еды/-" + _selectedPlace.bioFuelNeed + " топлива";
            else
                travelInfo.text = "Экспедиция уже начата!";
        }
    }

    public void UpdateMap()
    {
        foreach (Transform child in cellContainer.transform)
            Destroy(child.gameObject);
            
        for (int y = 0; y < placesPos.GetLength(1); y++)
        {
            for (int x = 0; x < placesPos.GetLength(0); x++)
            {
                if (placesPos[x, y] != null)
                {
                    var cell = Instantiate(cellPrefab, new Vector3(0, 0, 0), Quaternion.identity, cellContainer.transform);
                    cell.GetComponent<Image>().sprite = placesPos[x, y].placeCellIcon;
                    cell.transform.localPosition = new Vector3(x * 115, y * 125, 0);

                    // Проверяем, является ли клетка соседней к дому
                    cell.GetComponent<Button>().interactable = (x + 1 == homeX || x - 1 == homeX || x == homeX) && (y + 1 == homeY || y - 1 == homeY || y == homeY);

                    if (!cell.GetComponent<Button>().interactable) // Если остался выключенным
                    {
                        bool findChecked = false;

                        // Проверяем соседние клетки
                        for (int x1 = -1; x1 <= 1; x1++)
                        {
                            for (int y1 = -1; y1 <= 1; y1++)
                            {
                                if (x1 == 0 && y1 == 0) continue; // Пропускаем саму клетку

                                int neighborX = x + x1;
                                int neighborY = y + y1;

                                if (neighborX >= 0 && neighborX < placesPos.GetLength(0) && neighborY >= 0 && neighborY < placesPos.GetLength(1))
                                {
                                    if (placesPos[neighborX, neighborY] != null && placesPos[neighborX, neighborY].placeIsChecked)
                                    {
                                        findChecked = true;
                                        break;
                                    }
                                }
                            }

                            if (findChecked) break;
                        }

                        cell.GetComponent<Button>().interactable = findChecked;
                    }

                    int xForButton = x;
                    int yForButton = y;
                    cell.GetComponent<Button>().onClick.AddListener(() => ChoicePlace(placesPos[xForButton, yForButton]));
                }
            }
        }

        //_precentOfKnowPlanet.text =
        //"Мир исследован на " + (1 - (allPlaces.Length - _maxPlaces)) * 100 + "%";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            OpenTravelMenu(ColonyManager.Singleton.scoutHome);
    }

    private void FixedUpdate()
    {
        if (_activatedPlace == null) return;

        // Каждую секунду
        _timeElapsed += Time.deltaTime;
        _activatedPlace.timeNow += Mathf.FloorToInt(_timeElapsed);
        _timeElapsed -= Mathf.FloorToInt(_timeElapsed);
        if (_activatedPlace.timeNow >= _activatedPlace.timeToGoing)
        {
            GameEventsManager.Singleton.ActivateEvent(new GameEvent(_activatedPlace.nameOfPlace,
                _activatedPlace.description));
            _activatedPlace.placeIsChecked = true;
            _activatedPlace = new PlaceOfTravel(); // Очистка
            _timeElapsed = 0f;
        }
    }
}