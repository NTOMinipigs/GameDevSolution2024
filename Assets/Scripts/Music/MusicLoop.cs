
using System.Collections;
using UnityEngine;

/// <summary>
/// Класс отвечающий за цикличное проигрывание музыки
/// </summary>
public class AudioLoop
{
    
    /// <summary>
    /// Костыль
    /// </summary>
    private static allScripts _allScripts = GameObject.FindObjectOfType<allScripts>();
    
    /// <summary>
    /// Играет ли плейлист сейчас?
    /// </summary>
    public bool IsPlaying { get; private set; }

    /// <summary>
    /// Использовать ли FadeEffect
    /// </summary>
    private bool _fadeEffect;
    
    /// <summary>
    /// Список треков для воиспроизведения
    /// </summary>
    private Audio[] _audioSources;
    
    /// <summary>
    /// Текущий трек
    /// </summary>
    private Audio _currentAudioSource;

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="audioSources">Список треков, которые будут крутиться в loop'е</param>
    /// <param name="fadeEffect"></param>
    public AudioLoop(Audio[] audioSources, bool fadeEffect = false)
    {
        _audioSources = audioSources;
        _fadeEffect = fadeEffect;
    }
    
    /// <summary>
    /// Циклично воиспроизводить треки переданные первым аргументом
    /// </summary>
    public IEnumerator Play()
    {
        byte currentIndex = 0; // Индекс текущего трека в плейлисте
        _currentAudioSource = _audioSources[currentIndex];
        
        while (_currentAudioSource != null)
        {
            // Запускаем трек с фейдом/без фейда
            if (_fadeEffect)
            {
                // Запустим первый трек и начнем дожидаться фейд эффекта в конце
                _allScripts.StartCoroutine(_currentAudioSource.PlayWithFadeEffect());
                yield return new WaitForSeconds(_currentAudioSource.source.clip.length - 2); 
            }
            
            // Иначе запускаем трек без фейд эффекта
            else
            {
                _currentAudioSource.Play();
                yield return new WaitForSeconds(_currentAudioSource.source.clip.length);
            }
            
            // Если плейлист закончился, начать заново
            if (currentIndex == _audioSources.Length - 1)
            {
                currentIndex = 0;
                _currentAudioSource = _audioSources[currentIndex];
                continue;
            }
            
            // Иначе идти дальше по трекам
            currentIndex++;
            _currentAudioSource = _audioSources[currentIndex];
        }
    }
    
}
