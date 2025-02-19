using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingSystem : MonoBehaviour
{
    public GameObject buildingCreateMenu, buildMenu;

    [SerializeField]
    private Vector2Int gridSize = new Vector2Int(10, 10); // Сетка строительсва. P.s значение в юньке не 10 10

    private GameObject _bgObj,
        _buildMenuStandartButtons,
        _buildMenuMaterialsButtons,
        _noteBlock,
        _droneManage,
        _bearManage;

    // Кнопки прибавления/убавления рабочих на "фабриках" или дронов
    private Button _buttonAddBear, _buttonAddDrone, _buttonRemoveBear, _buttonRemoveDrone;
    private Transform _buildCreateMenuBuildingsTransform;

    private TextMeshProUGUI _textSelectedBuild, _textNameBuild, _textInfoBuild, _textCountBears, _textCountDrones;

    [HideInInspector] public BuildingController selectedBuild; // Выбранное строение для взаимодействий(НЕ ДЛЯ СТРОЕНИЯ)
    [HideInInspector] public Resources selectedResource; // Выбранный ресурс(строение)
    private BuildingController[,] _grid; // Размещение строений на сетке
    private BuildingController _flyingBuildingController; // Выделенное строение
    private Camera _mainCamera;
    private AllScripts _scripts;

    private void Awake()
    {
        _grid = new BuildingController[gridSize.x, gridSize.y];
        _mainCamera = Camera.main;
        _scripts = GameObject.Find("scripts").GetComponent<AllScripts>();

        // Общая инициализация
        _noteBlock = buildingCreateMenu.transform.Find("NoteBlock")?.gameObject;
        _textSelectedBuild = _noteBlock?.transform.Find("TextSelectedBuild").GetComponent<TextMeshProUGUI>();
        _buildCreateMenuBuildingsTransform = buildingCreateMenu.transform.Find("Scroll View").transform.Find("Viewport")
            .transform.Find("Content");

        _bgObj = buildMenu.transform.Find("bg")?.gameObject;
        _buildMenuStandartButtons = _bgObj?.transform.Find("buildButtons")?.gameObject;
        _buildMenuMaterialsButtons = _bgObj?.transform.Find("materialStackButtons")?.gameObject;
        _textNameBuild = _bgObj?.transform.Find("TextName")?.GetComponent<TextMeshProUGUI>();

        _textInfoBuild = _buildMenuStandartButtons?.transform.Find("TextBuildInfo")?.GetComponent<TextMeshProUGUI>();

        _bearManage = _buildMenuStandartButtons?.transform.Find("bearManage")?.gameObject;
        _buttonAddBear = _bearManage?.transform.Find("ButtonAddBear")?.GetComponent<Button>();
        _buttonRemoveBear = _bearManage?.transform.Find("ButtonDeleteBear")?.GetComponent<Button>();
        _textCountBears = _bearManage?.transform.Find("TextBearCount")?.GetComponent<TextMeshProUGUI>();

        _droneManage = _buildMenuStandartButtons?.transform.Find("droneManage")?.gameObject;
        _buttonAddDrone = _droneManage?.transform.Find("ButtonAddDrone")?.GetComponent<Button>();
        _buttonRemoveDrone = _droneManage?.transform.Find("ButtonDeleteDrone")?.GetComponent<Button>();
        _textCountDrones = _droneManage?.transform.Find("TextDroneCount")?.GetComponent<TextMeshProUGUI>();
    }

    public void SelectBuildingToInteraction(BuildingController buildingController)
    {
        if (_scripts.CheckOpenedWindows(true)) // Если какая-то менюха уже открыта
            return;
        if (!buildingController.isReady) // Потом изменить
            return;
        selectedBuild = buildingController;
        ManageBuildMenu();
    }

    public void DisableBuildMenu() => ManageBuildMenu(false); // Для UI кнопки

    private void UpdateWorkersText()
    {
        _textInfoBuild.text = "+ " + (selectedBuild.workersOfBears + selectedBuild.workersOfDrone) *
            selectedBuild.building.resourceOneWorker;
        _textCountDrones.text = selectedBuild.workersOfDrone + "/" + selectedBuild.building.maxDrones;
        _textCountBears.text = selectedBuild.workersOfBears + "/" + selectedBuild.building.maxBears;
    }

    /// <summary>
    /// Изменение числа рабочих в здании
    /// </summary>
    /// <param name="changeWorkers">+/- столько-то рабочих</param>
    public void ManageWorkers(int changeWorkers)
    {
        selectedBuild.workersOfBears += changeWorkers;
        _scripts.colonyManager.workingBears += changeWorkers; // Костыль
        UpdateWorkersText();
        // TODO сделать условие для дронов и медведей selectedBuild.countOfBears == selectedBuild.maxBears
        if ((_scripts.colonyManager.bearsInColony.Count - _scripts.colonyManager.workingBears) == 0 ||
            selectedBuild.workersOfBears == selectedBuild.building.maxBears)
            _bearManage.transform.Find("ButtonAddBear").GetComponent<Button>().interactable = false;
        else
            ManageBuildMenu();
    }

    /// <summary>
    /// Изменение числа дронов в здании
    /// </summary>
    /// <param name="changeDrones">+/- столько-то дронов</param>
    public void ManageDrones(int changeDrones)
    {
        selectedBuild.workersOfDrone += changeDrones;
        _scripts.colonyManager.workingBears += changeDrones; // Костыль
        UpdateWorkersText();

        // TODO сделать условие для дронов и медведей selectedBuild.countOfBears == selectedBuild.maxBears
        if ((_scripts.colonyManager.bearsInColony.Count - _scripts.colonyManager.workingBears) == 0 ||
            selectedBuild.workersOfBears == selectedBuild.building.maxBears)
            _droneManage.transform.Find("ButtonAddBear").GetComponent<Button>().interactable = false;
        else
            ManageBuildMenu();
    }

    /// <summary>
    /// Управление панелью с выставлением числа рабочих/дронов
    /// </summary>
    /// <param name="bear">Могут работать медведи?</param>
    /// <param name="drone">Могут работать дроны?</param>
    private void ManageDroneAndBearPanel(bool bear, bool drone)
    {
        // Установка возможности нажать на кнопку, если в здании работают bear и(ли) drone
        _buttonAddBear.interactable = bear;
        _buttonRemoveBear.interactable = bear;
        _buttonAddDrone.interactable = drone;
        _buttonRemoveDrone.interactable = drone;

        if (!drone)
            _textCountDrones.text = "";
        if (!bear)
            _textCountBears.text = "";

        // Костыль
        if ((_scripts.colonyManager.bearsInColony.Count - _scripts.colonyManager.workingBears) == 0 ||
            selectedBuild.workersOfBears == selectedBuild.building.maxBears)
        {
            _buttonAddBear.interactable = false;
            _buttonAddDrone.interactable = false;
        }

        if ((_scripts.colonyManager.bearsInColony.Count - _scripts.colonyManager.workingBears) ==
            _scripts.colonyManager.maxBears || selectedBuild.workersOfBears == 0)
        {
            _buttonRemoveBear.interactable = false;
            _buttonRemoveDrone.interactable = false;
        }
    }

    /// <summary>
    /// Управление меню строительства
    /// </summary>
    /// <param name="open"></param>
    private void ManageBuildMenu(bool open = true)
    {
        buildMenu.gameObject.SetActive(open);
        if (open)
        {
            _textNameBuild.text = selectedBuild.building.buildingName;
            if (selectedBuild.building.typeOfBuilding == Building.TypesOfBuilding.Building) // Если здание
            {
                UpdateWorkersText();

                // Если в здании можно работать
                if (selectedBuild.building.canWork)
                {
                    if (selectedBuild.building.typeOfWorkers == Building.TypeOfWorkers.Any)
                        ManageDroneAndBearPanel(true, false); // Костыль пока дронов нету
                    else if (selectedBuild.building.typeOfWorkers == Building.TypeOfWorkers.Drone)
                        ManageDroneAndBearPanel(false, false);
                    else
                        ManageDroneAndBearPanel(true, false);
                }
                else
                    ManageDroneAndBearPanel(false, false);

                _buildMenuStandartButtons.transform.Find("ButtonDestroy").transform.Find("TextResourceReturn")
                    .GetComponent<TextMeshProUGUI>().text = (selectedBuild.building.materialsNeed / 2).ToString();
            }
            else // Если это ресурс
            {
                _buildMenuStandartButtons.gameObject.SetActive(false);
                _buildMenuMaterialsButtons.gameObject.SetActive(true);
            }
        }
    }

    public void StartPickUpResource()
    {
        selectedBuild.isReady = false;
        buildMenu.gameObject.SetActive(false);
        _scripts.colonyManager.CreateNewTask(TasksMode.GetResource, selectedBuild.gameObject,
            selectedBuild.building.stepsNeed);
    }

    public void PickUpResource(GameObject resourceObj)
    {
        selectedResource = resourceObj.GetComponent<BuildingController>().building.typeResource;
        string resourceChanged = ""; // Здесь хранится строчное представление ресурса, который изменили. Для логов
        int earn = 0; // Отдельная переменная, т.к. после свитчкейса нужно записать все в лог

        switch (selectedResource)
        {
            case Resources.Material:
                earn = Random.Range(15, 40);
                resourceChanged = "materials";
                _scripts.colonyManager.Materials += earn;
                break;
            case Resources.MaterialPlus:
                earn = Random.Range(1, 5);
                resourceChanged = "materialPlus";
                _scripts.colonyManager.MaterialsPlus += earn;
                break;
            case Resources.Food:
                earn = Random.Range(1, 10);
                resourceChanged = "food";
                _scripts.colonyManager.Food += earn;
                break;
            case Resources.Honey:
                earn = Random.Range(5, 10);
                resourceChanged = "honey";
                _scripts.colonyManager.Honey += earn;
                break;
            case Resources.BioFuel:
                earn = Random.Range(5, 15);
                resourceChanged = "bioFuel";
                _scripts.colonyManager.Biofuel += earn;
                break;
        }

        DestroyBuilding(resourceObj);
        // Лог
        APIClient.Instance.CreateLogRequest(
            "Новые ресурсы произведенные в результате работы некоторого строения",
            Player.Instance.playerName,
            new Dictionary<string, string>() { { resourceChanged, "+" + earn } });
    }

    public void
        StartPlacingBuilding(BuildingController buildingControllerPrefab) // Начинаем размещать объект. Метод для кнопки
    {
        if (_flyingBuildingController)
            Destroy(_flyingBuildingController.gameObject);

        _flyingBuildingController = Instantiate(buildingControllerPrefab);
        _noteBlock.gameObject.SetActive(true);
        _textSelectedBuild.text = _flyingBuildingController.building.buildingName;
    }

    public void DestroyBuilding(GameObject destroyBuilding = null)
    {
        if (destroyBuilding)
            selectedBuild = destroyBuilding.GetComponent<BuildingController>();

        if (selectedBuild)
        {
            // Получаем координаты здания на сетке
            int startX = Mathf.RoundToInt(selectedBuild.transform.position.x) - selectedBuild.building.size.x / 2;
            int startY = Mathf.RoundToInt(selectedBuild.transform.position.z) - selectedBuild.building.size.y / 2;

            // Удаляем здание из grid
            for (int x = 0; x < selectedBuild.building.size.x; x++)
            {
                for (int y = 0; y < selectedBuild.building.size.y; y++)
                    _grid[startX + x, startY + y] = null; // Очищаем ячейку
            }

            // Уничтожаем объект
            Destroy(selectedBuild.gameObject);
            _scripts.colonyManager.Materials += selectedBuild.building.materialsNeed / 2;
            if (selectedBuild.building.energyNeed != 0)
            {
                _scripts.colonyManager.Energy -= selectedBuild.building.energyNeed;

                APIClient.Instance.CreateLogRequest(
                    "Потрачена энергия на снос здания",
                    Player.Instance.playerName,
                    new Dictionary<string, string>() { { "energy", "-" + selectedBuild.building.energyNeed } });
            }

            selectedBuild = null;
            buildMenu.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) // Активация меню какое здание построить
        {
            if (!_scripts.CheckOpenedWindows(!buildingCreateMenu.activeSelf)) // Если какая-то менюха уже открыта
            {
                buildingCreateMenu.gameObject.SetActive(!buildingCreateMenu.activeSelf);
                _noteBlock.gameObject.SetActive(false);
                // Генерация списка зданий к постройке
                foreach (Transform child in _buildCreateMenuBuildingsTransform)
                {
                    // Единичная инициализация
                    BuildingController buildingController = child.gameObject.GetComponent<BuildingController>();

                    // Возможность нажать на кнопку
                    child.gameObject.GetComponent<Button>().interactable =
                        buildingController.building.materialsNeed <= _scripts.colonyManager.Materials &&
                        buildingController.building.energyNeed <= _scripts.colonyManager.Energy;
                    // Смена цены в зависимости от достатка
                    child.transform.Find("TextPrice").GetComponent<TextMeshProUGUI>().color =
                        buildingController.building.materialsNeed <= _scripts.colonyManager.Materials
                            ? Color.black
                            : Color.red;
                    child.transform.Find("TextPriceEnergy").GetComponent<TextMeshProUGUI>().color =
                        buildingController.building.energyNeed <= _scripts.colonyManager.Energy
                            ? Color.black
                            : Color.red;
                }

                if (!buildingCreateMenu.activeSelf && _flyingBuildingController)
                    Destroy(_flyingBuildingController.gameObject);
            }
        }

        // TODO: сделать отмену выбора текущего здания + смену выбранного здания на другое(оно мб работает)
        if (_flyingBuildingController) // Если зданиее не выделено
        {
            if (Input.GetKeyDown(KeyCode.R)) // Поворот здания
            {
                _flyingBuildingController.building.size = new Vector2Int(_flyingBuildingController.building.size.y,
                    _flyingBuildingController.building.size.x);
                _flyingBuildingController.transform.Rotate(0, 90f, 0);
            }

            var groundPlane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition); // Пускаем луч из камеры

            if (groundPlane.Raycast(ray, out float position)) // Хз это за хуйня
            {
                Vector3 worldPosition = ray.GetPoint(position);

                int x = Mathf.RoundToInt(worldPosition.x);
                int y = Mathf.RoundToInt(worldPosition.z);

                x -= _flyingBuildingController.building.size.x / 2;
                y -= _flyingBuildingController.building.size.y / 2;

                bool available = true;

                // Если здание за сеткой - помечать расположение недействительным
                if (x < -gridSize.x / 2 || x > gridSize.x / 2 - _flyingBuildingController.building.size.x)
                    available = false;
                if (y < -gridSize.y / 2 || y > gridSize.y / 2 - _flyingBuildingController.building.size.y)
                    available = false;

                // Если здание расположено на другом - помечать расположение недействительным
                if (available && IsPlaceTaken(x, y)) available = false;

                _flyingBuildingController.transform.position =
                    new Vector3(x + _flyingBuildingController.building.size.x / 2f, 0,
                        y + _flyingBuildingController.building.size.y / 2f);

                if (available && Input.GetMouseButtonDown(0)) // При нажатии поставить здание 
                    PlaceFlyingBuilding(x, y);

                if (Input.GetMouseButtonDown(1)) // Отмена ставить строение
                {
                    Destroy(_flyingBuildingController.gameObject);
                    _noteBlock.gameObject.SetActive(false);
                }

                _flyingBuildingController.SetTransparent(available); // Смена окраски
            }
        }
    }

    /// <summary>
    /// Проверка - занято ли место
    /// </summary>
    /// <param name="placeX">Место по x</param>
    /// <param name="placeY">Место по y</param>
    /// <returns></returns>
    private bool IsPlaceTaken(int placeX, int placeY)
    {
        for (int x = 0; x < _flyingBuildingController.building.size.x; x++)
        {
            for (int y = 0; y < _flyingBuildingController.building.size.y; y++)
                if (_grid[placeX + x, placeY + y])
                    return true;
        }

        return false;
    }

    /// <summary>
    /// Вызывает PlaceBuilding() и ставит выбранное(flying) здание
    /// </summary>
    /// <param name="placeX"></param>
    /// <param name="placeY"></param>
    private void PlaceFlyingBuilding(int placeX, int placeY)
    {
        PlaceBuilding(_flyingBuildingController, placeX, placeY);

        switch (_flyingBuildingController.building.buildingName)
        {
            // Кор
            case "Солнечная панель" when
                _scripts.questSystem.totalQuest.questName == "StartQuest":
            case "Фабрика материалов" when
                _scripts.questSystem.totalQuest.questName == "StartQuest":
                _scripts.questSystem.MoveNextStep();
                break;
        }

        _scripts.colonyManager.CreateNewTask(TasksMode.Build, _flyingBuildingController.gameObject,
            _flyingBuildingController.building.stepsNeed);
        _flyingBuildingController.SetBuilding();
        _scripts.colonyManager.Energy -= _flyingBuildingController.building.energyNeed;
        _scripts.colonyManager.Materials -= _flyingBuildingController.building.materialsNeed;

        // Логи на создание здания
        APIClient.Instance.CreateLogRequest(
            "Затраты энергии и материалов на постройку здания",
            Player.Instance.playerName,
            new Dictionary<string, string>()
            {
                { "energy", "-1" },
                { "materials", "-" + _flyingBuildingController.building.materialsNeed }
            }
        );
        _flyingBuildingController = null;
        _noteBlock.gameObject.SetActive(false);
        buildingCreateMenu.gameObject.SetActive(false);
    }

    /// <summary>
    /// Поставит постройку по координатам
    /// </summary>
    /// <param name="buildingController">Постройка которую нужно поставить</param>
    /// <param name="placeX">Координата по X</param>
    /// <param name="placeY">Координата по Y</param>
    private void PlaceBuilding(BuildingController buildingController, int placeX, int placeY)
    {
        for (int x = 0; x < buildingController.building.size.x; x++)
        {
            for (int y = 0; y < buildingController.building.size.y; y++)
                _grid[placeX + x, placeY + y] = buildingController;
        }
    }

    /// <summary>
    /// Устанавливает бафы после постройки здания
    /// </summary>
    /// <param name="build"></param>
    public void SetBuildSettings(BuildingController build)
    {
        // Склады и тд
        selectedResource = build.building.typeResource;
        string resource = "";
        switch (selectedResource)
        {
            case Resources.Material:
                _scripts.colonyManager.MaxMaterials += build.building.resourceGive;
                resource = "maxMaterials";
                break;
            case Resources.MaterialPlus:
                _scripts.colonyManager.MaxMaterialsPlus += build.building.resourceGive;
                resource = "maxMaterialsPlus";
                break;
            case Resources.Food:
                _scripts.colonyManager.MaxFood += build.building.resourceGive;
                resource = "maxFood";
                break;
            case Resources.Honey:
                _scripts.colonyManager.MaxHoney += build.building.resourceGive;
                resource = "maxHoney";
                break;
            case Resources.BioFuel:
                _scripts.colonyManager.MaxBiofuel += build.building.resourceGive;
                resource = "maxBiofuel";
                break;
            case Resources.Bears:
                _scripts.colonyManager.maxBears += build.building.resourceGive;
                resource = "maxBears";
                break;
            case Resources.Energy:
                _scripts.colonyManager.Energy += build.building.resourceGive;
                _scripts.colonyManager.MaxEnergy += build.building.resourceGive;
                resource = "energy";
                break;
        }

        APIClient.Instance.CreateLogRequest(
            "Повышение лимитов за здание",
            Player.Instance.playerName,
            new Dictionary<string, string> { { resource, "+" + build.building.resourceGive } });
    }
}