using System.IO;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// Механизм отвечающий за сохранение в json
/// </summary>
public class SystemSaver : MonoBehaviour
{

    /// <summary>
    /// Здесь хранится информация о игре
    /// </summary>
    public GameSave gameSave;
    
    
    /// <summary>
    /// В singleton паттерне конструктор приватный
    /// </summary>
    private SystemSaver() {}
    

    /// <summary>
    /// Получить название json файла для конкретного игрока
    /// </summary>
    /// <returns>путь к .json</returns>
    private string GetFilePath()
    {
        Debug.Log(Application.persistentDataPath);
        
        return Path.Combine(Application.persistentDataPath, Player.Instance.playerName + ".json");
    }

    /// <summary>
    /// Выгрузка в json
    /// </summary>
    public void SaveGame()
    {
        string json = JsonConvert.SerializeObject(gameSave, Formatting.Indented);
        File.WriteAllText(GetFilePath(), json);
        Debug.Log("Данные сохранены: " + json);
    }

    /// <summary>
    /// Загрузка из json
    /// </summary>
    /// <returns>boolean - true если файл сохранения существует, иначе false</returns>
    public bool LoadGame()
    {
        string filePath = GetFilePath();

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            gameSave = JsonConvert.DeserializeObject<GameSave>(json);
            return true;
        }
        
        Debug.LogWarning("Файл сохранения не найден.");
        return false;
    }
}