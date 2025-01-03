using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


/// <summary>
/// Мендежер музыки, обращаться к нему по любому поводу
/// </summary>
public class MusicManager : MonoBehaviour
{
    /// <summary>
    /// Список MusicLoops заготовленных заранее
    /// </summary>
    public Dictionary<string, AudioLoop> AudioLoops = new();

    /// <summary>
    /// Список аудио треков присутствующих в игре
    /// </summary>
    public readonly Dictionary<string, Audio> Audios = new();

    public async void Start()
    {
        // Добавляем все известные музыкальные треки в список аудио
        foreach (AudioSource audioSource in GameObject.Find("Music").GetComponentsInChildren<AudioSource>())
        {
            Debug.Log(audioSource.name);
            Audios[audioSource.name] = new Audio(audioSource);
        }

        // Добавляем все известные звуки в список аудио
        foreach (AudioSource audioSource in GameObject.Find("Sounds").GetComponentsInChildren<AudioSource>())
        {
            Debug.Log(audioSource.name);
            Audios[audioSource.name] = new Audio(audioSource);
        }

        // Формируем известные заранее musicloop'ы

        // Цикл музыки
        AudioLoops["MusicLoop"] = new AudioLoop(
            new[]
            {
                Audios["BeerLofi"],
                Audios["SnowBeer"],
                Audios["Baltika9beer"],
                Audios["WindBeer"]
            },
            true
            );
        
        // Цикл кирки
        AudioLoops["pickaxe"] = new AudioLoop(
            new[]
            {
                Audios["kirka_1"],
                Audios["kirka_2"],
                Audios["kirka_3"],
                Audios["kirka_4"],
                Audios["kirka_5"],
                Audios["kirka_6"],
                Audios["kirka_7"]
            }
        );
        
        // Цикл шагов снега
        AudioLoops["snow_steps"] = new AudioLoop(
            new[]
            {
                Audios["snow_steps_1"],
                Audios["snow_steps_2"]
            }
        );

        await Task.Delay(1000);
    }
}