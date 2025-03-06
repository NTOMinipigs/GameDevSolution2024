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
        _directLight.intensity = 0.3f; // Стартовое значение
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
            // Смена глобального света в зависимости от времени. Цикл для плавности
            if (_worldHours < 16)
            {
                for (int i = 0; i < 10; i++)
                {
                    yield return new WaitForSeconds(0.00000001f);
                    _directLight.intensity += 0.0035f;
                }
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    yield return new WaitForSeconds(0.00000001f);
                    _directLight.intensity -= 0.006f;
                }
            }
        }

        _textTime.text = _worldHours.ToString("D2") + ":" + _worldMinuts.ToString("D2");
        yield return new WaitForSeconds(2.5f); // Частота обновления дня
        StartCoroutine(WorldTimeChange());
    }
}