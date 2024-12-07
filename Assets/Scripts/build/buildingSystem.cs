using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingSystem : MonoBehaviour
{
    public GameObject buildingCreateMenu, buildMenu;
    [SerializeField] private GameObject buildMenuStandartButtons, buildMenuMaterialsButtons, noteBlock, droneManage, bearManage;
    [SerializeField] private TextMeshProUGUI textSelectedBuild, textNameBuild, textInfoBuild, textCountBears, textCountDrones;
    [SerializeField] private Vector2Int GridSize = new Vector2Int(10, 10); // Сетка строительсва. P.s значение в юньке не 10 10
    public Building selectedBuild; // Выбранное строение для взаимодействий(НЕ ДЛЯ СТРОЕНИЯ)
    public ColonyManager.typeOfResource selectedResource; // Выбранный ресурс(строение)
    private Building[,] grid; // Размещение строений на сетке
    private Building flyingBuilding; // Выделенное строение
    private Camera mainCamera;
    [SerializeField] private allScripts scripts;

    private void Awake()
    {
        grid = new Building[GridSize.x, GridSize.y];
        mainCamera = Camera.main;
    }

    public void SelectBuildingToInteraction(Building building)
    {
        if (scripts.CheckOpenedWindows(true)) // Если какая-то менюха уже открыта
            return;
        if (!building.builded) // Потом изменить
            return;
        selectedBuild = building;
        bool isResource = building.typeOfBuilding == Building.TypesOfBuilding.resource;
        if (isResource)
            selectedResource = building.typeResource;
        ManageBuildMenu(true, isResource);
    }

    public void DisableBuildMenu() => ManageBuildMenu(false); // Для UI кнопки

    public void AddWorker(bool drone = false)
    {
        if (drone)
            selectedBuild.countOfDrone++;
        else
            selectedBuild.countOfBears++;
        scripts.colonyManager.workingBears++; // Костыль
        textCountDrones.text = selectedBuild.countOfDrone.ToString() + "/" + selectedBuild.maxDrones.ToString();
        textCountBears.text = selectedBuild.countOfBears.ToString() + "/" + selectedBuild.maxBears.ToString();
        textInfoBuild.text = "+ " + ((selectedBuild.countOfBears + selectedBuild.countOfDrone) * selectedBuild.resourseOneWorker).ToString();

        // TODO сделать условие для дронов и медведей selectedBuild.countOfBears == selectedBuild.maxBears
        if ((scripts.colonyManager.bearsInColony.Count - scripts.colonyManager.workingBears) == 0 || selectedBuild.countOfBears == selectedBuild.maxBears)
        {
            bearManage.transform.Find("ButtonAddBear").GetComponent<Button>().interactable = false;
            droneManage.transform.Find("ButtonAddBear").GetComponent<Button>().interactable = false;
        }
        else
            ManageBuildMenu(true);
    }

    public void RemoveWorker(bool drone = false)
    {
        if (drone)
            selectedBuild.countOfDrone--;
        else
            selectedBuild.countOfBears--;
        scripts.colonyManager.workingBears--; // Костыль
        textCountDrones.text = selectedBuild.countOfDrone.ToString() + "/" + selectedBuild.maxDrones.ToString();
        textCountBears.text = selectedBuild.countOfBears.ToString() + "/" + selectedBuild.maxBears.ToString();
        textInfoBuild.text = "+ " + ((selectedBuild.countOfBears + selectedBuild.countOfDrone) * selectedBuild.resourseOneWorker).ToString();

        if ((scripts.colonyManager.bearsInColony.Count - scripts.colonyManager.workingBears) == scripts.colonyManager.maxBears && scripts.colonyManager.maxBears != 0 || selectedBuild.countOfBears == 0)
        {
            bearManage.transform.Find("ButtonDeleteBear").GetComponent<Button>().interactable = false;
            droneManage.transform.Find("ButtonDeleteBear").GetComponent<Button>().interactable = false;
        }
        else
            ManageBuildMenu(true);
    }

    private void ManageDroneAndBearPanel(bool bear, bool drone)
    {
        bearManage.transform.Find("ButtonAddBear").GetComponent<Button>().interactable = bear;
        bearManage.transform.Find("ButtonDeleteBear").GetComponent<Button>().interactable = bear;
        droneManage.transform.Find("ButtonAddBear").GetComponent<Button>().interactable = drone;
        droneManage.transform.Find("ButtonDeleteBear").GetComponent<Button>().interactable = drone;
        if (!drone)
            textCountDrones.text = "";
        if (!bear)
            textCountBears.text = "";

        // Костыль
        if ((scripts.colonyManager.bearsInColony.Count - scripts.colonyManager.workingBears) == 0 || selectedBuild.countOfBears == selectedBuild.maxBears)
        {
            bearManage.transform.Find("ButtonAddBear").GetComponent<Button>().interactable = false;
            droneManage.transform.Find("ButtonAddBear").GetComponent<Button>().interactable = false;
        }
        if ((scripts.colonyManager.bearsInColony.Count - scripts.colonyManager.workingBears) == scripts.colonyManager.maxBears || selectedBuild.countOfBears == 0)
        {
            bearManage.transform.Find("ButtonDeleteBear").GetComponent<Button>().interactable = false;
            droneManage.transform.Find("ButtonDeleteBear").GetComponent<Button>().interactable = false;
        }
    }

    public void ManageBuildMenu(bool open = true, bool materialsMode = false)
    {
        buildMenu.gameObject.SetActive(open);
        if (open)
        {
            textNameBuild.text = selectedBuild.buildingName;
            textInfoBuild.text = "+ " + ((selectedBuild.countOfBears + selectedBuild.countOfDrone) * selectedBuild.resourseOneWorker).ToString();
            textCountDrones.text = selectedBuild.countOfDrone.ToString() + "/" + selectedBuild.maxDrones.ToString();
            textCountBears.text = selectedBuild.countOfBears.ToString() + "/" + selectedBuild.maxBears.ToString();

            if (selectedBuild.canWork)
            {
                if (selectedBuild.typeOfWorkers == Building.TypeOfWorkers.Any)
                    ManageDroneAndBearPanel(true, false); // Костыль пока дронов нету
                else if (selectedBuild.typeOfWorkers == Building.TypeOfWorkers.Drone)
                    ManageDroneAndBearPanel(false, false);
                else
                    ManageDroneAndBearPanel(true, false);
            }
            else
                ManageDroneAndBearPanel(false, false);

            buildMenuStandartButtons.gameObject.SetActive(!materialsMode);
            if (!materialsMode)
            {
                buildMenuStandartButtons.transform.Find("ButtonDestroy").transform.Find("TextResourceReturn").GetComponent<TextMeshProUGUI>().text = (selectedBuild.materialsNeed / 2).ToString();
            }

            buildMenuMaterialsButtons.gameObject.SetActive(materialsMode);
        }
    }

    public void StartPickUpResource()
    {
        selectedBuild.builded = false;
        buildMenu.gameObject.SetActive(false);
        scripts.colonyManager.CreateNewTask(BearTask.TasksMode.getResource, selectedBuild.gameObject, selectedBuild.stepsNeed);
    }

    public void PickUpResource(GameObject resourceObj)
    {
        selectedResource = resourceObj.GetComponent<Building>().typeResource;
        string resourceChanged = ""; // Здесь хранится строчное представление ресурса, который изменили. Для логов
        int earn = 0;  // Вынес в отдельную переменную, т.к. после свитчкейса нужно записать все в лог
        
        switch (selectedResource)
        {
            case ColonyManager.typeOfResource.materials:
                earn = Random.Range(10, 30);
                resourceChanged = "materials";
                scripts.colonyManager.Materials += earn;
                break;
            case ColonyManager.typeOfResource.materialPlus:
                earn = Random.Range(1, 5);
                resourceChanged = "materialPlus";
                scripts.colonyManager.materialsPlus += earn;
                break;
            case ColonyManager.typeOfResource.food:
                earn = Random.Range(1, 10);
                resourceChanged = "food";
                scripts.colonyManager.Food += earn;
                break;
            case ColonyManager.typeOfResource.honey:
                earn = Random.Range(5, 10);
                resourceChanged = "honey";
                scripts.colonyManager.Honey += earn;
                break;
            case ColonyManager.typeOfResource.bioFuel:
                earn = Random.Range(5, 15);
                resourceChanged = "bioFuel";
                scripts.colonyManager.Biofuel += earn;
                break;
        }
        
        DestroyBuilding(resourceObj);
        // Лог
        APIClient.Instance.CreateLogRequest(
            "Новые ресурсы произведенные в результате работы некоторого строения",
            Player.Instance.playerName,
            new Dictionary<string, string>() {{resourceChanged, "+" + earn}});
    }

    public void StartPlacingBuilding(Building buildingPrefab) // Начинаем размещать объект. Метод для кнопки
    {
        if (flyingBuilding != null)
            Destroy(flyingBuilding.gameObject);

        flyingBuilding = Instantiate(buildingPrefab);
        noteBlock.gameObject.SetActive(true);
        textSelectedBuild.text = flyingBuilding.buildingName;
    }

    public void DestroyBuilding(GameObject destroyBuilding = null)
    {
        if (destroyBuilding != null)
            selectedBuild = destroyBuilding.GetComponent<Building>();

        if (selectedBuild != null)
        {
            // Получаем координаты здания на сетке
            int startX = Mathf.RoundToInt(selectedBuild.transform.position.x) - selectedBuild.Size.x / 2;
            int startY = Mathf.RoundToInt(selectedBuild.transform.position.z) - selectedBuild.Size.y / 2;

            // Удаляем здание из grid
            for (int x = 0; x < selectedBuild.Size.x; x++)
            {
                for (int y = 0; y < selectedBuild.Size.y; y++)
                    grid[startX + x, startY + y] = null; // Очищаем ячейку
            }

            // Уничтожаем объект
            Destroy(selectedBuild.gameObject);
            scripts.colonyManager.Materials += selectedBuild.materialsNeed / 2;
            if (selectedBuild.energyNeed != 0)
            {
                scripts.colonyManager.Energy -= selectedBuild.energyNeed;

                APIClient.Instance.CreateLogRequest(
                    "Потрачена энергия на снос здания",
                    Player.Instance.playerName,
                    new Dictionary<string, string>() { { "energy", "-" + selectedBuild.energyNeed } });
            }

            selectedBuild = null;
            buildMenu.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) // Активация меню какое здание построить
        {
            if (!scripts.CheckOpenedWindows(!buildingCreateMenu.activeSelf)) // Если какая-то менюха уже открыта
            {
                buildingCreateMenu.gameObject.SetActive(!buildingCreateMenu.activeSelf);
                noteBlock.gameObject.SetActive(false);
                // Генерация списка зданий к постройке
                foreach (Transform child in buildingCreateMenu.transform.Find("Scroll View").transform.Find("Viewport").transform.Find("Content"))
                {
                    // Возможность нажать на кнопку. Значение в виде условия
                    child.gameObject.GetComponent<Button>().interactable = child.gameObject.GetComponent<Building>().materialsNeed <= scripts.colonyManager.Materials && child.gameObject.GetComponent<Building>().energyNeed <= scripts.colonyManager.Energy;
                    if (child.gameObject.GetComponent<Building>().materialsNeed <= scripts.colonyManager.Materials)
                        child.transform.Find("TextPrice").GetComponent<TextMeshProUGUI>().color = Color.black;
                    else
                        child.transform.Find("TextPrice").GetComponent<TextMeshProUGUI>().color = Color.red;

                    if (child.gameObject.GetComponent<Building>().energyNeed <= scripts.colonyManager.Energy)
                        child.transform.Find("TextPriceEnergy").GetComponent<TextMeshProUGUI>().color = Color.black;
                    else
                        child.transform.Find("TextPriceEnergy").GetComponent<TextMeshProUGUI>().color = Color.red;
                }
                if (!buildingCreateMenu.activeSelf)
                    Destroy(flyingBuilding.gameObject);
            }
        }

        // TODO: сделать отмену выбора текущего здания + смену выбранного здания на другое(оно мб работает)
        if (flyingBuilding != null) // Если зданиее не выделено
        {
            if (Input.GetKeyDown(KeyCode.R)) // Поворот здания
            {
                flyingBuilding.Size = new Vector2Int(flyingBuilding.Size.y, flyingBuilding.Size.x);
                flyingBuilding.transform.Rotate(0, 90f, 0);
            }
            var groundPlane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition); // Пускаем луч из камеры

            if (groundPlane.Raycast(ray, out float position)) // Хз это за хуйня
            {
                Vector3 worldPosition = ray.GetPoint(position);

                int x = Mathf.RoundToInt(worldPosition.x);
                int y = Mathf.RoundToInt(worldPosition.z);

                x -= flyingBuilding.Size.x / 2;
                y -= flyingBuilding.Size.y / 2;

                bool available = true;

                // Если здание за сеткой - помечать расположение недействительным
                if (x < -GridSize.x / 2 || x > GridSize.x / 2 - flyingBuilding.Size.x) available = false;
                if (y < -GridSize.y / 2 || y > GridSize.y / 2 - flyingBuilding.Size.y) available = false;

                // Если здание расположено на другом - помечать расположение недействительным
                if (available && IsPlaceTaken(x, y)) available = false;

                flyingBuilding.transform.position = new Vector3(x + flyingBuilding.Size.x / 2f, 0, y + flyingBuilding.Size.y / 2f);
                flyingBuilding.SetTransparent(available); // Смена окраски

                if (available && Input.GetMouseButtonDown(0)) // При нажатии поставить здание 
                    PlaceFlyingBuilding(x, y);

                if (Input.GetMouseButtonDown(1)) // Отмена ставить строение
                {
                    Destroy(flyingBuilding.gameObject);
                    noteBlock.gameObject.SetActive(false);
                }
            }
        }
    }

    // Место занято?
    private bool IsPlaceTaken(int placeX, int placeY)
    {
        for (int x = 0; x < flyingBuilding.Size.x; x++)
        {
            for (int y = 0; y < flyingBuilding.Size.y; y++)
                if (grid[placeX + x, placeY + y] != null) return true;
        }

        return false;
    }

    // Поставить здание
    private void PlaceFlyingBuilding(int placeX, int placeY)
    {
        PlaceBuilding(flyingBuilding, placeX, placeY);
        
        // Кор
        flyingBuilding = null;
        noteBlock.gameObject.SetActive(false);
        buildingCreateMenu.gameObject.SetActive(false);
        
        // Колония менеджер
        scripts.colonyManager.CreateNewTask(BearTask.TasksMode.build, flyingBuilding.gameObject,
            flyingBuilding.stepsNeed);
        flyingBuilding.SetBuilding();
        scripts.colonyManager.Energy -= 1;
        scripts.colonyManager.Materials -= flyingBuilding.materialsNeed;
        
        // Логи на создание здания
        APIClient.Instance.CreateLogRequest(
            "Затраты энергии и материалов на постройку здания",
            Player.Instance.playerName,
            new Dictionary<string, string>()
            {
                {"energy", "-1"},
                {"materials", "-" + flyingBuilding.materialsNeed}
            }
        );
        
    }

    /// <summary>
    /// Поставит постройку по координатам
    /// Вынес в отдельный метод так как это вполне себе самостоятельный метод который можно использовать извне (Уже используется)
    /// </summary>
    /// <param name="building">Постройка которую нужно поставить</param>
    /// <param name="placeX">Координата по X</param>
    /// <param name="placeY">Координата по Y</param>
    public void PlaceBuilding(Building building, int placeX, int placeY)
    {
        for (int x = 0; x < building.Size.x; x++)
        {
            for (int y = 0; y < building.Size.y; y++)
                grid[placeX + x, placeY + y] = building;
        }
    }

    public void SetBuildSettings(GameObject totalBuild)
    {
        // Склады и тд
        Building build = totalBuild.GetComponent<Building>();
        selectedResource = build.typeResource;
        switch (selectedResource)
        {
            case ColonyManager.typeOfResource.materials:
                scripts.colonyManager.MaxMaterials += build.resourceGive;
                break;
            case ColonyManager.typeOfResource.materialPlus:
                scripts.colonyManager.MaxMaterialsPlus += build.resourceGive;
                break;
            case ColonyManager.typeOfResource.food:
                scripts.colonyManager.MaxFood += build.resourceGive;
                break;
            case ColonyManager.typeOfResource.honey:
                scripts.colonyManager.MaxHoney += build.resourceGive;
                break;
            case ColonyManager.typeOfResource.bioFuel:
                scripts.colonyManager.MaxBiofuel += build.resourceGive;
                break;
            case ColonyManager.typeOfResource.bears:
                scripts.colonyManager.maxBears += build.resourceGive;
                break;
            case ColonyManager.typeOfResource.energy:
                scripts.colonyManager.Energy += build.resourceGive;
                APIClient.Instance.CreateLogRequest(
                    "Повышение объема максимальной энергии",
                    Player.Instance.playerName,
                    new Dictionary<string, string> {{"energy", "+" + build.resourceGive}});
                scripts.colonyManager.MaxEnergy += build.resourceGive;
                break;
        }
    }
}