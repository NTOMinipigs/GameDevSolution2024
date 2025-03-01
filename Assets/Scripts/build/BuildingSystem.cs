using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class BuildingSystem : MonoBehaviour
{
    public GameObject buildingCreateMenu, buildMenu;

    [SerializeField]
    private Vector2Int gridSize = new Vector2Int(10, 10); // Сетка строительсва. P.s значение в юньке не 10 10

    private BuildingController _selectedBuildController; // Выбранное строение для взаимодействий
    private Building _selectedBuilding;

    private BuildingController[,] _grid; // Сетка строений
    private BuildingController _flyingBuildingController; // Строение в процессе постановки
    private Camera _mainCamera;
    private AllScripts _scripts;

    // UI
    private GameObject _bgObj,
        _buildMenuBuildings,
        _buildMenuResources,
        _noteBlock,
        _droneManage,
        _bearManage,
        _workerButtons;

    [SerializeField] private GameObject resourceBlockAdd, resourceBlockRemove;
    private TextMeshProUGUI _resourceAddText, _resourceRemoveText;
    private Image _resourceAddImage, _resourceRemoveImage;
    [SerializeField] private Button buttonAddWorker, buttonRemoveWorker;

    [SerializeField] private TextMeshProUGUI textCountWorkers, textNameWorkers, textDestroy;
    [SerializeField] private TextMeshProUGUI textHealth, textEnergy;

    private Transform _buildCreateMenuBuildingsTransform;

    private TextMeshProUGUI _textSelectedBuild, _textNameBuild, _textInfoBuild;

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
        _buildMenuBuildings = _bgObj?.transform.Find("functionalBuildMode")?.gameObject;
        _buildMenuResources = _bgObj?.transform.Find("materialStackMode")?.gameObject;

        _textNameBuild = _bgObj?.transform.Find("TextName")?.GetComponent<TextMeshProUGUI>();
        _textInfoBuild = _bgObj?.transform.Find("TextBuildInfo")?.GetComponent<TextMeshProUGUI>();

        _workerButtons = _buildMenuBuildings?.transform.Find("workersButtons")?.gameObject;

        _resourceAddText = resourceBlockAdd.transform.Find("TextAddResource").GetComponent<TextMeshProUGUI>();
        _resourceRemoveText = resourceBlockRemove.transform.Find("TextRemoveResource").GetComponent<TextMeshProUGUI>();
        _resourceAddImage = resourceBlockAdd.transform.Find("ResourceIcon").GetComponent<Image>();
        _resourceRemoveImage = resourceBlockAdd.transform.Find("ResourceIcon").GetComponent<Image>();
    }

    public void SelectBuildingToInteraction(BuildingController buildingController)
    {
        if (_scripts.CheckOpenedWindows(true)) // Если какая-то менюха уже открыта
            return;
        if (!buildingController.isReady) // Потом изменить
            return;
        _selectedBuildController = buildingController;
        ManageBuildMenu();
    }

    public void DisableBuildMenu() => ManageBuildMenu(false); // Для UI кнопки

    private void UpdateBuildingText()
    {
        textHealth.text = "Состояние: " + _selectedBuildController.health + "%";
        textEnergy.text = "Потребление энергии: -" + _selectedBuilding.energyNeed;
        textNameWorkers.text = _selectedBuilding.typeOfWorkers.ToString();
        textCountWorkers.text = _selectedBuildController.workersCount + "/" +
                                _selectedBuildController.Building.MaxWorkers;
        if (_selectedBuildController.workersCount > 0) // т.е работает
        {
            resourceBlockAdd.gameObject.SetActive(true);
            _resourceAddText.text = "+" + _selectedBuildController.workersCount * _selectedBuilding.resourceOneWorker +
                                    " " + _selectedBuilding.typeResource;
        }
        else
            resourceBlockAdd.gameObject.SetActive(false);
    }

    /// <summary>
    /// Изменение числа рабочих в здании
    /// </summary>
    /// <param name="changeWorkers">+/- столько-то рабочих</param>
    public void ManageWorkers(int changeWorkers)
    {
        _selectedBuildController.workersCount += changeWorkers;
        _scripts.colonyManager.workingBears += changeWorkers; // Костыль
        UpdateBuildingText();
        // TODO сделать условие для ОПРЕДЕЛЕННОЙ ТРАДИЦИИ
        if (_scripts.colonyManager.bearsInColony.Count - _scripts.colonyManager.workingBears == 0 ||
            _selectedBuildController.workersCount == _selectedBuildController.Building.MaxWorkers)
            buttonAddWorker.interactable = false;
        else
            ManageBuildMenu();
    }

    /// <summary>
    /// Управление панелью с выставлением числа рабочих/дронов
    /// </summary>
    /// <param name="canWork">Можно работать?</param>
    private void ManageWorkersPanel(bool canWork)
    {
        _workerButtons.gameObject.SetActive(_selectedBuilding.canWork);

        buttonAddWorker.interactable = canWork;
        buttonRemoveWorker.interactable = canWork;

        // Костыль
        if (_scripts.colonyManager.bearsInColony.Count - _scripts.colonyManager.workingBears == 0 ||
            _selectedBuildController.workersCount == _selectedBuildController.Building.MaxWorkers)
            buttonAddWorker.interactable = false;

        if (_scripts.colonyManager.bearsInColony.Count - _scripts.colonyManager.workingBears ==
            _scripts.colonyManager.maxBears || _selectedBuildController.workersCount == 0)
            buttonRemoveWorker.interactable = false;
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
            _textNameBuild.text = _selectedBuildController.Building.BuildingName;
            _textInfoBuild.text = _selectedBuildController.Building.Description;
            if (_selectedBuildController.Building is Building building) // Если здание
            {
                _selectedBuilding = building;
                _buildMenuBuildings.gameObject.SetActive(true);
                ManageWorkersPanel(building.canWork);
                UpdateBuildingText();
                textDestroy.text = (building.materialsNeed / 2).ToString();
            }
            else // Если это ресурс
            {
                _buildMenuBuildings.gameObject.SetActive(false);
                _buildMenuResources.gameObject.SetActive(true);
            }
        }
    }

    public void StartPickUpResource()
    {
        _selectedBuildController.isReady = false;
        buildMenu.gameObject.SetActive(false);
        // На переделке
        //_scripts.colonyManager.CreateNewTask(TasksMode.GetResource, selectedBuild.gameObject, selectedBuild.Building.stepsNeed);
    }

    public void PickUpResource(GameObject resourceObj)
    {
        Resources selectedResource = resourceObj.GetComponent<BuildingController>().Building.TypeResource;
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

    // Начинаем размещать объект. Метод для кнопки
    public void StartPlacingBuilding(BuildingController buildingControllerPrefab)
    {
        if (_flyingBuildingController)
            Destroy(_flyingBuildingController.gameObject);

        _flyingBuildingController = Instantiate(buildingControllerPrefab);
        _noteBlock.gameObject.SetActive(true);
        _textSelectedBuild.text = _flyingBuildingController.Building.BuildingName;
    }

    public void DestroyBuilding(GameObject destroyBuilding = null)
    {
        if (destroyBuilding)
            _selectedBuildController = destroyBuilding.GetComponent<BuildingController>();

        if (_selectedBuildController)
        {
            // Получаем координаты здания на сетке
            int startX = Mathf.RoundToInt(_selectedBuildController.transform.position.x) -
                         _selectedBuildController.size.x / 2;
            int startY = Mathf.RoundToInt(_selectedBuildController.transform.position.z) -
                         _selectedBuildController.size.y / 2;

            // Удаляем здание из grid
            for (int x = 0; x < _selectedBuildController.size.x; x++)
            {
                for (int y = 0; y < _selectedBuildController.size.y; y++)
                    _grid[startX + x, startY + y] = null; // Очищаем ячейку
            }

            _scripts.buildingSaveSystem.DestroyBuildingSave(
                (int)_selectedBuildController.transform.position.x,
                (int)_selectedBuildController.transform.position.z); // Удаляем buildingsave из списка сохраненных
            // Уничтожаем объект
            Destroy(_selectedBuildController.gameObject);

            _scripts.colonyManager.Materials += _selectedBuilding.materialsNeed / 2;
            if (_selectedBuilding.energyNeed != 0)
            {
                _scripts.colonyManager.Energy -= _selectedBuilding.energyNeed;

                APIClient.Instance.CreateLogRequest(
                    "Потрачена энергия на снос здания",
                    Player.Instance.playerName,
                    new Dictionary<string, string>() { { "energy", "-" + _selectedBuilding.energyNeed } });
            }

            _selectedBuildController = null;
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
                    BuildingBuyInfo building = child.GetComponent<BuildingBuyInfo>();
                    if (building.building) // Всякое бывает
                    {
                        // Возможность нажать на кнопку
                        building.button.interactable =
                            building.building.materialsNeed <= _scripts.colonyManager.Materials &&
                            building.building.energyNeed <= _scripts.colonyManager.Energy;
                        // Смена цены в зависимости от достатка
                        building.textPriceMaterial.color =
                            building.building.materialsNeed <= _scripts.colonyManager.Materials
                                ? Color.black
                                : Color.red;
                        building.textPriceEnergy.color =
                            building.building.energyNeed <= _scripts.colonyManager.Energy
                                ? Color.black
                                : Color.red;
                    }
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
                _flyingBuildingController.size = new Vector2Int(_flyingBuildingController.size.y,
                    _flyingBuildingController.size.x);
                _flyingBuildingController.transform.Rotate(0, 90f, 0);
            }

            var groundPlane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition); // Пускаем луч из камеры

            if (groundPlane.Raycast(ray, out float position)) // Хз это за хуйня
            {
                Vector3 worldPosition = ray.GetPoint(position);

                int x = Mathf.RoundToInt(worldPosition.x);
                int y = Mathf.RoundToInt(worldPosition.z);

                x -= _flyingBuildingController.size.x / 2;
                y -= _flyingBuildingController.size.y / 2;

                bool available = true;

                // Если здание за сеткой - помечать расположение недействительным
                if (x < -gridSize.x / 2 || x > gridSize.x / 2 - _flyingBuildingController.size.x)
                    available = false;
                if (y < -gridSize.y / 2 || y > gridSize.y / 2 - _flyingBuildingController.size.y)
                    available = false;

                // Если здание расположено на другом - помечать расположение недействительным
                if (available && IsPlaceTaken(x, y)) available = false;

                _flyingBuildingController.transform.position =
                    new Vector3(x + _flyingBuildingController.size.x / 2f, 0,
                        y + _flyingBuildingController.size.y / 2f);

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
        for (int x = 0; x < _flyingBuildingController.size.x; x++)
        {
            for (int y = 0; y < _flyingBuildingController.size.y; y++)
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
        if (_flyingBuildingController.Building is Building building)
        {
            PlaceBuilding(_flyingBuildingController, placeX, placeY);
            _scripts.buildingSaveSystem.CreateBuildSave(placeX, placeY, _flyingBuildingController.name); // Создаем сохранение постройки

            switch (_flyingBuildingController.Building.BuildingName)
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
                building.stepsNeed);
            _flyingBuildingController.SetBuilding();
            _scripts.colonyManager.Energy -= building.energyNeed;
            _scripts.colonyManager.Materials -= building.materialsNeed;

            // Логи на создание здания
            APIClient.Instance.CreateLogRequest(
                "Затраты энергии и материалов на постройку здания",
                Player.Instance.playerName,
                new Dictionary<string, string>()
                {
                    { "energy", "-1" },
                    { "materials", "-" + building.materialsNeed }
                }
            );
        }

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
    public void PlaceBuilding(BuildingController buildingController, int placeX, int placeY)
    {
        for (int x = 0; x < buildingController.size.x; x++)
        {
            for (int y = 0; y < buildingController.size.y; y++)
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
        Resources selectedResource = build.Building.TypeResource;
        if (build.Building is Building building)
        {
            string resource = "";
            switch (selectedResource)
            {
                case Resources.Material:
                    _scripts.colonyManager.MaxMaterials += building.resourceGive;
                    resource = "maxMaterials";
                    break;
                case Resources.MaterialPlus:
                    _scripts.colonyManager.MaxMaterialsPlus += building.resourceGive;
                    resource = "maxMaterialsPlus";
                    break;
                case Resources.Food:
                    _scripts.colonyManager.MaxFood += building.resourceGive;
                    resource = "maxFood";
                    break;
                case Resources.Honey:
                    _scripts.colonyManager.MaxHoney += building.resourceGive;
                    resource = "maxHoney";
                    break;
                case Resources.BioFuel:
                    _scripts.colonyManager.MaxBiofuel += building.resourceGive;
                    resource = "maxBiofuel";
                    break;
                case Resources.Bears:
                    _scripts.colonyManager.maxBears += building.resourceGive;
                    resource = "maxBears";
                    break;
                case Resources.Energy:
                    _scripts.colonyManager.Energy += building.resourceGive;
                    _scripts.colonyManager.MaxEnergy += building.resourceGive;
                    resource = "energy";
                    break;
            }

            APIClient.Instance.CreateLogRequest(
                "Повышение лимитов за здание",
                Player.Instance.playerName,
                new Dictionary<string, string> { { resource, "+" + building.resourceGive } });
        }
    }
}