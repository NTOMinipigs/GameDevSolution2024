
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
    public AudioSource source;
    
    /// <summary>
    /// Конструктор, вызывается исключительно из MusicManager
    /// </summary>
    /// <param name="source">AudioSource of track</param>
    public Audio(AudioSource source)
    {
        this.source = source;
    }
    
    /// <summary>
    /// Прокси между AudioSource.Play() и Audio
    /// Нужно для обратной совместимки
    /// </summary>
    public void Play()
    {
        source.Play();
    }

    /// <summary>
    /// Прокси между AudioSource.Stop() и Audio
    /// Нужно для обратной совместимки
    /// </summary>
    public void Stop()
    {
        source.Stop();
    }

    /// <summary>
    /// Прокси между AudioSource.Pause() и Audio
    /// Нужно для обратной совместимки
    /// </summary>
    public void Pause()
    {
        source.Pause();
    }

    /// <summary>
    /// Прокси между AudioSource.UnPause() и Audio
    /// Нужно для обратной совместимки
    /// </summary>
    public void Resume()
    {
        source.UnPause();
    }
    
    /// <summary>
    /// Плавно изменить звук за delta до targetVolume
    /// </summary>
    /// <param name="targetVolume">Конечный результат</param>
    /// <returns>IEnumerator (корутина)</returns>
    private IEnumerator FadeEffect(float targetVolume)
    {
        float startVolume = source.volume;
        float time = 0;

        while (time < FadeDuration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, targetVolume, time / FadeDuration);
            yield return null;
        }

        source.volume = targetVolume;
    }
    
    /// <summary>
    /// Запустить звук с Fade эффектом в начале и в конце
    /// </summary>
    /// <returns></returns>
    public IEnumerator PlayWithFadeEffect()
    {
        source.volume = 0f;
        source.Play();
        yield return FadeEffect(1f); // входной фейд эффект
        
        while ( // Ждем когда нужно будет уменьшать громкость
               source.time != 0
               &&
               source.time < source.clip.length - FadeDuration * 2)
        {
            yield return null;
        }

        // Если трек остановился в точке 0, скорее всего к нему преминили метод .Stop(), в таком случае дальше продолжать не стоит
        if (source.time == 0)
        {
            yield return null;
        }
        
        yield return FadeEffect(0f); // Выходной эффект
        source.Stop();
    }
    
}
