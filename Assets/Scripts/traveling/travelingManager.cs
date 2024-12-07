using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TravelingManager : MonoBehaviour
{
    public GameObject travelMenu, infoOfPlaceMenu, blockOfTravel;
    [SerializeField] private PlaceOfTravel[] allPlaces = new PlaceOfTravel[0]; // Все места.
    [SerializeField] PlaceOfTravel activatedPlace; // Место, куда уже отправлены люди
    private PlaceOfTravel selectedPlace;
    [SerializeField] private TextMeshProUGUI textNameLocation, textDescriptionLocation, precentOfKnowPlanet;
    [SerializeField] private allScripts scripts;
    private int maxPlaces;

    private void Start() => maxPlaces = allPlaces.Length;

    public void OpenTravelMenu(bool canTravel = false)
    {
        travelMenu.gameObject.SetActive(!travelMenu.activeSelf);
        infoOfPlaceMenu.gameObject.SetActive(false);
        if (travelMenu.activeSelf)
        {
            UpdateMap();
            blockOfTravel.gameObject.SetActive(canTravel);
            if (blockOfTravel.activeSelf)
                blockOfTravel.transform.Find("Button").GetComponent<Button>().interactable = activatedPlace.gameName == ""; // Если исследование не идет
        }
    }

    public void StartExpedition()
    {

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
                    blockOfTravel.transform.Find("TextTravelInfo").GetComponent<TextMeshProUGUI>().text = (place.timeToGoing / 60).ToString() + " мин/-" + place.foodNeed.ToString() + " еды";
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            OpenTravelMenu(scripts.colonyManager.scoutHome);
    }
}
