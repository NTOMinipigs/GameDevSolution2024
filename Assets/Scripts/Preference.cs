using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Preference : MonoBehaviour
{
    public static Preference Singleton { get; private set; }
    [SerializeField] private GameObject preferenceMenu;
    public float sensitivityOfCamera = 5f;
    private Slider sliderOfSens;
    public float globalVolume = 100f;
    private Slider sliderOfVolume;
    public bool postProcessing = true;

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