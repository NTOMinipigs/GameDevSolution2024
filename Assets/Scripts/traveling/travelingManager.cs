using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TravelingManager : MonoBehaviour
{
    public GameObject travelMenu, infoOfPlaceMenu, blockOfTravel;
    [SerializeField] private GameObject resultOfTravelMenu;
    [SerializeField] private PlaceOfTravel[] allPlaces = new PlaceOfTravel[0]; // Все места.
    [SerializeField] PlaceOfTravel activatedPlace; // Место, куда уже отправлены люди
    private PlaceOfTravel selectedPlace;
    [SerializeField] private TextMeshProUGUI textNameLocation, textDescriptionLocation, precentOfKnowPlanet;
    [SerializeField] private allScripts scripts;
    private int maxPlaces;
    private float timeElapsed;

    private void Start() => maxPlaces = allPlaces.Length;

    public void OpenTravelMenu(bool canTravel = false)
    {
        if (scripts.CheckOpenedWindows(!travelMenu.activeSelf)) // Если какая-то менюха уже открыта
            return;
        travelMenu.gameObject.SetActive(!travelMenu.activeSelf);
        infoOfPlaceMenu.gameObject.SetActive(false);
        if (travelMenu.activeSelf)
        {
            UpdateMap();
            blockOfTravel.gameObject.SetActive(canTravel);
            if (blockOfTravel.activeSelf)
                blockOfTravel.transform.Find("Button").GetComponent<Button>().interactable = (activatedPlace.gameName == "" && scripts.colonyManager.Food >= activatedPlace.foodNeed); // Если исследование не идет
        }
    }

    public void StartExpedition()
    {
        activatedPlace = selectedPlace;
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
                selectedPlace = place;
                if (blockOfTravel.activeSelf)
                {
                    if (activatedPlace.gameName == "")
                        blockOfTravel.transform.Find("TextTravelInfo").GetComponent<TextMeshProUGUI>().text = (place.timeToGoing / 60).ToString() + " мин/-" + place.foodNeed.ToString() + " еды";
                    else
                        blockOfTravel.transform.Find("TextTravelInfo").GetComponent<TextMeshProUGUI>().text = "Экспедиция уже начата!";
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
        precentOfKnowPlanet.text = "Мир исследован на " + ((1 - (allPlaces.Length - maxPlaces)) * 100).ToString() + "%";
    }

    public void ActivateTravelResult(bool activate = true)
    {
        resultOfTravelMenu.gameObject.SetActive(activate);
        if (activate)
        {
            resultOfTravelMenu.transform.Find("TextName").GetComponent<TextMeshProUGUI>().text = activatedPlace.nameOfPlace;
            resultOfTravelMenu.transform.Find("TextInfo").GetComponent<TextMeshProUGUI>().text = activatedPlace.resultText;
            activatedPlace.placeIsChecked = true;
            scripts.clicksHandler.SetTimeScale(0.05f);
        }
        else
        {
            scripts.clicksHandler.SetTimeScale(1f);
            activatedPlace = new PlaceOfTravel(); // Очистка
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            OpenTravelMenu(scripts.colonyManager.scoutHome);

        if (activatedPlace.gameName != "")
        {
            // Каждую секунду
            timeElapsed += Time.deltaTime;
            activatedPlace.timeNow += Mathf.FloorToInt(timeElapsed);
            timeElapsed -= Mathf.FloorToInt(timeElapsed);
            if (activatedPlace.timeNow >= activatedPlace.timeToGoing)
                ActivateTravelResult(true);
        }
    }
}
