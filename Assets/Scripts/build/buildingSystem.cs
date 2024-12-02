using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingSystem : MonoBehaviour
{
    [SerializeField] private GameObject buildingCreateMenu, buildMenu, buildMenuStandartButtons, buildMenuMaterialsButtons, noteBlock;
    [SerializeField] private TextMeshProUGUI textSelectedBuild;
    [SerializeField] private Vector2Int GridSize = new Vector2Int(10, 10); // Сетка строительсва. P.s значение в юньке не 10 10
    public Building selectedBuild; // Выбранное строение для взаимодействий(НЕ ДЛЯ СТРОЕНИЯ)
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
        selectedBuild = building;
        ManageBuildMenu(true, building.typeOfBuilding == Building.TypesOfBuilding.materials);
    }

    public void DisableBuildMenu() => ManageBuildMenu(false); // Для UI кнопки

    public void ManageBuildMenu(bool open = true, bool materialsMode = false)
    {
        buildMenu.gameObject.SetActive(open);
        if (open)
        {
            buildMenu.transform.Find("bg").transform.Find("TextName").GetComponent<TextMeshProUGUI>().text = selectedBuild.buildingName;
            buildMenuStandartButtons.gameObject.SetActive(!materialsMode);
            buildMenuMaterialsButtons.gameObject.SetActive(materialsMode);
        }
    }

    public void StartPlacingBuilding(Building buildingPrefab) // Начинаем размещать объект. Метод для кнопки
    {
        if (flyingBuilding != null)
            Destroy(flyingBuilding.gameObject);

        flyingBuilding = Instantiate(buildingPrefab);
        noteBlock.gameObject.SetActive(true);
        textSelectedBuild.text = flyingBuilding.buildingName;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) // Активация меню какое здание построить
        {
            buildingCreateMenu.gameObject.SetActive(!buildingCreateMenu.activeSelf);
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

        flyingBuilding.SetNormal();
        flyingBuilding = null;
        noteBlock.gameObject.SetActive(false);
    }
}