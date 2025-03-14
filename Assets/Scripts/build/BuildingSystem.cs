using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class BuildingSystem : MonoBehaviour
{
    public static BuildingSystem Singleton { get; private set; }
    public GameObject buildingCreateMenu, buildMenu;

    [Header("Grid")]
    public Vector2Int gridSize = new Vector2Int(10, 10); // Сетка строительсва. P.s значение в юньке не 10 10
    public BuildingController[,] Grid; // Сетка строений

    private BuildingController _selectedBuildController; // Выбранное строение для взаимодействий
    private Building _selectedBuilding;

    private BuildingController _flyingBuildingController; // Строение в процессе постановки
    private Camera _mainCamera;

    // UI
    private GameObject _bgObj,
        _buildMenuBuildings,
        _buildMenuResources,
        _noteBlock,
        _droneManage,
        _bearManage,
        _workerButtons;

    [SerializeField] private GameObject buildingsBearMenu, buildingsWorkMenu, buildingsSienceMenu;
    [SerializeField] private GameObject resourceBlockAdd, resourceBlockRemove;
    private TextMeshProUGUI _resourceAddText, _resourceRemoveText;
    private Image _resourceAddImage, _resourceRemoveImage;
    [SerializeField] private Button buttonAddWorker, buttonRemoveWorker;
    [SerializeField] private Button buttonAddWorkerResources, buttonRemoveWorkerResources;

    [SerializeField]
    private TextMeshProUGUI textCountWorkers, textCountWorkersResource, textNameWorkers, textDestroy, textStop;

    [SerializeField] private TextMeshProUGUI textHealth, textEnergy, textResourceName, textResourceRemain;


    private TextMeshProUGUI _textSelectedBuild, _textNameBuild, _textInfoBuild;

    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        _mainCamera = Camera.main;

        // Общая инициализация
        _noteBlock = buildingCreateMenu.transform.Find("NoteBlock")?.gameObject;
        _textSelectedBuild = _noteBlock?.transform.Find("TextSelectedBuild").GetComponent<TextMeshProUGUI>();

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
        if (GameMenuManager.Singleton.CheckOpenedWindows()) // Если какая-то менюха уже открыта
            return;
        _selectedBuildController = buildingController;
        ManageBuildMenu();
    }

    # region WorkersManager

    /// <summary>
    /// Управление панелью с выставлением числа рабочих/дронов
    /// </summary>
    /// <param name="canWork">Можно работать?</param>
    /// <param name="resource">Это ресурс?</param>
    private void ManageWorkersPanel(bool canWork, bool resource = false)
    {
        if (!resource) // Если это не ресурс
        {
            int freeWorkersOfTradition =
                ColonyManager.Singleton.GetCountFreeBearsOfTradition(_selectedBuilding.typeOfWorkers);
            _workerButtons.gameObject.SetActive(_selectedBuilding.canWork && _selectedBuildController.isBuild);

            buttonAddWorker.interactable = canWork && freeWorkersOfTradition != 0;
            buttonRemoveWorker.interactable = canWork && _selectedBuildController.workersCount != 0;
        }
        else // Если ресурс
        {
            int freeWorkersOfTradition =
                ColonyManager.Singleton.GetCountFreeBearsOfTradition(Traditions.Drone);
            buttonAddWorkerResources.interactable = canWork && freeWorkersOfTradition != 0;
            buttonRemoveWorkerResources.interactable = canWork && _selectedBuildController.workersCount != 0;
        }
    }

    /// <summary>
    /// Добавление рабочего в здание
    /// </summary>
    public void AddWorkerToBuilding()
    {
        _selectedBuildController.workersCount++;
        BearTaskManager.Singleton.CreateNewTask(TasksMode.Create, _selectedBuildController.gameObject,
            _selectedBuilding.typeOfWorkers, -1f);

        ManageBuildMenu();
    }

    /// <summary>
    /// Добавление рабочего в здание
    /// </summary>
    public void AddWorkerToResource()
    {
        _selectedBuildController.workersCount++;
        BearTaskManager.Singleton.CreateNewTask(TasksMode.GetResource, _selectedBuildController.gameObject,
            Traditions.Drone, -1f);

        ManageBuildMenu();
    }

    /// <summary>
    /// Убрать рабочего из здания/руды
    /// </summary>
    public void RemoveWorker(bool resource = false)
    {
        _selectedBuildController.workersCount--;
        if (!resource)
        {
            UpdateBuildingText();
            BearTaskManager.Singleton.FindAndEndTask(_selectedBuilding.typeOfWorkers,
                _selectedBuildController.gameObject);
        }
        else
        {
            UpdateResourceText();
            BearTaskManager.Singleton.FindAndEndTask(Traditions.Drone, _selectedBuildController.gameObject);
        }

        ManageBuildMenu();
    }

    #endregion

    #region BuildMenu

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
                _buildMenuResources.gameObject.SetActive(false);
                ManageWorkersPanel(building.canWork);
                UpdateBuildingText();
                textDestroy.text = (building.materialsNeed / 2).ToString();
            }
            else // Если ресурс
            {
                _buildMenuBuildings.gameObject.SetActive(false);
                _buildMenuResources.gameObject.SetActive(true);
                ManageWorkersPanel(true, true);
                UpdateResourceText();
            }
        }
    }

    public void DisableBuildMenu() => ManageBuildMenu(false); // Для UI кнопки

    public void UpdateBuildingText()
    {
        if (_selectedBuildController.isBuild)
        {
            textHealth.text = "Состояние: " + _selectedBuildController.health + "%";
            if (_selectedBuildController.isReady)
                textEnergy.text = "Потребление энергии: -" + _selectedBuilding.energyNeed;
            else
                textEnergy.text = "Потребление энергии: -" + 0;
            textNameWorkers.text = _selectedBuilding.typeOfWorkers.GetString();
            textCountWorkers.text = _selectedBuildController.workersCount + "/" +
                                    _selectedBuildController.Building.MaxWorkers;
            if (_selectedBuildController.workersCount > 0) // т.е работает
            {
                resourceBlockAdd.gameObject.SetActive(true);
                _resourceAddText.text =
                    "+" + _selectedBuildController.workersCount * _selectedBuilding.resourceOneWorker +
                    " " + _selectedBuilding.typeResource.GetString();
            }
            else
                resourceBlockAdd.gameObject.SetActive(false);
        }
        else
        {
            textHealth.text = "Состояние: СТРОЙКА";
            textEnergy.text = "";
            resourceBlockAdd.gameObject.SetActive(false);
        }
    }

    public void UpdateResourceText()
    {
        textResourceName.text = "Ресурс: " + _selectedBuildController.Building.TypeResource.GetString();
        textResourceRemain.text = "Осталось: " + _selectedBuildController.health;
        textCountWorkersResource.text = _selectedBuildController.workersCount + "/" +
                                        _selectedBuildController.Building.MaxWorkers;
    }

    #endregion

    #region ManageBuild

    // Начинаем размещать объект. Метод для кнопки

    public void StartPlacingBuilding(BuildingController buildingControllerPrefab)
    {
        if (_flyingBuildingController)
            Destroy(_flyingBuildingController.gameObject);

        _flyingBuildingController = Instantiate(buildingControllerPrefab);
        _noteBlock.gameObject.SetActive(true);
        _textSelectedBuild.text = _flyingBuildingController.Building.BuildingName;
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
            {
                if (placeX + x >= 0 && placeX + x < Grid.GetLength(0) && placeY + y >= 0 &&
                    placeY + y < Grid.GetLength(1))
                {
                    if (Grid[placeX + x, placeY + y])
                        return true;
                }
            }
        }

        return false;
    }

    // Отрисовка в editor юнити сетки строения
    private void OnDrawGizmosSelected()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Gizmos.color = (x + y) % 2 == 0 ? new Color(0.88f, 0f, 1f, 0.3f) : new Color(1f, 0.68f, 0f, 0.3f);
                Gizmos.DrawCube(transform.position + new Vector3(x, 0, y), new Vector3(1, .1f, 1));
            }
        }
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
            _flyingBuildingController.name = _flyingBuildingController.name.Replace("(Clone)", "");
            PlaceBuilding(_flyingBuildingController, placeX, placeY);
            BuildingSaveSystem.Singleton.CreateBuildSave(placeX, placeY, _flyingBuildingController.name,
                false); // Создаем сохранение постройки

            switch (_flyingBuildingController.Building.BuildingName)
            {
                // Кор
                case "Солнечная панель":
                case "Фабрика материалов":
                    if (QuestSystem.Singleton.totalQuest.questName == "StartQuest")
                        QuestSystem.Singleton.MoveNextStep();
                    break;
            }

            BearTaskManager.Singleton.CreateNewTask(TasksMode.Build, _flyingBuildingController.gameObject,
                Traditions.Constructors,
                building.stepsNeed);
            _flyingBuildingController.SetBuilding();

            ColonyManager.Singleton.Materials -= building.materialsNeed;
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
            {
                if (placeX + x >= 0 && placeX + x < Grid.GetLength(0) && placeY + y >= 0 &&
                    placeY + y < Grid.GetLength(1))
                    Grid[placeX + x, placeY + y] = buildingController;
            }
        }
    }

    public void DeactivateBuilding()
    {
        _selectedBuildController.ChangeIsReady(!_selectedBuildController.isReady);
        textResourceRemain.text = _selectedBuildController.isReady ? "Остановить" : "Возобновить";
        UpdateBuildingText();
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
                    Grid[startX + x, startY + y] = null; // Очищаем ячейку
            }

            BuildingSaveSystem.Singleton.DestroyBuildingSave(
                (int)_selectedBuildController.transform.position.x,
                (int)_selectedBuildController.transform.position.z); // Удаляем buildingsave из списка сохраненных
            // Уничтожаем объект
            Destroy(_selectedBuildController.gameObject);

            ColonyManager.Singleton.Materials += _selectedBuilding.materialsNeed / 2;
            if (_selectedBuilding.energyNeed != 0)
            {
                ColonyManager.Singleton.Energy -= _selectedBuilding.energyNeed;

                APIClient.Instance.CreateLogRequest(
                    "Потрачена энергия на снос здания",
                    Player.Instance.playerName,
                    new Dictionary<string, string>() { { "energy", "-" + _selectedBuilding.energyNeed } });
            }

            _selectedBuildController = null;
            buildMenu.gameObject.SetActive(false);
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
                    ColonyManager.Singleton.MaxMaterials += building.resourceGive;
                    resource = "maxMaterials";
                    break;
                case Resources.MaterialPlus:
                    ColonyManager.Singleton.MaxMaterialsPlus += building.resourceGive;
                    resource = "maxMaterialsPlus";
                    break;
                case Resources.Food:
                    ColonyManager.Singleton.MaxFood += building.resourceGive;
                    resource = "maxFood";
                    break;
                case Resources.Honey:
                    ColonyManager.Singleton.MaxHoney += building.resourceGive;
                    resource = "maxHoney";
                    break;
                case Resources.BioFuel:
                    ColonyManager.Singleton.MaxBiofuel += building.resourceGive;
                    resource = "maxBiofuel";
                    break;
                case Resources.Bears:
                    ColonyManager.Singleton.MaxBears += building.resourceGive;
                    resource = "maxBears";
                    break;
                case Resources.Energy:
                    ColonyManager.Singleton.MaxEnergy += building.resourceGive;
                    ColonyManager.Singleton.Energy += building.resourceGive;
                    resource = "energy";
                    break;
            }

            APIClient.Instance.CreateLogRequest(
                "Повышение лимитов за здание",
                Player.Instance.playerName,
                new Dictionary<string, string> { { resource, "+" + building.resourceGive } });
        }
    }

    #endregion

    public void ChangeBuildingPage(GameObject page)
    {
        buildingsBearMenu.SetActive(false);
        buildingsWorkMenu.SetActive(false);
        buildingsSienceMenu.SetActive(false);

        page.SetActive(true);
        // Обновление списка зданий к постройке
        foreach (Transform child in page.transform)
        {
            // Единичная инициализация
            BuildingBuyInfo building = child.GetComponent<BuildingBuyInfo>();
            if (building.building) // Всякое бывает
            {
                // Возможность нажать на кнопку
                building.button.interactable =
                    building.building.materialsNeed <= ColonyManager.Singleton.Materials &&
                    building.building.energyNeed <= ColonyManager.Singleton.Energy;
                // Смена цены в зависимости от достатка
                building.textPriceMaterial.color =
                    building.building.materialsNeed <= ColonyManager.Singleton.Materials
                        ? Color.white
                        : Color.red;
                building.textPriceEnergy.color =
                    building.building.energyNeed <= ColonyManager.Singleton.Energy
                        ? Color.white
                        : Color.red;
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) // Активация меню какое здание построить
        {
            if (!GameMenuManager.Singleton.CheckOpenedWindows(!buildingCreateMenu
                    .activeSelf)) // Если какая-то менюха уже открыта
            {
                buildingCreateMenu.gameObject.SetActive(!buildingCreateMenu.activeSelf);
                _noteBlock.gameObject.SetActive(false);
                ChangeBuildingPage(buildingsBearMenu); // Исходная
                if (!buildingCreateMenu.activeSelf && _flyingBuildingController)
                    Destroy(_flyingBuildingController.gameObject);
            }
        }

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
                if (x < -gridSize.x / 2 || x + _flyingBuildingController.size.x > gridSize.x / 2)
                    available = false;
                if (y < -gridSize.y / 2 || y + _flyingBuildingController.size.y > gridSize.y / 2)
                    available = false;

                // Если здание расположено на другом - помечать расположение недействительным
                if (available && IsPlaceTaken(x, y)) available = false;

                _flyingBuildingController.transform.position =
                    new Vector3(x + _flyingBuildingController.size.x / 2f, 4,
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
}