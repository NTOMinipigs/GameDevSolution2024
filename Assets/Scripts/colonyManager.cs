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
    [SerializeField]private GameObject bearPrefab, spawnBears; // Потом сделать spawnBears более рандомным
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
        return null;
    }

    /// <summary>
    /// Генерирует нового медведя
    /// </summary>
    /// <param name="tradition"></param>
    /// <exception cref="ArgumentException"></exception>
    private void GenerateNewBear(Bear.Traditions tradition)
    {
        bool gender = GetBearGender();
        string bearName = GetBearName(gender);

        // Здесь ставятся иконки. Все икноки, не завися от гендера, в одном месте
        Sprite[] selectedSprites = tradition switch
        {
            Bear.Traditions.Beekeepers => spriteBeekeepers,
            Bear.Traditions.Constructors => spriteConstructors,
            Bear.Traditions.Programmers => spriteProgrammers,
            Bear.Traditions.BioEngineers => spriteBioengineers,
        };

        Sprite newIcon = selectedSprites[Random.Range(0, selectedSprites.Length - 1)];

        if (newIcon == null) {
            throw new ArgumentException("No found tradition " + tradition);
        }

        // TODO: сделать норм индексацию
        Bear newBear = new Bear(tradition.ToString() + Random.Range(0, 1000), bearName, tradition, newIcon);
        newBear.gender = gender;
        bearsInColony.Add(newBear);
        GameObject bearObj = Instantiate(bearPrefab, spawnBears.transform.position, Quaternion.identity);
        bearObj.name = newBear.gameName;
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

                    Image image = newObj.transform.Find("Icon").GetComponent<Image>();
                    image.sprite = bear.sprite;
                    image.SetNativeSize();

                    newObj.transform.Find("TextInfo").GetComponent<TextMeshProUGUI>().text = "Имя: " + bear.bearName + "\nТрадиция: " + bear.traditionStr + "\nТекущее дело" + "\nУсталость/голод: " + bear.tired + "/" + bear.hungry;
                }
                // Почему-то при ПЕРВОМ открытии - метод нормально не срабатывает и sizeDelta.y == 0. При дальнейших открытиях все норм
                bearsListAS.UpdateContentSize();
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
            _ => throw new ArgumentException("Tradition " + tradition + " not found!")
        };
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