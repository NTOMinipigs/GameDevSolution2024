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

    public void ActivateTravelResult()
    {
        resultOfTravelMenu.gameObject.SetActive(true);
        resultOfTravelMenu.transform.Find("TextName").GetComponent<TextMeshProUGUI>().text = "Экспедиция вернулась: " + activatedPlace.nameOfPlace;
        resultOfTravelMenu.transform.Find("TextInfo").GetComponent<TextMeshProUGUI>().text = activatedPlace.resultText;
        activatedPlace.placeIsChecked = true;
        foreach (Reward reward in activatedPlace.rewards)
        {
            switch (reward.typeOfReward)
            {
                case ResourcesManager.Resources.Material:
                    scripts.colonyManager.Materials += reward.count;
                    break;
                case ResourcesManager.Resources.MaterialPlus:
                    scripts.colonyManager.materialsPlus += reward.count;
                    break;
                case ResourcesManager.Resources.Food:
                    scripts.colonyManager.Food += reward.count;
                    break;
                case ResourcesManager.Resources.Honey:
                    scripts.colonyManager.Honey += reward.count;
                    break;
                case ResourcesManager.Resources.BioFuel:
                    scripts.colonyManager.Biofuel += reward.count;
                    break;
                case ResourcesManager.Resources.Bears:
                    for (int i = 0; i < reward.count; i++)
                    {
                        // Получаем все значения перечисления Traditions
                        TraditionsManager.Traditions[] traditions = (TraditionsManager.Traditions[])System.Enum.GetValues(typeof(TraditionsManager.Traditions));

                        // Генерируем случайный индекс
                        int randomIndex = Random.Range(0, traditions.Length);
                        scripts.colonyManager.GenerateNewBear(traditions[randomIndex]);
                    }
                    break;
            }
        }
        scripts.clicksHandler.SetTimeScale(0.05f);
        activatedPlace = new PlaceOfTravel(); // Очистка
    }

    public void DisableTravelResult() // Для UI
    {
        resultOfTravelMenu.gameObject.SetActive(false);
        scripts.clicksHandler.SetTimeScale(1f);
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
            timeElapsed += Time.deltaTime;
            activatedPlace.timeNow += Mathf.FloorToInt(timeElapsed);
            timeElapsed -= Mathf.FloorToInt(timeElapsed);
            if (activatedPlace.timeNow >= activatedPlace.timeToGoing)
            {
                ActivateTravelResult();
                timeElapsed = 0f;
                activatedPlace.gameName = "";
            }
        }
    }
}
