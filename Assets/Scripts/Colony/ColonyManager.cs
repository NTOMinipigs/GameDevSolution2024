using System.Collections.Generic;
using ArgumentException = System.ArgumentException;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
            _honey = value;
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
            honeyText.text = _honey.ToString() + "/" + _maxHoney.ToString();
        }
    }

    [Header("-Biofuel")]
    [SerializeField] private TextMeshProUGUI biofuelText;
    private float _biofuel;
    public float Biofuel
    {
        get { return _biofuel; }
        set
        {
            _biofuel = value;
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
            biofuelText.text = _biofuel.ToString() + "/" + _maxBiofuel.ToString();
        }
    }

    [Header("-materials")]
    [SerializeField] private TextMeshProUGUI materialsText;
    private float _materials;
    public float Materials
    {
        get { return _materials; }
        set
        {
            _materials = value;
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
            _materialsPlus = value;
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
            _food = value;
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
            foodText.text = _food.ToString() + "/" + _maxFood.ToString();
        }
    }

    [Header("Bears")]
    public List<Bear> bearsInColony = new List<Bear>();
    public int maxBears;
    public List<BearTask> bearTasks = new List<BearTask>();
    [SerializeField] private GameObject spawnBears; // Потом сделать spawnBears более рандомным
    [SerializeField] private string[] menBearsFirstnames, womanBearsFirstnames, bearsLastnames = new string[0];
    [SerializeField] private SerializableBear[] spriteBeekeepers, spriteConstructors, spriteProgrammers, spriteBioengineers = new SerializableBear[0];
    [SerializeField] private TextMeshProUGUI bearsCountText;
    public GameObject bearsListMenu;
    [SerializeField] private GameObject bearsListContainer;
    [SerializeField] private GameObject cardBearPrefab;
    [SerializeField] private adaptiveScroll bearsListAS;

    [Header("Other")]
    [SerializeField] private allScripts scripts;
    public enum typeOfResource { materials, materialPlus, food, bioFuel, honey }

    private void Start()
    {
        // Чисто для тестов
        GenerateNewBear(Bear.Traditions.Beekeepers);
        GenerateNewBear(Bear.Traditions.Programmers);
        GenerateNewBear(Bear.Traditions.Beekeepers);
        GenerateNewBear(Bear.Traditions.Programmers);
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
    private SerializableBear GetSerializableBear(Bear.Traditions tradition)
    {
        return tradition switch  // Упростил выражение
        {
            Bear.Traditions.Beekeepers => spriteBeekeepers[Random.Range(0, spriteBeekeepers.Length - 1)],
            Bear.Traditions.Constructors => spriteConstructors[Random.Range(0, spriteConstructors.Length - 1)],
            Bear.Traditions.Programmers => spriteProgrammers[Random.Range(0, spriteProgrammers.Length - 1)],
            Bear.Traditions.BioEngineers => spriteBioengineers[Random.Range(0, spriteBioengineers.Length - 1)],
            _ => throw new ArgumentException("Tradition " + tradition + " not found!")
        };
    }

    /// <summary>
    /// Генерирует нового медведя
    /// </summary>
    /// <param name="tradition"></param>
    /// <exception cref="ArgumentException"></exception>
    private void GenerateNewBear(Bear.Traditions tradition)
    {
        SerializableBear serializableBear = GetSerializableBear(tradition);
        string bearName = GetBearName(serializableBear.gender);

        // TODO: сделать норм индексацию
        Bear newBear = new Bear(tradition.ToString() + Random.Range(0, 1000), bearName, tradition, serializableBear.sprite);
        bearsInColony.Add(newBear);
        GameObject bearObj = Instantiate(serializableBear.prefab, spawnBears.transform.position + new Vector3(Random.Range(-50f, 50f), 0, Random.Range(-50f, 50f)), Quaternion.identity);
        bearObj.name = newBear.gameName;
        bearObj.GetComponent<BearMovement>().totalBear = newBear;
        bearsCountText.text = bearsInColony.Count.ToString() + "/" + maxBears;
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
            if ((bear.activity == Bear.Activities.chill || bear.totalTask == null) && bear.tradition != Bear.Traditions.Chrom)
                return bear;
        }
        return null;
    }

    /// <summary>
    /// Сгенерировать имя основываясь на гендере
    /// </summary>
    public void CreateNewTask(BearTask.TasksMode newTaskMode, GameObject objectOfTask, float steps)
    {
        // TODO: сделать возможнсть работы по кастам
        BearTask task = new BearTask(newTaskMode, objectOfTask, steps);
        Bear chillBear = GetChillBear();
        if (chillBear != null)
        {
            task.selectedBear = chillBear;
            chillBear.totalTask = task;
            chillBear.activity = Bear.Activities.work;
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
            Debug.Log(task.selectedBear);
            if (task.selectedBear == null)
            {
                task.selectedBear = bear;
                bear.totalTask = task;
                bear.activity = Bear.Activities.work;
                break;
            }
        }
        // Если работы не нашлось
        if (task.taskMode == BearTask.TasksMode.None)
            bear.activity = Bear.Activities.chill;
    }

    public void EndTask(BearTask task)
    {
        if (task.taskMode == BearTask.TasksMode.build)
        {
            task.objectOfTask.GetComponent<Building>().SetNormal();
            task.objectOfTask.GetComponent<Building>().builded = true;
        }
        Bear selectedBear = task.selectedBear;
        bearTasks.Remove(task);

        selectedBear.totalTask = new BearTask(); // Очистка

        if (selectedBear.tired >= 5 || selectedBear.hungry >= 5)
            selectedBear.activity = Bear.Activities.chill;
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
            scripts.clicksHandler.blockMove = bearsListMenu.activeSelf;
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

