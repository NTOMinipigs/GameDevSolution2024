using System;
using System.Collections.Generic;
using System.Linq;
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
    public static ColonyManager Singleton { get; private set; }
    public BearTaskManager BearTaskManager = new BearTaskManager();
    
    [Header("Main resources")]

    # region Resources

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
            _honey = value >= MaxHoney ? MaxHoney : value;
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
            _biofuel = value >= MaxBiofuel ? MaxBiofuel : value;
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
            _energy = value >= MaxEnergy ? MaxEnergy : value;
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
            _materials = value >= MaxMaterials ? MaxMaterials : value;
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
            _materialsPlus = value >= MaxMaterialsPlus ? MaxMaterialsPlus : value;
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
            _food = value >= MaxFood ? MaxFood : value;
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

    # endregion

    [Header("Bears")]

    #region Bears

    public List<Bear> bearsInColony;

    private readonly Dictionary<string, Bear> _bearsInColonyDict = new Dictionary<string, Bear>();
    private readonly Dictionary<string, Traditions> _bearsTraditionsInColonyDict = new Dictionary<string, Traditions>();
    public int maxBears;
    public int workingBears; // Временный костыль
    public List<BearTask> bearTasks = new List<BearTask>();
    public GameObject spawnBears; // Потом сделать spawnBears более рандомным
    [SerializeField] private string[] menBearsFirstnames, womanBearsFirstnames, bearsLastnames = Array.Empty<string>();

    [SerializeField] private SerializableBear[] spriteBeekeepers,
        spriteConstructors,
        spriteProgrammers,
        spriteBioengineers,
        spriteDrones = Array.Empty<SerializableBear>();

    [SerializeField] private TextMeshProUGUI bearsCountText;

    [SerializeField] private TextMeshProUGUI dronesText,
        beekeepersText,
        constructorsText,
        programmersText,
        bioengineersText;

    public GameObject bearsListMenu;
    [SerializeField] private GameObject bearsListContainer;
    [SerializeField] private GameObject cardBearPrefab;
    [SerializeField] private AdaptiveScroll bearsListAs;

    #endregion

    [Header("Other")] public bool scoutHome;
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

    private void Awake()
    {
        Singleton = this;
        BearTaskManager.Initialize();
    }

    private async void Start()
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

    public int GetCountFreeBearsOfTradition(Traditions tradition)
    {
        int freeWorkersOfTradition = 0;
        for (int i = 0; i < bearsInColony.Count; i++)
        {
            Bear newBear = bearsInColony[i];
            if (newBear.tradition == tradition && newBear.activity == Activities.Chill)
                freeWorkersOfTradition++;
        }

        return freeWorkersOfTradition;
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
            Traditions.Drone => spriteDrones[
                Random.Range(0, spriteDrones.Length - 1)],
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
    public Bear GenerateNewBear(Traditions tradition)
    {
        SerializableBear serializableBear = GetSerializableBear(tradition);
        string bearName = GetBearName(serializableBear.gender);

        // TODO: сделать норм индексацию
        Bear newBear = new Bear(tradition.ToString() + Random.Range(0, 1000), bearName, tradition,
            serializableBear.sprite);
        SaveBear(newBear, serializableBear.name);
        return newBear;
    }

    public Bear GenerateNewBearWithRandomTradition()
    {
        Bear newBear = new Bear();
        // Рандомный выбор по ВЫБРАННЫМ традициям
        int traditionRandom = Random.Range(1, 5);
        newBear = GenerateNewBear((Traditions)traditionRandom);

        BearSpawn(newBear);
        return newBear;
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

    // ReSharper disable Unity.PerformanceAnalysis
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
        _bearsTraditionsInColonyDict.Add(newBear.gameName, newBear.tradition);
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
    public Bear GetChillBear(Traditions bearTradition)
    {
        foreach (Bear bear in bearsInColony)
        {
            if ((bear.activity == Activities.Chill || BearTaskManager.Singleton.GetBearTask(bear) == null) &&
                (bear.tradition == bearTradition || bearTradition == Traditions.None))
                return bear;
        }

        return null;
    }

    /// <summary>
    ///  Подсчитывает число работников каждой традиции
    /// </summary>
    public void UpdateWorkersCount()
    {
        // TODO: сделать обновление на определенную традицию, А НЕ НА ВСЁ НАХУЙ
        // Максимальное кол-во работников на традицию
        var maxCounts = new Dictionary<Traditions, int>
        {
            {
                Traditions.Beekeepers,
                _bearsTraditionsInColonyDict.Count(kvp => kvp.Value.Equals(Traditions.Beekeepers))
            },
            {
                Traditions.Constructors,
                _bearsTraditionsInColonyDict.Count(kvp => kvp.Value.Equals(Traditions.Constructors))
            },
            {
                Traditions.Programmers,
                _bearsTraditionsInColonyDict.Count(kvp => kvp.Value.Equals(Traditions.Programmers))
            },
            {
                Traditions.BioEngineers,
                _bearsTraditionsInColonyDict.Count(kvp => kvp.Value.Equals(Traditions.BioEngineers))
            },
            {
                Traditions.Drone,
                _bearsTraditionsInColonyDict.Count(kvp => kvp.Value.Equals(Traditions.Drone))
            }
        };

        // Словарь для хранения текущего количества работников каждой традиции
        var currentCounts = new Dictionary<Traditions, int>();

        foreach (Bear bear in bearsInColony)
        {
            if (bear.activity != Activities.Chill) continue;
            if (currentCounts.ContainsKey(bear.tradition))
                currentCounts[bear.tradition]++;
            else
                currentCounts[bear.tradition] = 1; // Если традиция не была добавлена, инициализируем ее
        }

        // Обновление текстов
        beekeepersText.text =
            $"{currentCounts.GetValueOrDefault(Traditions.Beekeepers, 0)}/{maxCounts[Traditions.Beekeepers]}";
        constructorsText.text =
            $"{currentCounts.GetValueOrDefault(Traditions.Constructors, 0)}/{maxCounts[Traditions.Constructors]}";
        programmersText.text =
            $"{currentCounts.GetValueOrDefault(Traditions.Programmers, 0)}/{maxCounts[Traditions.Programmers]}";
        bioengineersText.text =
            $"{currentCounts.GetValueOrDefault(Traditions.BioEngineers, 0)}/{maxCounts[Traditions.BioEngineers]}";
        dronesText.text = $"{currentCounts.GetValueOrDefault(Traditions.Drone, 0)}/{maxCounts[Traditions.Drone]}";
    }

    /// <summary>
    /// Выдать награды
    /// </summary>
    /// <param name="eventRewards">Список наград</param>
    /// <returns></returns>
    public string GiveRewards(Reward[] eventRewards)
    {
        string textReward = "";
        foreach (Reward reward in eventRewards)
        {
            switch (reward.typeOfReward)
            {
                case Resources.Material:
                    Materials += reward.count;
                    textReward += "+" + Resources.Material + " x" + reward.count + "\n";
                    break;
                case Resources.MaterialPlus:
                    MaterialsPlus += reward.count;
                    textReward += "+" + Resources.MaterialPlus + " x" + reward.count + "\n";
                    break;
                case Resources.Food:
                    Food += reward.count;
                    textReward += "+" + Resources.Food + " x" + reward.count + "\n";
                    break;
                case Resources.Honey:
                    Honey += reward.count;
                    textReward += "+" + Resources.Honey + " x" + reward.count + "\n";
                    break;
                case Resources.BioFuel:
                    Biofuel += reward.count;
                    textReward += "+" + Resources.BioFuel + " x" + reward.count + "\n";
                    break;
                case Resources.Bears:
                    for (int i = 0; i < reward.count; i++)
                    {
                        Bear newBear = GenerateNewBearWithRandomTradition();
                        textReward += "+" + newBear.tradition.GetString() + " " + newBear.bearName + "\n";
                    }
                    break;
            }
        }

        return textReward;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (GameMenuManager.Singleton.CheckOpenedWindows(!bearsListMenu
                    .activeSelf)) // Если какая-то менюха уже открыта
                return;

            bearsListMenu.gameObject.SetActive(!bearsListMenu.activeSelf);
            CameraMove.Singleton.blockMove = bearsListMenu.activeSelf;
            if (!bearsListMenu.activeSelf) return;

            foreach (Transform child in bearsListContainer.transform)
                Destroy(child.gameObject);

            foreach (Bear bear in bearsInColony)
            {
                if (bear.tradition != Traditions.Drone)
                {
                    GameObject newObj = Instantiate(cardBearPrefab, Vector3.zero, Quaternion.identity,
                        bearsListContainer.transform);
                    newObj.name = bear.gameName;

                    Image image = newObj.transform.Find("Icon").GetComponent<Image>();
                    image.sprite = bear.sprite;
                    image.SetNativeSize();
                }
            }

            bearsListAs.UpdateContentSize();
        }
    }

    private void FixedUpdate()
    {
        bearsCountText.text = bearsInColony.Count + "/" + maxBears; // костыль
        if (bearsListMenu.activeSelf)
        {
            foreach (Transform child in bearsListContainer.transform)
            {
                Bear bear = GetBear(child.name);
                child.transform.Find("TextInfo").GetComponent<TextMeshProUGUI>().text = "Имя: " + bear.bearName +
                    "\nТрадиция: " + bear.tradition.GetString() + "\nТекущее дело: " + bear.activity.GetString() +
                    "\nУсталость/голод: " +
                    (Mathf.Round(bear.tired * 10) / 10) + "/" + (Mathf.Round(bear.hungry * 10) / 10);
            }
        }
    }
}