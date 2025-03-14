using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
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
    public GameObject camera;

    public float sensitivityOfCamera
    {
        get => _sensitivityOfCamera;
        set
        {
            _sensitivityOfCamera = value;
            CameraMove.Singleton.sensitivity = value;
            SystemSaver.Singleton.gameSave.PreferenceSave.sensitivity = value;
        }
    }

    public bool postProcessing
    {
        get => _postProcessing;
        set
        {
            _postProcessing = value;
            UpdatePostProcessing();
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
    
    /// <summary>
    /// Обновите постпроцессинг на камере
    /// </summary>
    private void UpdatePostProcessing()
    {
        PostProcessLayer postProcessLayer = camera.GetComponent<PostProcessLayer>();
        PostProcessVolume postProcessVolume = camera.GetComponent<PostProcessVolume>();
        
        postProcessLayer.enabled = _postProcessing;
        postProcessVolume.enabled = _postProcessing;
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