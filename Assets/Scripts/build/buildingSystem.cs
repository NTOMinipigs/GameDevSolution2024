using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingSystem : MonoBehaviour
{
    public GameObject buildingCreateMenu, buildMenu;
    [SerializeField] private GameObject buildMenuStandartButtons, buildMenuMaterialsButtons, noteBlock;
    [SerializeField] private TextMeshProUGUI textSelectedBuild;
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
        selectedBuild = building;
        bool isResource = building.typeOfBuilding == Building.TypesOfBuilding.resource;
        if (isResource)
            selectedResource = building.typeResource;
        ManageBuildMenu(true, isResource);
    }

    public void DisableBuildMenu() => ManageBuildMenu(false); // Для UI кнопки

    public void ManageBuildMenu(bool open = true, bool materialsMode = false)
    {
        buildMenu.gameObject.SetActive(open);
        if (open)
        {
            buildMenu.transform.Find("bg").transform.Find("TextName").GetComponent<TextMeshProUGUI>().text = selectedBuild.buildingName;
            buildMenuStandartButtons.gameObject.SetActive(!materialsMode);
            if (!materialsMode)
                buildMenuStandartButtons.transform.Find("ButtonDestroy").transform.Find("TextResourceReturn").GetComponent<TextMeshProUGUI>().text = (selectedBuild.materialsNeed / 2).ToString();

            buildMenuMaterialsButtons.gameObject.SetActive(materialsMode);
        }
    }

    public void PickUpResource()
    {
        switch (selectedResource)
        {
            case ColonyManager.typeOfResource.materials:
                scripts.colonyManager.Materials += Random.Range(10, 30);
                break;
            case ColonyManager.typeOfResource.materialPlus:
                scripts.colonyManager.materialsPlus += Random.Range(1, 5);
                break;
            case ColonyManager.typeOfResource.food:
                scripts.colonyManager.Food += Random.Range(1, 10);
                break;
            case ColonyManager.typeOfResource.honey:
                scripts.colonyManager.Honey += Random.Range(5, 10);
                break;
            case ColonyManager.typeOfResource.bioFuel:
                scripts.colonyManager.Biofuel += Random.Range(5, 15);
                break;
        }
        DestroyBuilding();
    }

    public void StartPlacingBuilding(Building buildingPrefab) // Начинаем размещать объект. Метод для кнопки
    {
        if (flyingBuilding != null)
            Destroy(flyingBuilding.gameObject);

        flyingBuilding = Instantiate(buildingPrefab);
        noteBlock.gameObject.SetActive(true);
        textSelectedBuild.text = flyingBuilding.buildingName;
    }


    public void DestroyBuilding()
    {
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
                    child.gameObject.GetComponent<Button>().interactable = child.gameObject.GetComponent<Building>().materialsNeed <= scripts.colonyManager.Materials;
                    if (child.gameObject.GetComponent<Building>().materialsNeed <= scripts.colonyManager.Materials)
                        child.transform.Find("TextPrice").GetComponent<TextMeshProUGUI>().color = Color.black;
                    else
                        child.transform.Find("TextPrice").GetComponent<TextMeshProUGUI>().color = Color.red;
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
        for (int x = 0; x < flyingBuilding.Size.x; x++)
        {
            for (int y = 0; y < flyingBuilding.Size.y; y++)
                grid[placeX + x, placeY + y] = flyingBuilding;
        }

        scripts.colonyManager.CreateNewTask(BearTask.TasksMode.build, flyingBuilding.gameObject, flyingBuilding.stepsNeed);
        flyingBuilding.SetBuilding();
        flyingBuilding = null;
        noteBlock.gameObject.SetActive(false);
        buildingCreateMenu.gameObject.SetActive(false);
    }
}