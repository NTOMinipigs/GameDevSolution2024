using System;
using System.Collections.Generic;
using ArgumentException = System.ArgumentException;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

/// <summary>
/// Singleton паттерн
/// </summary>
public class ColonyManager : MonoBehaviour
{
    [Header("Main resources")]
    // Структура каждого ресурса: _ресурсText _ресурсPrivate Ресурс _максимумРесурсаПриват МаксимумРесурса
    // _ресурс/_максимумРесурсаПриват
    [Header("-Honey")]
    [SerializeField]
    private TextMeshProUGUI honeyText;

    private float _honey;

    public float Honey
    {
        get => _honey;
        set
        {
            if (value > MaxHoney && MaxHoney != 0)
                _honey = MaxHoney;
            else
                _honey = value;
            _ = APIClient.Instance.SetUserInventoryRequest(Player.Instance.playerName, SendDictionary);
            honeyText.text = _honey + "/" + _maxHoney;
        }
    }

    private float _maxHoney;

    public float MaxHoney
    {
        get => _maxHoney;
        set
        {
            _maxHoney = value;
            _ = APIClient.Instance.SetUserInventoryRequest(Player.Instance.playerName, SendDictionary);
            honeyText.text = _honey + "/" + _maxHoney;
        }
    }

    [Header("-Biofuel")] [SerializeField] private TextMeshProUGUI biofuelText;
    private float _biofuel;

    public float Biofuel
    {
        get => _biofuel;
        set
        {
            if (value > MaxBiofuel && MaxBiofuel != 0)
                _biofuel = MaxBiofuel;
            else
                _biofuel = value;
            _ = APIClient.Instance.SetUserInventoryRequest(Player.Instance.playerName, SendDictionary);
            biofuelText.text = _biofuel + "/" + _maxBiofuel;
        }
    }

    private float _maxBiofuel;

    public float MaxBiofuel
    {
        get => _maxBiofuel;
        set
        {
            _maxBiofuel = value;
            _ = APIClient.Instance.SetUserInventoryRequest(Player.Instance.playerName, SendDictionary);
            biofuelText.text = _biofuel + "/" + _maxBiofuel;
        }
    }

    [Header("-Energy")] [SerializeField] private TextMeshProUGUI energyText;
    private float _energy;

    public float Energy
    {
        get => _energy;
        set
        {
            if (value > MaxEnergy && MaxEnergy != 0)
                _energy = MaxEnergy;
            else
                _energy = value;
            _ = APIClient.Instance.SetUserInventoryRequest(Player.Instance.playerName, SendDictionary);
            energyText.text = _energy + "/" + _maxEnergy;
        }
    }

    private float _maxEnergy;

    public float MaxEnergy
    {
        get => _maxEnergy;
        set
        {
            _maxEnergy = value;
            _ = APIClient.Instance.SetUserInventoryRequest(Player.Instance.playerName, SendDictionary);
            energyText.text = _energy + "/" + _maxEnergy;
        }
    }

    [Header("-materials")] [SerializeField]
    private TextMeshProUGUI materialsText;

    private float _materials;

    public float Materials
    {
        get => _materials;
        set
        {
            if (value > MaxMaterials && MaxMaterials != 0)
                _materials = MaxMaterials;
            else
                _materials = value;
            _materials = value;
            _ = APIClient.Instance.SetUserInventoryRequest(Player.Instance.playerName, SendDictionary);
            materialsText.text = _materials + "/" + _maxMaterials;
        }
    }

    private float _maxMaterials;

    public float MaxMaterials
    {
        get => _maxMaterials;
        set
        {
            _maxMaterials = value;
            _ = APIClient.Instance.SetUserInventoryRequest(Player.Instance.playerName, SendDictionary);
            materialsText.text = _materials + "/" + _maxMaterials;
        }
    }

    [SerializeField] private TextMeshProUGUI materialsPlusText;
    private float _materialsPlus;

    public float MaterialsPlus
    {
        get => _materialsPlus;
        set
        {
            if (value > MaxMaterialsPlus && MaxMaterialsPlus != 0)
                _materialsPlus = MaxMaterialsPlus;
            else
                _materialsPlus = value;
            _ = APIClient.Instance.SetUserInventoryRequest(Player.Instance.playerName, SendDictionary);
            materialsPlusText.text = _materialsPlus + "/" + _maxMaterialsPlus;
        }
    }

    private float _maxMaterialsPlus;

    public float MaxMaterialsPlus
    {
        get => _maxMaterialsPlus;
        set
        {
            _maxMaterialsPlus = value;
            _ = APIClient.Instance.SetUserInventoryRequest(Player.Instance.playerName, SendDictionary);
            materialsPlusText.text = _materialsPlus + "/" + _maxMaterialsPlus;
        }
    }

    [Header("-food")] [SerializeField] private TextMeshProUGUI foodText;
    private float _food;

    public float Food
    {
        get => _food;
        set
        {
            if (value > MaxFood && MaxFood != 0)
                _food = MaxFood;
            else
                _food = value;
            _ = APIClient.Instance.SetUserInventoryRequest(Player.Instance.playerName, SendDictionary);
            foodText.text = _food + "/" + _maxFood;
        }
    }

    private float _maxFood;

    public float MaxFood
    {
        get => _maxFood;
        set
        {
            _maxFood = value;
            _ = APIClient.Instance.SetUserInventoryRequest(Player.Instance.playerName, SendDictionary);
            foodText.text = _food + "/" + _maxFood;
        }
    }

    [Header("Bears")] public List<Bear> bearsInColony;
    public Dictionary<string, Bear> _bearsInColonyDict = new Dictionary<string, Bear>();
    public int maxBears;
    public int workingBears; // Временный костыль
    public List<BearTask> bearTasks = new List<BearTask>();
    [SerializeField] private GameObject spawnBears; // Потом сделать spawnBears более рандомным
    [SerializeField] private string[] menBearsFirstnames, womanBearsFirstnames, bearsLastnames = Array.Empty<string>();

    [SerializeField] private SerializableBear[] spriteBeekeepers,
        spriteConstructors,
        spriteProgrammers,
        spriteBioengineers = Array.Empty<SerializableBear>();

    [SerializeField] private TextMeshProUGUI bearsCountText;
    public GameObject bearsListMenu;
    [SerializeField] private GameObject bearsListContainer;
    [SerializeField] private GameObject cardBearPrefab;
    [SerializeField] private adaptiveScroll bearsListAs;

    [Header("Other")] public bool scoutHome;
    [SerializeField] private allScripts scripts;
    private SystemSaver _systemSaver;

    private Dictionary<string, Func<float>> _materialsRefs;

    /// <summary>
    /// Словарь _materialRefs хранится в несереализуемом виде, так как в нем присутствуют лямбды
    /// Этот геттер позволяет удобно привести словарь к сереализуемому виду.
    /// </summary>
    /// TODO: Сейчас здесь работает приведение к типу int, т.к. все остальное хранится в флоат. Исправить
    private Dictionary<string, int> SendDictionary
    {
        get
        {
            Dictionary<string, int> sendDictionary = new Dictionary<string, int>();
            foreach (var (key, value) in _materialsRefs)
                sendDictionary[key] = (int)value();

            return sendDictionary;
        }
    }

    public async void Start()
    {
        _systemSaver = gameObject.GetComponent<SystemSaver>();

        // Здесь инициализируется словарь с значениями-ссылками на ресурсы
        _materialsRefs = new Dictionary<string, Func<float>>()
        {
            { "materials", () => Materials },
            { "food", () => Food },
            { "bioFuel", () => Biofuel },
            { "honey", () => Honey },
            { "bears", () => bearsInColony.Count },
            { "materialPlus", () => MaterialsPlus },
            { "energy", () => Energy },
            { "maxMaterials", () => MaxMaterials },
            { "maxFood", () => MaxFood },
            { "maxBioFuel", () => MaxBiofuel },
            { "maxHoney", () => MaxHoney },
            { "maxBears", () => maxBears },
            { "maxMaterialPlus", () => MaxMaterialsPlus },
            { "maxEnergy", () => MaxEnergy }
        };

        APIClient.UserInventory inventory =
            await APIClient.Instance.GetUserInventoryRequest(Player.Instance.playerName);
        if (inventory == null) return;
        _materials = inventory.Resources["materials"];
        _food = inventory.Resources["food"];
        _biofuel = inventory.Resources["bioFuel"];
        _honey = inventory.Resources["honey"];
        _materialsPlus = inventory.Resources["materialPlus"];
        _energy = inventory.Resources["energy"];
        _maxMaterials = inventory.Resources["maxMaterials"];
        _maxFood = inventory.Resources["maxFood"];
        _maxBiofuel = inventory.Resources["maxBioFuel"];
        _maxHoney = inventory.Resources["maxHoney"];
        maxBears = inventory.Resources["maxBears"];
        _maxMaterialsPlus = inventory.Resources["maxMaterialPlus"];
        _maxEnergy = inventory.Resources["maxEnergy"];
    }
    
    /// <summary>
    /// Получаем медведя по его dev имени
    /// </summary>
    /// <param name="gameName">Dev имя</param>
    /// <returns>Возвращает медведя по dev имени</returns>
    public Bear GetBear(string gameName)
    {
        return _bearsInColonyDict[gameName];
    }

    /// <summary>
    /// Вернет случайного медведя в обмен на традицию (вынесено из метода GenerateNewBear)
    /// </summary>
    /// <param name="tradition">Традиция</param>
    /// <returns>Объект SerailizeBear (см документацию этого класса)</returns>
    /// <exception cref="ArgumentException">Если активность не найдена.</exception>
    private SerializableBear GetSerializableBear(Traditions tradition)
    {
        return tradition switch // Упростил выражение
        {
            Traditions.Beekeepers => spriteBeekeepers[Random.Range(0, spriteBeekeepers.Length - 1)],
            Traditions.Constructors => spriteConstructors[
                Random.Range(0, spriteConstructors.Length - 1)],
            Traditions.Programmers => spriteProgrammers
                [Random.Range(0, spriteProgrammers.Length - 1)],
            Traditions.BioEngineers => spriteBioengineers[
                Random.Range(0, spriteBioengineers.Length - 1)],
            _ => throw new ArgumentException("Tradition " + tradition + " not found!")
        };
    }

    /// <summary>
    /// Сохраняет медведя + генерирует позицию(вынесено из метода GenerateNewBear)
    /// </summary>
    /// <param name="bear"></param>
    /// <param name="serializableBearName"></param>
    private void SaveBear(Bear bear, string serializableBearName)
    {
        Vector3 generatePosition = spawnBears.transform.position +
                                   new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));

        // Записываем медведя в SystemSaver, чтобы в будущем удобно записывать в json
        bear.serializableBear = serializableBearName;
        bear.x = generatePosition.x;
        bear.z = generatePosition.z;
        bear.y = generatePosition.y;

        _systemSaver.gameSave.bears.Add(bear);
    }

    /// <summary>
    /// Генерирует нового медведя по традиции
    /// </summary>
    /// <param name="tradition"></param>
    /// <exception cref="ArgumentException"></exception>
    public void GenerateNewBear(Traditions tradition)
    {
        SerializableBear serializableBear = GetSerializableBear(tradition);
        string bearName = GetBearName(serializableBear.gender);

        // TODO: сделать норм индексацию
        Bear newBear = new Bear(tradition.ToString() + Random.Range(0, 1000), bearName, tradition,
            serializableBear.sprite);
        SaveBear(newBear, serializableBear.name);
    }

    /// <summary>
    /// Генерирует ЗАДАННОГО медведя
    /// </summary>
    /// <param name="bearName"></param>
    /// <param name="serializableBearObj"></param>
    /// <param name="tradition"></param>
    public void GenerateSpecialBear(string bearName, GameObject serializableBearObj, Traditions tradition)
    {
        SerializableBear serializableBear = serializableBearObj.GetComponent<SerializableBear>();
        Bear newBear = new Bear(
            tradition.ToString() + Random.Range(0, 1000),
            bearName,
            tradition,
            serializableBear.sprite
        );
        SaveBear(newBear, serializableBear.name);
    }

    /// <summary>
    /// Осуществляет создание медведей на поле
    /// </summary>
    /// <param name="newBear">сейв медведя из json'а</param>
    public void BearSpawn(Bear newBear)
    {
        SerializableBear serializableBear = GameObject.Find(newBear.serializableBear).GetComponent<SerializableBear>();
        newBear.sprite = serializableBear.sprite; // Добавлено, т.к как-то не сохраняется и не назначается, хз
        bearsInColony.Add(newBear);
        _bearsInColonyDict.Add(newBear.gameName, newBear);
        if ((Traditions)Enum.Parse(typeof(Traditions), newBear.tradition.ToString()) != Traditions.Chrom)
        {
            GameObject bearObj = Instantiate(serializableBear.prefab, new Vector3(newBear.x, newBear.y, newBear.z),
                Quaternion.identity);
            bearObj.name = newBear.gameName;
            bearObj.GetComponent<BearMovement>().totalBear = newBear;
        }
    }

    /// <summary>
    /// Сгенерировать имя основываясь на гендере
    /// </summary>
    /// <returns>ФИ медведя</returns>
    private string GetBearName(SerializableBear.Gender gender)
    {
        string firstName = gender == SerializableBear.Gender.Men
            ? menBearsFirstnames[Random.Range(0, menBearsFirstnames.Length - 1)]
            : womanBearsFirstnames[Random.Range(0, womanBearsFirstnames.Length - 1)];
        string lastName = bearsLastnames[Random.Range(0, bearsLastnames.Length - 1)];

        return firstName + " " + lastName;
    }

    /// <summary>
    /// Получить свободного медведя
    /// </summary>
    private Bear GetChillBear()
    {
        foreach (Bear bear in bearsInColony)
        {
            if ((bear.activity == Activities.Chill || GetBearTask(bear) == null) && bear.tradition != Traditions.Chrom)
                return bear;
        }

        return null;
    }

    /// <summary>
    /// Сгенерировать имя основываясь на гендере
    /// </summary>
    public void CreateNewTask(TasksMode newTaskMode, GameObject objectOfTask, float steps)
    {
        // TODO: сделать возможнсть работы по кастам
        BearTask task = new BearTask(newTaskMode, objectOfTask, steps);
        Bear chillBear = GetChillBear();
        if (chillBear != null)
        {
            task.selectedBear = chillBear;
            chillBear.activity = Activities.Work;
        }

        bearTasks.Add(task);
    }

    /// <summary>
    /// Выдать освободившемуся медведю новую задачу
    /// </summary>
    private void SetTaskToBear(Bear bear)
    {
        foreach (BearTask task in bearTasks)
        {
            if (task.selectedBear == null)
            {
                task.selectedBear = bear;
                bear.activity = Activities.Work;
                break;
            }
        }

        // Если работы не нашлось
        if (GetBearTask(bear) == null)
            bear.activity = Activities.Chill;
    }

    public BearTask GetBearTask(Bear bear)
    {
        foreach (BearTask task in bearTasks)
        {
            if (task.selectedBear == bear)
                return task;
        }

        return null;
    }

    public void EndTask(BearTask task)
    {
        if (task.taskMode == TasksMode.Build)
        {
            Building building = task.objectOfTask.GetComponent<Building>();
            building.SetNormal();
            building.builded = true;
            scripts.buildingSystem.SetBuildSettings(building);
            if (building.scoutHome)
                scoutHome = true;
        }
        else if (task.taskMode == TasksMode.GetResource)
            scripts.buildingSystem.PickUpResource(task.objectOfTask);

        Bear selectedBear = task.selectedBear;
        bearTasks.Remove(task);

        if (selectedBear.tired >= 5 || selectedBear.hungry >= 5)
            selectedBear.activity = Activities.Chill;
        else
            SetTaskToBear(selectedBear);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (scripts.CheckOpenedWindows(!bearsListMenu.activeSelf)) // Если какая-то менюха уже открыта
                return;

            bearsListMenu.gameObject.SetActive(!bearsListMenu.activeSelf);
            scripts.cameraMove.blockMove = bearsListMenu.activeSelf;
            if (!bearsListMenu.activeSelf) return;

            foreach (Transform child in bearsListContainer.transform)
                Destroy(child.gameObject);

            foreach (Bear bear in bearsInColony)
            {
                GameObject newObj = Instantiate(cardBearPrefab, Vector3.zero, Quaternion.identity,
                    bearsListContainer.transform);
                newObj.name = bear.gameName;

                Image image = newObj.transform.Find("Icon").GetComponent<Image>();
                image.sprite = bear.sprite;
                image.SetNativeSize();
            }

            bearsListAs.UpdateContentSize();
        }
    }

    private void FixedUpdate()
    {
        bearsCountText.text = (bearsInColony.Count - scripts.colonyManager.workingBears) + "/" + maxBears; // костыль
        if (bearsListMenu.activeSelf)
        {
            foreach (Transform child in bearsListContainer.transform)
            {
                Bear bear = GetBear(child.name);
                child.transform.Find("TextInfo").GetComponent<TextMeshProUGUI>().text = "Имя: " + bear.bearName +
                    "\nТрадиция: " + bear.TraditionStr + "\nТекущее дело: " + bear.ActivityStr + "\nУсталость/голод: " +
                    (Mathf.Round(bear.tired * 10) / 10) + "/" + (Mathf.Round(bear.hungry * 10) / 10);
            }
        }
    }
}