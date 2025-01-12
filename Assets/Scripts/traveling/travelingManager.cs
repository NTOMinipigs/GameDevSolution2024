using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public class TravelingManager : MonoBehaviour
{
    [Header("Точки интереса")] [SerializeField]
    private PlaceOfTravel[] allPlaces = Array.Empty<PlaceOfTravel>(); // Все места.

    private Dictionary<string, PlaceOfTravel> _allPlacesDict = new Dictionary<string, PlaceOfTravel>();
    private PlaceOfTravel _activatedPlace; // Место, куда уже отправлены медведи
    private PlaceOfTravel _selectedPlace; // Выбранное место на карте

    [Header("Настройки")] public GameObject travelMenu;
    private GameObject _infoOfPlaceMenu, _blockOfTravel, _resultOfTravelMenu;
    private TextMeshProUGUI _textNameLocation, _textDescriptionLocation, _precentOfKnowPlanet;
    private Button _travelButton;
    private int _maxPlaces;
    private float _timeElapsed;
    private allScripts _scripts;

    private void Start()
    {
        _scripts = GameObject.Find("scripts").GetComponent<allScripts>();
        _maxPlaces = allPlaces.Length;

        // Инициализация
        _infoOfPlaceMenu = travelMenu.transform.Find("panelPlaceInfo")?.gameObject;

        _textNameLocation = _infoOfPlaceMenu?.transform.Find("TextLocationName")?.GetComponent<TextMeshProUGUI>();
        _textDescriptionLocation =
            _infoOfPlaceMenu?.transform.Find("TextLocationDescription")?.GetComponent<TextMeshProUGUI>();
        _precentOfKnowPlanet = _infoOfPlaceMenu?.transform.Find("TextPrecent")?.GetComponent<TextMeshProUGUI>();
        _blockOfTravel = _infoOfPlaceMenu?.transform.Find("StartTravelBlock")?.gameObject;
        _resultOfTravelMenu = GameObject.Find("menuEvent");
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
        if (_scripts.CheckOpenedWindows(!travelMenu.activeSelf)) return;
        travelMenu.gameObject.SetActive(!travelMenu.activeSelf);
        _infoOfPlaceMenu.gameObject.SetActive(false); // Закрытие информации о месте, если была открыта
        if (!travelMenu.activeSelf) return;

        UpdateMap();
        _blockOfTravel.gameObject.SetActive(canTravel);
        if (_blockOfTravel.activeSelf)
            _travelButton.interactable =
                (_activatedPlace.gameName == "" &&
                 _scripts.colonyManager.Food >= _activatedPlace.foodNeed); // Если исследование не идет
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

    /// <summary>
    /// Блять что это
    /// </summary>
    public void ActivateTravelResult()
    {
        _resultOfTravelMenu.gameObject.SetActive(true);
        _resultOfTravelMenu.transform.Find("TextName").GetComponent<TextMeshProUGUI>().text =
            "Экспедиция вернулась: " + _activatedPlace.nameOfPlace;
        _resultOfTravelMenu.transform.Find("TextInfo").GetComponent<TextMeshProUGUI>().text =
            _activatedPlace.resultText;
        _activatedPlace.placeIsChecked = true;
        foreach (Reward reward in _activatedPlace.rewards)
        {
            switch (reward.typeOfReward)
            {
                case Resources.Material:
                    _scripts.colonyManager.Materials += reward.count;
                    break;
                case Resources.MaterialPlus:
                    _scripts.colonyManager.MaterialsPlus += reward.count;
                    break;
                case Resources.Food:
                    _scripts.colonyManager.Food += reward.count;
                    break;
                case Resources.Honey:
                    _scripts.colonyManager.Honey += reward.count;
                    break;
                case Resources.BioFuel:
                    _scripts.colonyManager.Biofuel += reward.count;
                    break;
                case Resources.Bears:
                    for (int i = 0; i < reward.count; i++)
                    {
                        // Получаем все значения перечисления Traditions
                        Traditions[] traditions = (Traditions[])System.Enum.GetValues(typeof(Traditions));

                        // Генерируем случайный индекс
                        int randomIndex = Random.Range(0, traditions.Length);
                        _scripts.colonyManager.GenerateNewBear(traditions[randomIndex]);
                    }

                    break;
            }
        }

        Time.timeScale = 0.05f;
        _activatedPlace = new PlaceOfTravel(); // Очистка
    }

    public void DisableTravelResult() // Для UI
    {
        _resultOfTravelMenu.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            OpenTravelMenu(_scripts.colonyManager.scoutHome);
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
            ActivateTravelResult();
            _timeElapsed = 0f;
            _activatedPlace.gameName = "";
        }
    }
}