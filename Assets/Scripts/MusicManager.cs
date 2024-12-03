using System;
using System.Collections;
using UnityEngine;


/// <summary>
/// Мендежер музыки, обращаться к нему по любому поводу
/// </summary>
public class MusicManager : MonoBehaviour
{

    public void Start()
    {
        StartCoroutine(StartPlaylist(_musicClips, true));
    }
    
    /// <summary>
    /// Текущий трек/звук. Нужен чтобы удобно взаимодействовать с текущим треком изнутри
    /// </summary>
    private AudioSource _currentAudioSource;

    /// <summary>
    /// Длительность fade эффекта в конце и в начале
    /// </summary>
    private const float FadeDuration = 2.0f;

    public bool IsPlaying { get; } = false;

    [SerializeField] private AudioSource[] _musicClips = new AudioSource[0];

    /// <summary>
    /// Запускает один трек
    /// </summary>
    /// <param name="audioSource">Ссылка на объект с музыкой</param>
    /// <param name="fade">Включить фейд эффект, по умолчанию false</param>
    public void StartAudio(AudioSource audioSource, bool fade = false)
    {
        _currentAudioSource = audioSource;

        // Логика фейд эффекта
        if (fade)
        {
            StartCoroutine(PlayWithFadeEffect(_currentAudioSource));
            return;
        }
        
        // Запустим без фейда
        audioSource.Play();
    }
    
    

    /// <summary>
    /// Остановить текущий трек
    /// </summary>
    /// <exception cref="NullReferenceException">Если сейчас ничего не играет</exception>
    public void StopAudio()
    {
        if (_currentAudioSource == null)
        {
            throw new NullReferenceException("audiosource is null");
        }
        
        _currentAudioSource.Stop();
        _currentAudioSource.volume = 1f; // Сбросим громкость, вдруг менялась.
        // Возможно ты задашь вопрос нахера я менял громкость, если _currentAudioSource в итоге стал null. Так вот, _currentAudioSource это не объект, а рефка на него
        _currentAudioSource = null;
    }

    /// <summary>
    /// Приостановить аудио, но оставить возможность проигрывать дальше
    /// </summary>
    /// <exception cref="NullReferenceException">Если сейчас ничего не играет</exception>
    public void PauseAudio()
    {
        if (_currentAudioSource == null)
        {
            throw new NullReferenceException("audiosource is null");
        }
        
        _currentAudioSource.Pause();
    }

    /// <summary>
    /// Используй, чтобы продолжить воиспроизведение трека.
    /// В отличие от AudioSource.Start() продолжает воиспроизводить трек с момента, когда он был остановлен, а так же сохраняет фейды  
    /// </summary>
    /// <exception cref="NullReferenceException">Если сейчас ничего не играет</exception>
    public void ResumeAudio()
    {
        if (_currentAudioSource == null)
        {
            throw new NullReferenceException("audiosource is null");
        }
        _currentAudioSource.Play();
    }

    /// <summary>
    /// Циклично воиспроизводить треки переданные первым аргументом
    /// </summary>
    /// <param name="audioSources">Плейлист (массив сурсов аудио)</param>
    /// <param name="fade">Включить фейд эффект, по умолчанию false</param>
    public IEnumerator StartPlaylist(AudioSource[] audioSources, bool fade = false)
    {
        byte CurrentIndex = 0; // Индекс текущего трека в плейлисте
        _currentAudioSource = audioSources[CurrentIndex];
        
        while (_currentAudioSource != null)
        {

            // Запускаем трек с фейдом/без фейда
            if (fade)
            {
                // Запустим первый трек и начнем дожидаться фейд эффекта в конце
                StartCoroutine(PlayWithFadeEffect(_currentAudioSource));
                yield return new WaitForSeconds(_currentAudioSource.clip.length - 2); 
            }

            // Иначе запускаем трек без фейд эффекта
            else
            {
                _currentAudioSource.Play();
                yield return new WaitForSeconds(_currentAudioSource.clip.length);
            }
            
            // Если плейлист закончился, начать заново
            if (CurrentIndex == audioSources.Length - 1)
            {
                CurrentIndex = 0;
                _currentAudioSource = audioSources[CurrentIndex];
                continue;
            }
            
            // Иначе идти дальше по трекам
            CurrentIndex++;
            _currentAudioSource = audioSources[CurrentIndex];
        }
    }
    
    /// <summary>
    /// Плавно изменить звук за delta до targetVolume
    /// </summary>
    /// <param name="audioSource"></param>
    /// <param name="targetVolume">Конечный результат</param>
    /// <returns>IEnumerator (корутина)</returns>
    private IEnumerator FadeEffect(AudioSource audioSource, float targetVolume)
    {
        float startVolume = audioSource.volume;
        float time = 0;

        while (time < FadeDuration)
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, time / FadeDuration);
            yield return null;
        }

        audioSource.volume = targetVolume;
    }

    /// <summary>
    /// Запустить звук с Fade эффектом в начале и в конце
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayWithFadeEffect(AudioSource audioSource)
    {
        audioSource.volume = 0f;
        audioSource.Play();
        yield return StartCoroutine(FadeEffect(audioSource, 1f)); // входной фейд эффект
        
        while ( // Ждем когда нужно будет уменьшать громкость
               audioSource.time != 0
            &&
            audioSource.time < audioSource.clip.length - FadeDuration * 2)
        {
            yield return null;
        }

        // Если трек остановился в точке 0, скорее всего к нему преминили метод .Stop(), в таком случае дальше продолжать не стоит
        if (audioSource.time == 0)
        {
            yield return null;
        }
        
        yield return StartCoroutine(FadeEffect(_currentAudioSource, 0f)); // Выходной эффект
        audioSource.Stop();
    }
}