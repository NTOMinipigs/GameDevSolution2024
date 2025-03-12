using UnityEngine;
using UnityEngine.UI;

public class Preference : MonoBehaviour
{
    public static Preference Singleton { get; private set; }
    [SerializeField] private GameObject preferenceMenu;
    private float _sensitivityOfCamera;
    private Slider sliderOfSens;
    private float _globalVolume;
    private Slider sliderOfVolume;
    private bool _postProcessing;

    public float sensitivityOfCamera
    {
        get => _sensitivityOfCamera;
        set
        {
            _sensitivityOfCamera = value;
            SystemSaver.Singleton.gameSave.PreferenceSave.sensitivity = value;
        }
    }

    public bool postProcessing
    {
        get => _postProcessing;
        set
        {
            _postProcessing = value;
            SystemSaver.Singleton.gameSave.PreferenceSave.postProcessing = value;
        }
    }
    
    public float globalVolume
    {
        get => _globalVolume;
        set
        {
            _globalVolume = value;
            SystemSaver.Singleton.gameSave.PreferenceSave.globalVolume = value;
        }
    }
    
    private void Awake()
    {
        Singleton = this;
    }

    public void ManagePrefenceMenu()
    {
        preferenceMenu.gameObject.SetActive(!preferenceMenu.activeSelf);
        if (sliderOfSens == null) // Первое обнаружение
        {
            sliderOfSens = preferenceMenu.transform.Find("sensOfCam").GetComponent<Slider>();
            sliderOfVolume = preferenceMenu.transform.Find("volumeChange").GetComponent<Slider>();
        }
    }

    public void Update() // Знаю, что это неправильно... Но как есть
    {
        // if (preferenceMenu.activeSelf)
        // {
        //     sliderOfSens.value = sensitivityOfCamera;
        //     sliderOfVolume.value = globalVolume;
        // }
    }
}