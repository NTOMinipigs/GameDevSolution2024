using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameEventsManager : MonoBehaviour
{
    private int _worldHours, _worldMinuts;
    private TextMeshProUGUI _textTime;
    private Light _directLight;

    private void Start()
    {
        _textTime = GameObject.Find("TextTime").GetComponent<TextMeshProUGUI>();
        _directLight = GameObject.Find("Directional Light").GetComponent<Light>();
        _directLight.intensity = 0.5f; // Стартовое значение
        StartCoroutine(WorldTimeChange());
    }

    // НЕ ВЫРЕЗАТЬ, ЭТО УПРАВЛЕНИЕ ВРЕМЕНЕМ ИГРЫ. В ПРЯМОМ БЛЯТЬ СМЫСЛЕ
    public void SetTimeScale(float timeScale) => Time.timeScale = timeScale;

    /// <summary>
    /// Обновление времени каждую N секунду + действия
    /// </summary>
    /// <returns></returns>
    private IEnumerator WorldTimeChange()
    {
        _worldMinuts += 10;
        if (_worldMinuts >= 60)
        {
            _worldMinuts = 0;
            _worldHours++;
            // Начало нового дня
            if (_worldHours == 24)
                _worldHours = 0;
            // Смена глобального света в зависимости от времени
            if (_worldHours < 16)
                _directLight.intensity += 0.06f;
            else
                _directLight.intensity -= 0.125f;
        }

        _textTime.text = _worldHours.ToString("D2") + ":" + _worldMinuts.ToString("D2");
        yield return new WaitForSeconds(1f);
        StartCoroutine(WorldTimeChange());
    }
}