using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public class TravelingManager : MonoBehaviour
{
    public static TravelingManager Singleton { get; private set; }
    [Header("Точки интереса")]
    [SerializeField]
    private PlaceOfTravel[] allPlaces = Array.Empty<PlaceOfTravel>(); // Все возможные места.
    private PlaceOfTravel[,] placesPos = new PlaceOfTravel[10, 10];
    private PlaceOfTravel _activatedPlace; // Место, куда уже отправлены медведи
    private PlaceOfTravel _selectedPlace; // Выбранное место на карте

    [Header("Настройки")] public GameObject travelMenu;
    [SerializeField] private GameObject cellContainer, cellPrefab;
    private GameObject _infoOfPlaceMenu, _blockOfTravel;
    private TextMeshProUGUI _textNameLocation, _textDescriptionLocation, _precentOfKnowPlanet;
    private Button _travelButton;
    private int _maxPlaces;
    private float _timeElapsed;

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
        _precentOfKnowPlanet = _infoOfPlaceMenu?.transform.Find("TextPrecent")?.GetComponent<TextMeshProUGUI>();
        _blockOfTravel = _infoOfPlaceMenu?.transform.Find("StartTravelBlock")?.gameObject;
        _travelButton = _blockOfTravel?.transform.Find("Button")?.GetComponent<Button>();
    }

    public void CreateNewMap(int seed)
    {
        Random.InitState(seed);
        bool homeSelected = false;

        // Назначаем дом
        int homeX = Mathf.RoundToInt(placesPos.GetLength(0) / 2f);
        int homeY = Mathf.RoundToInt(placesPos.GetLength(1) / 2f);
        placesPos[homeX, homeY] = new PlaceOfTravel("bearColony", "Ваше поселение", true);

        // Текуущая выбранная клетка. Начинаем с дома
        int selectedX = homeX;
        int selectedY = homeY;

        for (int i = 0; i < 30; i++)
        {
            PlaceOfTravel newPlace = allPlaces[Random.Range(0, allPlaces.Length)];
            int newX = selectedX + Random.Range(-1, 1);
            int newY = selectedY + Random.Range(-1, 1);
            if ((newX > 0 && newX < placesPos.GetLength(0)) && (newY > 0 && newY < placesPos.GetLength(1)))
            {
                if (newX != homeX && newY != homeY) // Клетки могут менять друг друга, но не дом
                    placesPos[newX, newY] = newPlace;
            }
            selectedX = newX;
            selectedY = newY;
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
        _activatedPlace = _selectedPlace;
        travelMenu.gameObject.SetActive(!travelMenu.activeSelf);
    }

    /// <summary>
    /// Выбрать какое-либо место для путешествия
    /// </summary>
    /// <param name="placeObj"></param>
    public void ChoicePlace(GameObject placeObj)
    {
        //_selectedPlace = _allPlacesDict[placeObj.name];
        _infoOfPlaceMenu.gameObject.SetActive(true);
        _textNameLocation.text = _selectedPlace.nameOfPlace;
        _textDescriptionLocation.text = _selectedPlace.description;
        if (_blockOfTravel.activeSelf)
        {
            TextMeshProUGUI travelInfo =
                _blockOfTravel.transform.Find("TextTravelInfo").GetComponent<TextMeshProUGUI>();

            //if (_activatedPlace.gameName == "")
                //travelInfo.text = _selectedPlace.timeToGoing / 60 + " мин/-" + _selectedPlace.foodNeed + " еды";
            //else
                //travelInfo.text = "Экспедиция уже начата!";
        }
    }

    public void UpdateMap()
    {
        for (int y = 0; y < 10; y++)
        {
            for (int x = 0; x < 10; x++)
            {
                if (placesPos[x, y] != null)
                {
                    var cell = Instantiate(cellPrefab, new Vector3(0, 0, 0), Quaternion.identity, cellContainer.transform);
                    cell.transform.localPosition = new Vector3(x * 85, y * 85, 0);
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
            _activatedPlace.gameName = "";
        }
    }
}