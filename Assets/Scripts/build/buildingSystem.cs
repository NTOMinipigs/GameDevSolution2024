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

    [HideInInspector] public Building selectedBuild; // Выбранное строение для взаимодействий(НЕ ДЛЯ СТРОЕНИЯ)
    [HideInInspector] public Resources selectedResource; // Выбранный ресурс(строение)
    private Building[,] _grid; // Размещение строений на сетке
    private Building _flyingBuilding; // Выделенное строение
    private Camera _mainCamera;
    private allScripts _scripts;

    private void Awake()
    {
        _grid = new Building[gridSize.x, gridSize.y];
        _mainCamera = Camera.main;
        _scripts = GameObject.Find("scripts").GetComponent<allScripts>();

        // Общая инициализация
        _noteBlock = buildingCreateMenu.transform.Find("NoteBlock")?.GetComponent<GameObject>();
        _textSelectedBuild = _noteBlock?.transform.Find("TextSelectedBuild")?.GetComponent<TextMeshProUGUI>();
        _buildCreateMenuBuildingsTransform = buildingCreateMenu.transform.Find("Scroll View").transform.Find("Viewport")
            .transform.Find("Content");

        _bgObj = buildMenu.transform.Find("bg")?.GetComponent<GameObject>();
        _buildMenuStandartButtons = _bgObj?.transform.Find("buildButtons")?.GetComponent<GameObject>();
        _buildMenuMaterialsButtons = _bgObj?.transform.Find("materialStackButtons")?.GetComponent<GameObject>();
        _textNameBuild = _bgObj?.transform.Find("TextName")?.GetComponent<TextMeshProUGUI>();

        _textInfoBuild = _buildMenuStandartButtons?.transform.Find("TextBuildInfo")?.GetComponent<TextMeshProUGUI>();

        _bearManage = _buildMenuStandartButtons?.transform.Find("bearManage")?.GetComponent<GameObject>();
        _buttonAddBear = _bearManage?.transform.Find("ButtonAddBear")?.GetComponent<Button>();
        _buttonRemoveBear = _bearManage?.transform.Find("ButtonDeleteBear")?.GetComponent<Button>();
        _textCountBears = _bearManage?.transform.Find("TextBearCount")?.GetComponent<TextMeshProUGUI>();

        _droneManage = _buildMenuStandartButtons?.transform.Find("droneManage")?.GetComponent<GameObject>();
        _buttonAddDrone = _droneManage?.transform.Find("ButtonAddDrone")?.GetComponent<Button>();
        _buttonRemoveDrone = _droneManage?.transform.Find("ButtonDeleteDrone")?.GetComponent<Button>();
        _textCountDrones = _droneManage?.transform.Find("TextDroneCount")?.GetComponent<TextMeshProUGUI>();
    }

    public void SelectBuildingToInteraction(Building building)
    {
        if (_scripts.CheckOpenedWindows(true)) // Если какая-то менюха уже открыта
            return;
        if (!building.builded) // Потом изменить
            return;
        selectedBuild = building;
        ManageBuildMenu();
    }

    public void DisableBuildMenu() => ManageBuildMenu(false); // Для UI кнопки

    private void UpdateWorkersText()
    {
        _textInfoBuild.text = "+ " + (selectedBuild.countOfBears + selectedBuild.countOfDrone) *
            selectedBuild.resourceOneWorker;
        _textCountDrones.text = selectedBuild.countOfDrone + "/" + selectedBuild.maxDrones;
        _textCountBears.text = selectedBuild.countOfBears + "/" + selectedBuild.maxBears;
    }

    /// <summary>
    /// Изменение числа рабочих в здании
    /// </summary>
    /// <param name="changeWorkers">+/- столько-то рабочих</param>
    public void ManageWorkers(int changeWorkers)
    {
        selectedBuild.countOfBears += changeWorkers;
        _scripts.colonyManager.workingBears += changeWorkers; // Костыль
        UpdateWorkersText();
        // TODO сделать условие для дронов и медведей selectedBuild.countOfBears == selectedBuild.maxBears
        if ((_scripts.colonyManager.bearsInColony.Count - _scripts.colonyManager.workingBears) == 0 ||
            selectedBuild.countOfBears == selectedBuild.maxBears)
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
        selectedBuild.countOfDrone += changeDrones;
        _scripts.colonyManager.workingBears += changeDrones; // Костыль
        UpdateWorkersText();

        // TODO сделать условие для дронов и медведей selectedBuild.countOfBears == selectedBuild.maxBears
        if ((_scripts.colonyManager.bearsInColony.Count - _scripts.colonyManager.workingBears) == 0 ||
            selectedBuild.countOfBears == selectedBuild.maxBears)
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
            selectedBuild.countOfBears == selectedBuild.maxBears)
        {
            _buttonAddBear.interactable = false;
            _buttonAddDrone.interactable = false;
        }

        if ((_scripts.colonyManager.bearsInColony.Count - _scripts.colonyManager.workingBears) ==
            _scripts.colonyManager.maxBears || selectedBuild.countOfBears == 0)
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
            _textNameBuild.text = selectedBuild.buildingName;
            if (selectedBuild.typeOfBuilding == Building.TypesOfBuilding.Building) // Если здание
            {
                UpdateWorkersText();

                // Если в здании можно работать
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

                _buildMenuStandartButtons.transform.Find("ButtonDestroy").transform.Find("TextResourceReturn")
                    .GetComponent<TextMeshProUGUI>().text = (selectedBuild.materialsNeed / 2).ToString();
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
        selectedBuild.builded = false;
        buildMenu.gameObject.SetActive(false);
        _scripts.colonyManager.CreateNewTask(TasksMode.GetResource, selectedBuild.gameObject, selectedBuild.stepsNeed);
    }

    public void PickUpResource(GameObject resourceObj)
    {
        selectedResource = resourceObj.GetComponent<Building>().typeResource;
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

    public void StartPlacingBuilding(Building buildingPrefab) // Начинаем размещать объект. Метод для кнопки
    {
        if (_flyingBuilding)
            Destroy(_flyingBuilding.gameObject);

        _flyingBuilding = Instantiate(buildingPrefab);
        _noteBlock.gameObject.SetActive(true);
        _textSelectedBuild.text = _flyingBuilding.buildingName;
    }

    public void DestroyBuilding(GameObject destroyBuilding = null)
    {
        if (destroyBuilding)
            selectedBuild = destroyBuilding.GetComponent<Building>();

        if (selectedBuild)
        {
            // Получаем координаты здания на сетке
            int startX = Mathf.RoundToInt(selectedBuild.transform.position.x) - selectedBuild.size.x / 2;
            int startY = Mathf.RoundToInt(selectedBuild.transform.position.z) - selectedBuild.size.y / 2;

            // Удаляем здание из grid
            for (int x = 0; x < selectedBuild.size.x; x++)
            {
                for (int y = 0; y < selectedBuild.size.y; y++)
                    _grid[startX + x, startY + y] = null; // Очищаем ячейку
            }

            // Уничтожаем объект
            Destroy(selectedBuild.gameObject);
            _scripts.colonyManager.Materials += selectedBuild.materialsNeed / 2;
            if (selectedBuild.energyNeed != 0)
            {
                _scripts.colonyManager.Energy -= selectedBuild.energyNeed;

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
            if (!_scripts.CheckOpenedWindows(!buildingCreateMenu.activeSelf)) // Если какая-то менюха уже открыта
            {
                buildingCreateMenu.gameObject.SetActive(!buildingCreateMenu.activeSelf);
                _noteBlock.gameObject.SetActive(false);
                // Генерация списка зданий к постройке
                foreach (Transform child in _buildCreateMenuBuildingsTransform)
                {
                    // Единичная инициализация
                    Building building = child.gameObject.GetComponent<Building>();

                    // Возможность нажать на кнопку
                    child.gameObject.GetComponent<Button>().interactable =
                        building.materialsNeed <= _scripts.colonyManager.Materials &&
                        building.energyNeed <= _scripts.colonyManager.Energy;
                    // Смена цены в зависимости от достатка
                    child.transform.Find("TextPrice").GetComponent<TextMeshProUGUI>().color =
                        building.materialsNeed <= _scripts.colonyManager.Materials ? Color.black : Color.red;
                    child.transform.Find("TextPriceEnergy").GetComponent<TextMeshProUGUI>().color =
                        building.energyNeed <= _scripts.colonyManager.Energy ? Color.black : Color.red;
                }

                if (!buildingCreateMenu.activeSelf && _flyingBuilding)
                    Destroy(_flyingBuilding.gameObject);
            }
        }

        // TODO: сделать отмену выбора текущего здания + смену выбранного здания на другое(оно мб работает)
        if (_flyingBuilding) // Если зданиее не выделено
        {
            if (Input.GetKeyDown(KeyCode.R)) // Поворот здания
            {
                _flyingBuilding.size = new Vector2Int(_flyingBuilding.size.y, _flyingBuilding.size.x);
                _flyingBuilding.transform.Rotate(0, 90f, 0);
            }

            var groundPlane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition); // Пускаем луч из камеры

            if (groundPlane.Raycast(ray, out float position)) // Хз это за хуйня
            {
                Vector3 worldPosition = ray.GetPoint(position);

                int x = Mathf.RoundToInt(worldPosition.x);
                int y = Mathf.RoundToInt(worldPosition.z);

                x -= _flyingBuilding.size.x / 2;
                y -= _flyingBuilding.size.y / 2;

                bool available = true;

                // Если здание за сеткой - помечать расположение недействительным
                if (x < -gridSize.x / 2 || x > gridSize.x / 2 - _flyingBuilding.size.x) available = false;
                if (y < -gridSize.y / 2 || y > gridSize.y / 2 - _flyingBuilding.size.y) available = false;

                // Если здание расположено на другом - помечать расположение недействительным
                if (available && IsPlaceTaken(x, y)) available = false;

                _flyingBuilding.transform.position =
                    new Vector3(x + _flyingBuilding.size.x / 2f, 0, y + _flyingBuilding.size.y / 2f);
                _flyingBuilding.SetTransparent(available); // Смена окраски

                if (available && Input.GetMouseButtonDown(0)) // При нажатии поставить здание 
                    PlaceFlyingBuilding(x, y);

                if (Input.GetMouseButtonDown(1)) // Отмена ставить строение
                {
                    Destroy(_flyingBuilding.gameObject);
                    _noteBlock.gameObject.SetActive(false);
                }
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
        for (int x = 0; x < _flyingBuilding.size.x; x++)
        {
            for (int y = 0; y < _flyingBuilding.size.y; y++)
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
        PlaceBuilding(_flyingBuilding, placeX, placeY);

        switch (_flyingBuilding.buildingName)
        {
            // Кор
            case "Солнечная панель" when
                _scripts.questSystem.totalQuest.questName == "StartQuest":
            case "Фабрика материалов" when
                _scripts.questSystem.totalQuest.questName == "StartQuest":
                _scripts.questSystem.MoveNextStep();
                break;
        }
        _scripts.colonyManager.CreateNewTask(TasksMode.Build, _flyingBuilding.gameObject, _flyingBuilding.stepsNeed);
        _flyingBuilding.SetBuilding();
        _scripts.colonyManager.Energy -= _flyingBuilding.energyNeed;
        _scripts.colonyManager.Materials -= _flyingBuilding.materialsNeed;

        // Логи на создание здания
        APIClient.Instance.CreateLogRequest(
            "Затраты энергии и материалов на постройку здания",
            Player.Instance.playerName,
            new Dictionary<string, string>()
            {
                { "energy", "-1" },
                { "materials", "-" + _flyingBuilding.materialsNeed }
            }
        );
        _flyingBuilding = null;
        _noteBlock.gameObject.SetActive(false);
        buildingCreateMenu.gameObject.SetActive(false);
    }

    /// <summary>
    /// Поставит постройку по координатам
    /// </summary>
    /// <param name="building">Постройка которую нужно поставить</param>
    /// <param name="placeX">Координата по X</param>
    /// <param name="placeY">Координата по Y</param>
    private void PlaceBuilding(Building building, int placeX, int placeY)
    {
        for (int x = 0; x < building.size.x; x++)
        {
            for (int y = 0; y < building.size.y; y++)
                _grid[placeX + x, placeY + y] = building;
        }
    }

    /// <summary>
    /// Устанавливает бафы после постройки здания
    /// </summary>
    /// <param name="build"></param>
    public void SetBuildSettings(Building build)
    {
        // Склады и тд
        selectedResource = build.typeResource;
        string resource = "";
        switch (selectedResource)
        {
            case Resources.Material:
                _scripts.colonyManager.MaxMaterials += build.resourceGive;
                resource = "maxMaterials";
                break;
            case Resources.MaterialPlus:
                _scripts.colonyManager.MaxMaterialsPlus += build.resourceGive;
                resource = "maxMaterialsPlus";
                break;
            case Resources.Food:
                _scripts.colonyManager.MaxFood += build.resourceGive;
                resource = "maxFood";
                break;
            case Resources.Honey:
                _scripts.colonyManager.MaxHoney += build.resourceGive;
                resource = "maxHoney";
                break;
            case Resources.BioFuel:
                _scripts.colonyManager.MaxBiofuel += build.resourceGive;
                resource = "maxBiofuel";
                break;
            case Resources.Bears:
                _scripts.colonyManager.maxBears += build.resourceGive;
                resource = "maxBears";
                break;
            case Resources.Energy:
                _scripts.colonyManager.Energy += build.resourceGive;
                _scripts.colonyManager.MaxEnergy += build.resourceGive;
                resource = "energy";
                break;
        }

        APIClient.Instance.CreateLogRequest(
            "Повышение лимитов за здание",
            Player.Instance.playerName,
            new Dictionary<string, string> { { resource, "+" + build.resourceGive } });
    }
}