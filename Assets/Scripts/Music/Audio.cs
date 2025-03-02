
using System.Collections;
using UnityEngine;


/// <summary>
/// Наименьшая единица в архитектуре MusicManager'а
/// Позволяет работать с треком на низком уровне
/// Фактически расширяет возможности AudioSource класса, но не отбирает возможность работать с ним напрямую, через source field
/// Не рекомендуется работать с этим классом вне MusicManager, однако предоставляется возможность при крайней необходимости 
/// </summary>
public class Audio
{
    private const float FadeDuration = 2.0f;
    
    /// <summary>
    /// Воиспроизводимый AudioSource. Публичный, однако НЕ РЕКОМЕНДУЕТСЯ РАБОТАТЬ С НИМ НА ПРЯМУЮ 
    /// </summary>
    public AudioSource Source;
    
    /// <summary>
    /// Конструктор, вызывается исключительно из MusicManager
    /// </summary>
    /// <param name="source">AudioSource of track</param>
    public Audio(AudioSource source)
    {
        Source = source;
    }
    
    /// <summary>
    /// Реальная громкость аудио
    /// </summary>
    private float _volume = 1f;
    
    /// <summary>
    /// getter and getter for _volume field
    /// setter: update volume for audiosource
    /// </summary>
    public float Volume
    {
        get => _volumeRatio;
        set
        {
            _volume = value;
            Source.volume = _volume * _volumeRatio;
        }
    }

    /// <summary>
    /// Коэффициент настраивается через меню настроек
    /// </summary>
    private float _volumeRatio = 1f;
    
    /// <summary>
    /// getter and setter for _volumeRatio field
    /// setter: update volume for audiosource
    /// </summary>
    public float volumeRatio
    {
        get => _volumeRatio;
        set
        {
            _volumeRatio = value;
            Source.volume = Volume * _volumeRatio;
        }
    }
    
    /// <summary>
    /// Прокси между AudioSource.Play() и Audio
    /// Нужно для обратной совместимки
    /// </summary>
    public void Play()
    {
        Source.Play();
    }

    /// <summary>
    /// Прокси между AudioSource.Stop() и Audio
    /// Нужно для обратной совместимки
    /// </summary>
    public void Stop()
    {
        Source.Stop();
        
        // Ресетаем параметры трека
        Volume = 1.0f;  
    }

    /// <summary>
    /// Прокси между AudioSource.Pause() и Audio
    /// Нужно для обратной совместимки
    /// </summary>
    public void Pause()
    {
        Source.Pause();
    }

    /// <summary>
    /// Прокси между AudioSource.UnPause() и Audio
    /// Нужно для обратной совместимки
    /// </summary>
    public void Resume()
    {
        Source.UnPause();
    }
    
    /// <summary>
    /// Плавно изменить звук за delta до targetVolume
    /// </summary>
    /// <param name="targetVolume">Конечный результат</param>
    /// <returns>IEnumerator (корутина)</returns>
    private IEnumerator FadeEffect(float targetVolume)
    {
        float startVolume = Source.volume;
        float time = 0;

        while (time < FadeDuration)
        {
            time += Time.deltaTime;
            Volume = Mathf.Lerp(startVolume, targetVolume, time / FadeDuration);
            yield return null;
        }

        Volume = targetVolume;
    }
    
    /// <summary>
    /// Запустить звук с Fade эффектом в начале и в конце
    /// </summary>
    /// <returns></returns>
    public IEnumerator PlayWithFadeEffect()
    {
        Volume = 0f;
        Source.Play();
        yield return FadeEffect(1f); // входной фейд эффект
        
        while ( // Ждем когда нужно будет уменьшать громкость
               Source.time != 0
               &&
               Source.time < Source.clip.length - FadeDuration * 2)
        {
            yield return null;
        }

        // Если трек остановился в точке 0, скорее всего к нему применили метод .Stop(), в таком случае дальше продолжать не стоит
        if (Source.time == 0)
        {
            yield return null;
        }
        
        yield return FadeEffect(0f); // Выходной эффект
        Stop();
    }
    
}
