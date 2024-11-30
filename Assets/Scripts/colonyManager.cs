using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Random = UnityEngine.Random;

public class ColonyManager : MonoBehaviour
{
    [Header("Main resources")]
    [Header("-Honey")]
    [SerializeField] private TextMeshProUGUI honeyText;
    private float _honey;
    public float Honey
    {
        get { return _honey; }
        set
        {
            _honey = value;
            honeyText.text = _honey.ToString();
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
            biofuelText.text = _biofuel.ToString();
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
            materialsText.text = _materials.ToString();
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

    [Header("-food")]
    [SerializeField] private TextMeshProUGUI foodText;
    private float _food;
    public float Food
    {
        get { return _food; }
        set
        {
            _food = value;
            foodText.text = _food.ToString();
        }
    }

    [Header("Bears")]
    public List<Bear> bearsInColony = new List<Bear>();
    [SerializeField] private GameObject bearProgerPrefab, bearBeekeepersPrefab, bearConstructorPrefab, spawnBears; // Потом сделать spawnBears более рандомным
    [SerializeField] private string[] menBearsFirstnames, womanBearsFirstnames, bearsLastnames = new string[0];
    [SerializeField] private Sprite[] spriteBeekeepers, spriteConstructors, spriteProgrammers, spriteBioengineers = new Sprite[0];
    [SerializeField] private TextMeshProUGUI bearsCountText;
    [SerializeField] private GameObject bearsListMenu, bearsListContainer;
    [SerializeField] private GameObject cardBearPrefab;
    [SerializeField] private adaptiveScroll bearsListAS;

    [Header("Other")]
    [SerializeField] private allScripts scripts;

    private void Start()
    {
        // Чисто для тестов firstnamesForBears
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
    /// Генерирует нового медведя
    /// </summary>
    /// <param name="tradition"></param>
    /// <exception cref="ArgumentException"></exception>
    private void GenerateNewBear(Bear.Traditions tradition)
    {
        // Имя не зависит от гендера
        bool gender = GetBearGender();
        string bearName = GetBearName(gender);        Sprite newIcon = null;
        GameObject bearPrefab = null;
        switch (tradition)
        {
            case Bear.Traditions.Beekeepers:
                newIcon = spriteBeekeepers[Random.Range(0, spriteBeekeepers.Length - 1)];
                bearPrefab = bearBeekeepersPrefab;
                break;
            case Bear.Traditions.Constructors:
                newIcon = spriteConstructors[Random.Range(0, spriteConstructors.Length - 1)];
                bearPrefab = bearConstructorPrefab;

                break;
            case Bear.Traditions.Programmers:
                newIcon = spriteProgrammers[Random.Range(0, spriteProgrammers.Length - 1)];
                bearPrefab = bearProgerPrefab;
                break;
            case Bear.Traditions.BioEngineers:
                newIcon = spriteBioengineers[Random.Range(0, spriteBioengineers.Length - 1)];
                bearPrefab = bearProgerPrefab;
                break;
        }
        
        if (newIcon == null) {
            throw new System.ArgumentException("No found tradition " + tradition);
        }

        // TODO: сделать норм индексацию
        Bear newBear = new Bear(tradition.ToString() + Random.Range(0, 1000), bearName, tradition, newIcon);
        newBear.gender = gender;
        bearsInColony.Add(newBear);
        GameObject bearObj = Instantiate(bearPrefab, spawnBears.transform.position + new Vector3(Random.Range(-50f, 50f), 0, Random.Range(-50f, 50f)), Quaternion.identity);
        bearObj.name = newBear.gameName;
        bearObj.GetComponent<BearMovement>().totalBear = newBear;
        bearsCountText.text = bearsInColony.Count.ToString();
    }

    /// <summary>
    /// Возвращает случайный гендер медведя
    /// </summary>
    /// <returns>true если медведь мужчина, иначе false</returns>
    private bool GetBearGender()
    {
        return Random.value > 0.5f;
    }

    /// <summary>
    /// Получить имя медведя основываясь на гендере
    /// </summary>
    /// <returns>ФИ медведя</returns>
    private string GetBearName(bool gender)
    {
        string firstName = gender ? menBearsFirstnames[Random.Range(0, menBearsFirstnames.Length - 1)] : womanBearsFirstnames[Random.Range(0, menBearsFirstnames.Length - 1)];
        string lastName = bearsLastnames[Random.Range(0, bearsLastnames.Length - 1)];
        
        return firstName + " " + lastName;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
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
                child.transform.Find("TextInfo").GetComponent<TextMeshProUGUI>().text = "Имя: " + bear.bearName + "\nТрадиция: " + bear.traditionStr + "\nТекущее дело: " + bear.activityStr + "\nУсталость/голод: " + (Mathf.Round(bear.tired * 10) / 10) + "/" + (Mathf.Round(bear.hungry * 10) / 10);
            }
        }
    }
}

[Serializable]
public class Bear
{

    public bool gender; // Если гендер true - мужской пол, иначе женский
    public string gameName;
    public string bearName;
    public Sprite sprite;
    public enum Traditions { None, Beekeepers, Constructors, Programmers, BioEngineers, Special, Chrom }
    public Traditions tradition;
    [HideInInspector]
    public string traditionStr
    {
        get
        {
        return tradition switch
        {
            Traditions.Beekeepers => "Пасечник",
            Traditions.Constructors => "Конструктор",
            Traditions.Programmers => "Программист",
            Traditions.BioEngineers => "Биоинженер",
            Traditions.Chrom => "Первопроходец",
            _ => throw new System.ArgumentException("Tradition " + tradition + " not found!")
        };
        }
    }
    
    public enum activities { chill, work, eat }
    public activities activity;
    public string activityStr
    {
        get
        {
            switch (activity)
            {
                case activities.chill:
                    return "отдыхаю";
                case activities.work:
                    return "работаю";
                case activities.eat:
                    return "ем";
            }
            return "";
        }
    }
    
    public float lvl = 0f;
    public float hungry, tired;

    public Bear(string _gameName, string _bearName, Traditions _tradition, Sprite _sprite)
    {
        gameName = _gameName;
        bearName = _bearName;
        tradition = _tradition;
        sprite = _sprite;
    }
}