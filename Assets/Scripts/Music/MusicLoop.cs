
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Класс отвечающий за цикличное проигрывание музыки
/// </summary>
public class AudioLoop
{
    /// <summary>
    /// Текущий трек
    /// НЕ МЕНЯТЬ МОДИФИКАТОРЫ!
    /// </summary>
    public Audio CurrentAudio {get; private set; }

    /// <summary>
    /// Громкость треков в цикле
    /// </summary>
    public float Volume;
    
    /// <summary>
    /// Использовать ли FadeEffect
    /// </summary>
    public readonly bool FadeEffect;
    
    /// <summary>
    /// Запускать треки в случайном порядке?
    /// </summary>
    public readonly bool Randomize;

    /// <summary>
    /// Возможно тебе нужно воиспроизвести цикл только один раз?
    /// </summary>
    public readonly bool PlayOnce;
    
    /// <summary>
    /// Костыль
    /// </summary>
    private static AllScripts _allScripts = GameObject.FindObjectOfType<AllScripts>();

    /// <summary>
    /// Длина фейд эффекта
    /// </summary>
    private int _fadeEffectLength = 2;

    /// <summary>
    /// Индекс текущего трека в массиве
    /// </summary>
    private byte _currentIndex;
    
    /// <summary>
    /// Список треков для воиспроизведения
    /// НЕ СТОИТ делать публичным. Может быть опасно для цикла
    /// </summary>
    private Audio[] _audios;
    
    /// <summary>
    /// Здесь будет храниться корутина PlayExecutor, в момент, когда цикл запущен
    /// Использовать для возможности остановить корутину
    /// </summary>
    private Coroutine _playExecutorCoroutine;

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="audioSources">Список треков, которые будут крутиться в loop'е</param>
    /// <param name="volume">Громкость треков в цикле, non req</param>
    /// <param name="fadeEffect">Использовать эффект затухания? non req</param>
    /// <param name="randomize">Включать треки в случайном порядке? non req</param>
    /// <param name="playOnce">Проиграть цикл один раз, см. док this.PlayOnce</param>
    public AudioLoop(
        Audio[] audioSources, 
        float volume = 1.0f,
        bool fadeEffect = false,
        bool randomize = false,
        bool playOnce = false)
    {
        _audios = audioSources;
        Volume = volume;
        FadeEffect = fadeEffect;
        Randomize = randomize;
        PlayOnce = playOnce;
    }

    /// <summary>
    /// Начать воиспроизведение треков с начала
    /// </summary>
    public void Play()
    {
        if (_playExecutorCoroutine != null)
        {
            // Поставьте сюда какую-то более "говорящую" ошибку
            throw new Exception("Цикл уже запущен");
        }
        
        // Записываем корутину в филд, и отправляем исполняться
        _playExecutorCoroutine = _allScripts.StartCoroutine(PlayExecutor());
    }
    
    /// <summary>
    /// ДОСТАТОЧНО НЕОЧЕВИДНЫЙ МОМЕНТ
    /// Этот участок кода вынесен в отдельный метод, так как мне нужно сохранять объект корутины Play()
    /// Для взаимодействия с ним вне области видимости этого метода. Например его нужно останавливать
    /// Вынести это в лямбду нельзя - так как лямбды не могут быть корутинами
    /// UNSAVE!
    /// </summary>
    /// <returns>IEnumerator - coroutine</returns>
    private IEnumerator PlayExecutor()
    {
        SetNewAudio();
        
        // Цикл не бесконечный, он умрет как только я использую StopCoroutine()
        while (true)
        {
            // Ставим громкость на указанную в конструкторе
            CurrentAudio.Source.volume = Volume;
            
            // Запускаем трек с фейдом/без фейда
            if (FadeEffect)
            {
                // Запустим первый трек и начнем дожидаться фейд эффекта в конце
                _allScripts.StartCoroutine(CurrentAudio.PlayWithFadeEffect());
                yield return new WaitForSeconds(CurrentAudio.Source.clip.length - _fadeEffectLength); 
            }
            
            // Иначе запускаем трек без фейд эффекта
            else
            {
                CurrentAudio.Play();
                yield return new WaitForSeconds(CurrentAudio.Source.clip.length);
            }
            
            CurrentAudio.Source.volume = 1.0f; // Возвращаем в стандартное положение, на всякий
            SetNewAudio();
        }
    }

    /// <summary>
    /// Установить следующий трек для проигрывания 
    /// </summary>
    private void SetNewAudio()
    {
        // Логика на случай, если треки нужно включать в случайном порядке
        if (Randomize)
        {
            int number;
            do
            { 
                number = Random.Range(0, _audios.Length - 1); // Генерируем случайный индекс
            } while (number == _currentIndex); // Проверяем, чтобы индекс не был равен предыдущему
            
            _currentIndex = (byte) number;
            CurrentAudio = _audios[_currentIndex];
        }

        // Иначе
        else
        {
            if (CurrentAudio == null)
            {
                _currentIndex = 0;
                CurrentAudio = _audios[_currentIndex];
                return;
            }
            
            // Если плейлист закончился, начать заново 
            // Или если PlayOnce = true, остановить проигрывание
            if (_currentIndex == _audios.Length - 1)
            {
                if (PlayOnce)
                {
                    // Плейлист закончился, дропаем корутину
                    _allScripts.StopCoroutine(_playExecutorCoroutine);
                    return;
                }
                
                _currentIndex = 0;
                CurrentAudio = _audios[_currentIndex];
                return;
            }
            
            // Иначе идти дальше по трекам
            _currentIndex++;
            CurrentAudio = _audios[_currentIndex];
        }
    }

    /// <summary>
    /// Полностью остановить loop
    /// </summary>
    public void Stop()
    {
        // Убиваем корутину Play
        _allScripts.StopCoroutine(_playExecutorCoroutine);
        _playExecutorCoroutine = null;
        _currentIndex = 0;
        CurrentAudio.Stop();
        CurrentAudio = null;
    }

    /// <summary>
    /// Поставить текущий трек на паузу
    /// </summary>
    public void Pause()
    {
        // Убиваем корутину Play
        _allScripts.StopCoroutine(_playExecutorCoroutine);
        _playExecutorCoroutine = null;
        CurrentAudio.Pause();
    }
    
    /// <summary>
    /// Возобновить работу плейлиста после паузы
    /// Корутина, желательно бы сделать void, но лень
    /// </summary>
    public IEnumerator Resume()
    {
        CurrentAudio.Resume();
        yield return new WaitForSeconds(CurrentAudio.Source.clip.length - CurrentAudio.Source.time); // Ждем конца текущего трека
        _allScripts.StartCoroutine(PlayExecutor());
    }
}
