using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TravelingManager : MonoBehaviour
{
    public GameObject travelMenu, infoOfPlaceMenu, blockOfTravel;
    [SerializeField] private GameObject resultOfTravelMenu;
    [SerializeField] private PlaceOfTravel[] allPlaces = new PlaceOfTravel[0]; // Все места.
    [SerializeField] PlaceOfTravel activatedPlace; // Место, куда уже отправлены люди
    private PlaceOfTravel _selectedPlace;
    [SerializeField] private TextMeshProUGUI textNameLocation, textDescriptionLocation, precentOfKnowPlanet;
    private Button _travelButton;
    [SerializeField] private allScripts scripts;
    private int _maxPlaces;
    private float _timeElapsed;

    private void Awake()
    {
        _maxPlaces = allPlaces.Length;
        _travelButton = blockOfTravel.transform.Find("Button").GetComponent<Button>();
    }

    private void OpenTravelMenu(bool canTravel = false)
    {
        if (scripts.CheckOpenedWindows(!travelMenu.activeSelf)) // Если какая-то менюха уже открыта
            return;
        travelMenu.gameObject.SetActive(!travelMenu.activeSelf);
        infoOfPlaceMenu.gameObject.SetActive(false);
        if (!travelMenu.activeSelf) return;

        UpdateMap();
        blockOfTravel.gameObject.SetActive(canTravel);
        if (blockOfTravel.activeSelf)
            _travelButton.interactable =
                (activatedPlace.gameName == "" &&
                 scripts.colonyManager.Food >= activatedPlace.foodNeed); // Если исследование не идет
    }

    public void StartExpedition()
    {
        activatedPlace = _selectedPlace;
        travelMenu.gameObject.SetActive(!travelMenu.activeSelf);
    }

    public void ChoicePlace(GameObject placeObj)
    {
        foreach (PlaceOfTravel place in allPlaces)
        {
            if (place.gameName == placeObj.name)
            {
                infoOfPlaceMenu.gameObject.SetActive(true);
                textNameLocation.text = place.nameOfPlace;
                textDescriptionLocation.text = place.description;
                _selectedPlace = place;
                if (blockOfTravel.activeSelf)
                {
                    TextMeshProUGUI travelInfo =
                        blockOfTravel.transform.Find("TextTravelInfo").GetComponent<TextMeshProUGUI>();

                    if (activatedPlace.gameName == "")
                        travelInfo.text = (place.timeToGoing / 60).ToString() + " мин/-" + place.foodNeed.ToString() +
                                          " еды";
                    else
                        travelInfo.text = "Экспедиция уже начата!";
                }

                break;
            }
        }
    }

    public void UpdateMap()
    {
        foreach (Transform child in travelMenu.transform.Find("places"))
        {
            foreach (PlaceOfTravel place in allPlaces)
            {
                if (place.gameName == child.gameObject.name)
                {
                    child.gameObject.GetComponent<Button>().interactable = !place.placeIsChecked;
                    break;
                }
            }
        }

        precentOfKnowPlanet.text =
            "Мир исследован на " + ((1 - (allPlaces.Length - _maxPlaces)) * 100).ToString() + "%";
    }

    /// <summary>
    /// Блять что это
    /// </summary>
    public void ActivateTravelResult()
    {
        resultOfTravelMenu.gameObject.SetActive(true);
        resultOfTravelMenu.transform.Find("TextName").GetComponent<TextMeshProUGUI>().text =
            "Экспедиция вернулась: " + activatedPlace.nameOfPlace;
        resultOfTravelMenu.transform.Find("TextInfo").GetComponent<TextMeshProUGUI>().text = activatedPlace.resultText;
        activatedPlace.placeIsChecked = true;
        foreach (Reward reward in activatedPlace.rewards)
        {
            switch (reward.typeOfReward)
            {
                case Resources.Material:
                    scripts.colonyManager.Materials += reward.count;
                    break;
                case Resources.MaterialPlus:
                    scripts.colonyManager.MaterialsPlus += reward.count;
                    break;
                case Resources.Food:
                    scripts.colonyManager.Food += reward.count;
                    break;
                case Resources.Honey:
                    scripts.colonyManager.Honey += reward.count;
                    break;
                case Resources.BioFuel:
                    scripts.colonyManager.Biofuel += reward.count;
                    break;
                case Resources.Bears:
                    for (int i = 0; i < reward.count; i++)
                    {
                        // Получаем все значения перечисления Traditions
                        Traditions[] traditions = (Traditions[])System.Enum.GetValues(typeof(Traditions));

                        // Генерируем случайный индекс
                        int randomIndex = Random.Range(0, traditions.Length);
                        scripts.colonyManager.GenerateNewBear(traditions[randomIndex]);
                    }

                    break;
            }
        }

        Time.timeScale = 0.05f;
        activatedPlace = new PlaceOfTravel(); // Очистка
    }

    public void DisableTravelResult() // Для UI
    {
        resultOfTravelMenu.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            OpenTravelMenu(scripts.colonyManager.scoutHome);
    }

    private void FixedUpdate()
    {
        if (activatedPlace.gameName != "")
        {
            // Каждую секунду
            _timeElapsed += Time.deltaTime;
            activatedPlace.timeNow += Mathf.FloorToInt(_timeElapsed);
            _timeElapsed -= Mathf.FloorToInt(_timeElapsed);
            if (activatedPlace.timeNow >= activatedPlace.timeToGoing)
            {
                ActivateTravelResult();
                _timeElapsed = 0f;
                activatedPlace.gameName = "";
            }
        }
    }
}