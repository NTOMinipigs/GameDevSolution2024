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
    [SerializeField] private GameObject spawnBears; // Потом сделать spawnBears более рандомным
    [SerializeField] private string[] menBearsFirstnames, womanBearsFirstnames, bearsLastnames = new string[0];
    [SerializeField] private SerializableBear[] spriteBeekeepers, spriteConstructors, spriteProgrammers, spriteBioengineers = new SerializableBear[0];
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
        Bear newBear = new Bear(tradition.ToString() + Random.Range(0, 1000), bearName , tradition, serializableBear.sprite);
        bearsInColony.Add(newBear);
        GameObject bearObj = Instantiate(serializableBear.prefab, spawnBears.transform.position + new Vector3(Random.Range(-50f, 50f), 0, Random.Range(-50f, 50f)), Quaternion.identity);
        bearObj.name = newBear.gameName;
        bearObj.GetComponent<BearMovement>().totalBear = newBear;
        bearsCountText.text = bearsInColony.Count.ToString();
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
                child.transform.Find("TextInfo").GetComponent<TextMeshProUGUI>().text = "Имя: " + bear.bearName + "\nТрадиция: " + bear.TraditionStr + "\nТекущее дело: " + bear.ActivityStr + "\nУсталость/голод: " + (Mathf.Round(bear.tired * 10) / 10) + "/" + (Mathf.Round(bear.hungry * 10) / 10);
            }
        }
    }
}
