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
    [SerializeField] private TextMeshProUGUI honeyText;
    private float _honey;

    public float Honey
    {
        get { return _honey; }
        set
        {
            if (value > MaxHoney && MaxHoney != 0)
                _honey = MaxHoney;
            else
                _honey = value;
            APIClient.Instance.SetUserInventoryRequest(Player.Instance.playerName, SendDictionary);
            honeyText.text = _honey.ToString() + "/" + _maxHoney.ToString();
        }
    }
    private float _maxHoney;
    public float MaxHoney
    {
        get { return _maxHoney; }
        set
        {
            _maxHoney = value;
            APIClient.Instance.SetUserInventoryRequest(Player.Instance.playerName, SendDictionary);
            honeyText.text = _honey.ToString() + "/" + _maxHoney.ToString();
        }
    }

    [Header("-Biofuel")][SerializeField] private TextMeshProUGUI biofuelText;
    private float _biofuel;

    public float Biofuel
    {
        get { return _biofuel; }
        set
        {
            if (value > MaxBiofuel && MaxBiofuel != 0)
                _biofuel = MaxBiofuel;
            else
                _biofuel = value;
            APIClient.Instance.SetUserInventoryRequest(Player.Instance.playerName, SendDictionary);
            biofuelText.text = _biofuel.ToString() + "/" + _maxBiofuel.ToString();
        }
    }
    private float _maxBiofuel;
    public float MaxBiofuel
    {
        get { return _maxBiofuel; }
        set
        {
            _maxBiofuel = value;
            APIClient.Instance.SetUserInventoryRequest(Player.Instance.playerName, SendDictionary);
            biofuelText.text = _biofuel.ToString() + "/" + _maxBiofuel.ToString();
        }
    }

    [Header("-Energy")]
    [SerializeField] private TextMeshProUGUI energyText;
    private float _energy;
    public float Energy
    {
        get { return _energy; }
        set
        {
            if (value > MaxEnergy && MaxEnergy != 0)
                _energy = MaxEnergy;
            else
                _energy = value;
            APIClient.Instance.SetUserInventoryRequest(Player.Instance.playerName, SendDictionary);
            energyText.text = _energy.ToString() + "/" + _maxEnergy.ToString();
        }
    }
    private float _maxEnergy;
    public float MaxEnergy
    {
        get { return _maxEnergy; }
        set
        {
            _maxEnergy = value;
            APIClient.Instance.SetUserInventoryRequest(Player.Instance.playerName, SendDictionary);
            energyText.text = _energy.ToString() + "/" + _maxEnergy.ToString();
        }
    }

    [Header("-materials")]
    [SerializeField]
    private TextMeshProUGUI materialsText;

    private float _materials;

    public float Materials
    {
        get { return _materials; }
        set
        {
            if (value > MaxMaterials && MaxMaterials != 0)
                _materials = MaxMaterials;
            else
                _materials = value;
            _materials = value;
            APIClient.Instance.SetUserInventoryRequest(Player.Instance.playerName, SendDictionary);
            materialsText.text = _materials.ToString() + "/" + _maxMaterials.ToString();
        }
    }

    private float _maxMaterials;
    public float MaxMaterials
    {
        get { return _maxMaterials; }
        set
        {
            _maxMaterials = value;
            APIClient.Instance.SetUserInventoryRequest(Player.Instance.playerName, SendDictionary);
            materialsText.text = _materials.ToString() + "/" + _maxMaterials.ToString();
        }
    }

    [SerializeField] private TextMeshProUGUI materialsPlusText;
    private float _materialsPlus;

    public float materialsPlus
    {
        get { return _materialsPlus; }
        set
        {
            if (value > MaxMaterialsPlus && MaxMaterialsPlus != 0)
                _materialsPlus = MaxMaterialsPlus;
            else
                _materialsPlus = value;
            APIClient.Instance.SetUserInventoryRequest(Player.Instance.playerName, SendDictionary);
            materialsPlusText.text = _materialsPlus.ToString();
        }
    }

    private float _maxMaterialsPlus;
    public float MaxMaterialsPlus
    {
        get { return _maxMaterialsPlus; }
        set
        {
            _maxMaterialsPlus = value;
            APIClient.Instance.SetUserInventoryRequest(Player.Instance.playerName, SendDictionary);
            materialsText.text = _materialsPlus.ToString() + "/" + _maxMaterialsPlus.ToString();
        }
    }

    [Header("-food")]
    [SerializeField] private TextMeshProUGUI foodText;
    private float _food;

    public float Food
    {
        get { return _food; }
        set
        {
            if (value > MaxFood && MaxFood != 0)
                _food = MaxFood;
            else
                _food = value;
            APIClient.Instance.SetUserInventoryRequest(Player.Instance.playerName, SendDictionary);
            foodText.text = _food.ToString() + "/" + _maxFood.ToString();
        }
    }
    private float _maxFood;
    public float MaxFood
    {
        get { return _maxFood; }
        set
        {
            _maxFood = value;
            APIClient.Instance.SetUserInventoryRequest(Player.Instance.playerName, SendDictionary);
            foodText.text = _food.ToString() + "/" + _maxFood.ToString();
        }
    }

    [Header("Bears")]
    public List<Bear> bearsInColony = new List<Bear>();
    public int maxBears;
    public int workingBears; // Временный костыль
    public List<BearTask> bearTasks = new List<BearTask>();
    [SerializeField] private GameObject spawnBears; // Потом сделать spawnBears более рандомным
    [SerializeField] private string[] menBearsFirstnames, womanBearsFirstnames, bearsLastnames = new string[0];

    [SerializeField]
    private SerializableBear[] spriteBeekeepers,
        spriteConstructors,
        spriteProgrammers,
        spriteBioengineers = new SerializableBear[0];

    [SerializeField] private TextMeshProUGUI bearsCountText;
    public GameObject bearsListMenu;
    [SerializeField] private GameObject bearsListContainer;
    [SerializeField] private GameObject cardBearPrefab;
    [SerializeField] private adaptiveScroll bearsListAS;

    [Header("Other")]
    public bool scoutHome;
    [SerializeField] private allScripts scripts;

    private Dictionary<string, Func<float>> _materialsRefs;

    /// <summary>
    /// Словарь _materialRefs хранится в несереализуемом виде, так как в нем присутствуют лямбды
    /// Этот геттер позволяет удобно привести словарь к сереализуемому виду.
    /// </summary>
    /// TODO: Сейчас здесь работает приведение к типу int, т.к. все остальное хранится в флоат. Исправить
    public Dictionary<string, int> SendDictionary
    {
        get
        {
            Dictionary<string, int> sendDictionary = new Dictionary<string, int>();
            foreach (var (key, value) in _materialsRefs)
            {
                sendDictionary[key] = (int)value();
            }
            return sendDictionary;
        }
    }

    public async void Start()
    {
        // Здесь инициализируется словарь с значениями-ссылками на ресурсы
        _materialsRefs = new Dictionary<string, Func<float>>()
        {
            {"materials", () => Materials},
            {"food", () => Food},
            {"bioFuel", () => Biofuel },
            {"honey", () => Honey },
            {"bears", () => bearsInColony.Count },
            {"materialPlus", () => materialsPlus},
            {"energy", () => Energy},
            {"maxMaterials", () => MaxMaterials},
            {"maxFood", () => MaxFood},
            {"maxBioFuel", () => MaxBiofuel},
            {"maxHoney", () => MaxHoney},
            {"maxBears", () => maxBears},
            {"maxMaterialPlus", () => MaxMaterialsPlus},
            {"maxEnergy", () => MaxEnergy}
        };
       
        APIClient.UserInventory inventory = await APIClient.Instance.GetUserInventoryRequest(Player.Instance.playerName);
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
    /// Получаем медведя по названию игры? 
    /// </summary>
    /// <param name="gameName">Название игры? (спасибо за чистый код)</param>
    /// <returns>Возвращает медведя по названию игры</returns>
    public Bear GetBear(string gameName)
    {
        foreach (Bear bear in bearsInColony)
        {
            if (bear.gameName == gameName)
                return bear;
        }

        Debug.Log(gameName + " dont finded");
        return null;
    }

    /// <summary>
    /// Вернет случайного медведя в обмен на традицию (вынесено из метода GenerateNewBear)
    /// </summary>
    /// <param name="tradition">Традиция</param>
    /// <returns>Объект SerailizeBear (см документацию этого класса)</returns>
    /// <exception cref="ArgumentException">Если активность не найдена. АРТЕМ НЕ НАДО ВЫРЕЗАТЬ ARGUMENTEXCEIPTIONS! Если ты обосрешься, то благодаря ошибке ты увидишь это быстрее</exception>
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
    /// Генерирует нового медведя
    /// </summary>
    /// <param name="tradition"></param>
    /// <exception cref="ArgumentException"></exception>
    public void GenerateNewBear(Traditions tradition)
    {
        SerializableBear serializableBear = GetSerializableBear(tradition);
        string bearName = GetBearName(serializableBear.gender);

        // Блядь мне это трогать страшно, я просто позицию в отдельную переменную вынес и все нахуй посыпалось
        // TODO: сделать норм индексацию
        Bear newBear = new Bear(tradition.ToString() + Random.Range(0, 1000), bearName, tradition,
            serializableBear.sprite);
        Vector3 generatePosition = spawnBears.transform.position +
                                   new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));

        // Записываем медведя в SystemSaver, чтобы в будущем удобно записывать в json
        newBear.serializableBear = serializableBear.name;
        newBear.x = generatePosition.x;
        newBear.z = generatePosition.z;
        newBear.y = generatePosition.y;
        
        SystemSaver systemSaver = gameObject.GetComponent<SystemSaver>();
        systemSaver.gameSave.bears.Add(newBear);
    }

    /// <summary>
    /// Генератор для Хрома (Отдельно т.к. задается отдельно)
    /// </summary>
    public void GenerateChrom()
    {
        string bearName = "Хром";
        SerializableBear serializableBear = GameObject.Find("BearChrom_0").GetComponent<SerializableBear>();
        Bear newBear = new Bear(
            Traditions.Chrom.ToString() + Random.Range(0, 1000),
            bearName,
            Traditions.Chrom,
             serializableBear.sprite
            );
        newBear.activity = Activities.Chill;
        Vector3 generatePosition = spawnBears.transform.position +
                                   new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));

        // Записываем медведя в SystemSaver, чтобы в будущем удобно записывать в json
        newBear.serializableBear = serializableBear.name;
        newBear.x = generatePosition.x;
        newBear.z = generatePosition.z;
        newBear.y = generatePosition.y;
        
        SystemSaver systemSaver = gameObject.GetComponent<SystemSaver>();
        systemSaver.gameSave.bears.Add(newBear);
    }

    /// <summary>
    /// Осуществляет создание медведей на поле
    /// </summary>
    /// <param name="newBear">сейв медведя из json'а</param>
    public void BearSpawn(Bear newBear)
    {
        SerializableBear serializableBear = GameObject.Find(newBear.serializableBear).GetComponent<SerializableBear>();
        bearsInColony.Add(newBear);
        if ((Traditions)Enum.Parse(typeof(Traditions), newBear.tradition.ToString()) != Traditions.Chrom)
        {
            GameObject bearObj = Instantiate(serializableBear.prefab, new Vector3(newBear.x, newBear.y, newBear.z), Quaternion.identity);
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
        string firstName = gender == SerializableBear.Gender.Men ? menBearsFirstnames[Random.Range(0, menBearsFirstnames.Length - 1)] : womanBearsFirstnames[Random.Range(0, womanBearsFirstnames.Length - 1)];
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
    public void SetTaskToBear(Bear bear)
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
            task.objectOfTask.GetComponent<Building>().SetNormal();
            task.objectOfTask.GetComponent<Building>().builded = true;
            scripts.buildingSystem.SetBuildSettings(task.objectOfTask);
            if (task.objectOfTask.GetComponent<Building>().scoutHome)
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
            if (bearsListMenu.activeSelf)
            {
                foreach (Transform child in bearsListContainer.transform)
                    Destroy(child.gameObject);

                foreach (Bear bear in bearsInColony)
                {
                    GameObject newObj = Instantiate(cardBearPrefab, Vector3.zero, Quaternion.identity, bearsListContainer.transform);
                    newObj.name = bear.gameName;

                    Image image = newObj.transform.Find("Icon").GetComponent<Image>();
                    image.sprite = bear.sprite;
                    image.SetNativeSize();
                }
                bearsListAS.UpdateContentSize();
            }
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
                child.transform.Find("TextInfo").GetComponent<TextMeshProUGUI>().text = "Имя: " + bear.bearName + "\nТрадиция: " + bear.TraditionStr + "\nТекущее дело: " + bear.ActivityStr + "\nУсталость/голод: " + (Mathf.Round(bear.tired * 10) / 10) + "/" + (Mathf.Round(bear.hungry * 10) / 10);
            }
        }
    }
}

