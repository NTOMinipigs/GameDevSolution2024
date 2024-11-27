using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class colonyManager : MonoBehaviour
{
    public int totalDay = 1;
    public TextMeshProUGUI dayText;

    [Header("Main resources")]
    [Header("-Honey")]
    [SerializeField] private TextMeshProUGUI honeyText;
    private float _honey;
    public float honey
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
    public float biofuel
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
    public float materials
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
    public float food
    {
        get { return _food; }
        set
        {
            _food = value;
            foodText.text = _food.ToString();
        }
    }

    [Header("Bears")]
    public List<bear> bearsInColony = new List<bear>();
    [SerializeField] private string[] firstnamesForBears, secondnamesForBears = new string[0];
    [SerializeField] private Sprite[] spriteBeekeepers, spriteConstructors, spriteProgrammers, spriteBioengineers = new Sprite[0];
    [SerializeField] private TextMeshProUGUI bearsCountText;
    [SerializeField] private GameObject bearsListMenu, bearsListContainer;
    [SerializeField] private GameObject cardBearPrefab;
    [SerializeField] private adaptiveScroll bearsListAS;

    [Header("Other")]
    [SerializeField] private allScripts scripts;

    private void Start()
    {
        dayText.text = totalDay.ToString() + " день";
        // Чисто для тестов
        GenerateNewBear("", bear.traditions.beekeepers);
        GenerateNewBear("", bear.traditions.programmers);
        GenerateNewBear("", bear.traditions.beekeepers);
        GenerateNewBear("", bear.traditions.programmers);
    }

    public void GenerateNewBear(string gameName, bear.traditions tradition)
    {
        string bearName = firstnamesForBears[Random.Range(0, firstnamesForBears.Length - 1)] + " " + secondnamesForBears[Random.Range(0, secondnamesForBears.Length - 1)];
        Sprite newIcon = null;
        switch (tradition)
        {
            case bear.traditions.beekeepers:
                newIcon = spriteBeekeepers[Random.Range(0, spriteBeekeepers.Length - 1)];
                break;
            case bear.traditions.constructors:
                newIcon = spriteConstructors[Random.Range(0, spriteConstructors.Length - 1)];
                break;
            case bear.traditions.programmers:
                newIcon = spriteProgrammers[Random.Range(0, spriteProgrammers.Length - 1)];
                break;
            case bear.traditions.bioEngineers:
                newIcon = spriteBioengineers[Random.Range(0, spriteBioengineers.Length - 1)];
                break;
        }
        bear newBear = new bear("", bearName, tradition, newIcon);
        bearsInColony.Add(newBear);
        bearsCountText.text = bearsInColony.Count.ToString();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            bearsListMenu.gameObject.SetActive(!bearsListMenu.activeSelf);
            if (bearsListMenu.activeSelf)
            {
                foreach (Transform child in bearsListContainer.transform)
                    Destroy(child.gameObject);

                foreach (bear bear in bearsInColony)
                {
                    GameObject newObj = Instantiate(cardBearPrefab, Vector3.zero, Quaternion.identity, bearsListContainer.transform);
                    newObj.transform.Find("Icon").GetComponent<Image>().sprite = bear.sprite;
                    newObj.transform.Find("TextInfo").GetComponent<TextMeshProUGUI>().text = "Имя: " + bear.bearName + "\nТрадиция: " + bear.traditionStr + "\nТекущее дело" + "\nУсталость/голод: " + bear.tired + "/" + bear.hungry;
                }
                // Почему-то при ПЕРВОМ открытии - метод нормально не срабатывает и sizeDelta.y == 0. При дальнейших открытиях все норм
                bearsListAS.UpdateContentSize();
            }
        }
    }
}

[System.Serializable]
public class bear
{
    public string gameName;
    public string bearName;
    public Sprite sprite;
    public enum traditions { none, beekeepers, constructors, programmers, bioEngineers, special, chrom }
    public traditions tradition;
    [HideInInspector]
    public string traditionStr
    {
        get
        {
            switch (tradition)
            {
                case traditions.beekeepers:
                    return "Пасечник";
                case traditions.constructors:
                    return "Конструктор";
                case traditions.programmers:
                    return "Программист";
                case traditions.bioEngineers:
                    return "Биоинженер";
                case traditions.chrom:
                    return "Первопроходец";
            }
            return "";
        }
    }
    public float lvl = 0f;
    public float hungry, tired;

    public bear()
    {

    }

    public bear(string _gameName, string _bearName, traditions _tradition, Sprite _sprite)
    {
        gameName = _gameName;
        bearName = _bearName;
        tradition = _tradition;
        sprite = _sprite;
    }
}