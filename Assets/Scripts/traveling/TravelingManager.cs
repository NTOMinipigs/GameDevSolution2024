using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public class TravelingManager : MonoBehaviour
{
    public static TravelingManager Singleton { get; private set; }
    [Header("Точки интереса")] [SerializeField]
    private PlaceOfTravel[] allPlaces = Array.Empty<PlaceOfTravel>(); // Все места.

    private Dictionary<string, PlaceOfTravel> _allPlacesDict = new Dictionary<string, PlaceOfTravel>();
    private PlaceOfTravel _activatedPlace; // Место, куда уже отправлены медведи
    private PlaceOfTravel _selectedPlace; // Выбранное место на карте

    [Header("Настройки")] public GameObject travelMenu;
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

        foreach (PlaceOfTravel pot in allPlaces)
            _allPlacesDict.Add(pot.gameName, pot);
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
        if (_blockOfTravel.activeSelf)
            _travelButton.interactable =
                (_activatedPlace.gameName == "" &&
                 ColonyManager.Singleton.Food >= _activatedPlace.foodNeed); // Если исследование не идет
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
        _selectedPlace = _allPlacesDict[placeObj.name];
        _infoOfPlaceMenu.gameObject.SetActive(true);
        _textNameLocation.text = _selectedPlace.nameOfPlace;
        _textDescriptionLocation.text = _selectedPlace.description;
        if (_blockOfTravel.activeSelf)
        {
            TextMeshProUGUI travelInfo =
                _blockOfTravel.transform.Find("TextTravelInfo").GetComponent<TextMeshProUGUI>();

            if (_activatedPlace.gameName == "")
                travelInfo.text = _selectedPlace.timeToGoing / 60 + " мин/-" + _selectedPlace.foodNeed + " еды";
            else
                travelInfo.text = "Экспедиция уже начата!";
        }
    }

    public void UpdateMap()
    {
        foreach (Transform child in travelMenu.transform.Find("places"))
        {
            child.gameObject.GetComponent<Button>().interactable =
                !_allPlacesDict[child.gameObject.name].placeIsChecked;
            break;
        }

        _precentOfKnowPlanet.text =
            "Мир исследован на " + (1 - (allPlaces.Length - _maxPlaces)) * 100 + "%";
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