using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace Config
{
    /// <summary>
    /// Менеджер конфига, загрузка, изменение и т.д.
    /// Singleton паттерн
    /// </summary>
    public class ConfigManager : MonoBehaviour
    {
        /// <summary>
        /// Инстанс, участник Singleton паттерна
        /// </summary>
        public static ConfigManager Instance { get; private set; }

        public Config config { get; private set; }

        /// <summary>
        /// Названия конфига. Конфиг обаязательно должен находиться в папке Assets/StreamAssets
        /// </summary>
        private const string CONFIG_NAME = "config.json";
        
        /// <summary>
        /// Инициализируем Singleton
        /// </summary>
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                Initialize();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Подгрузим конфиг
        /// </summary>
        private void Initialize()
        {
            config = LoadDebugConfig();
        }
        
        /// <summary>
        /// Загрузите и сериализуйте конфиг
        /// </summary>
        /// <returns>Объект Config</returns>
        /// <exception cref="FileNotFoundException">Если конфиг не найден. В идеале на этом запуск игры должен остановиться</exception>
        private Config LoadDebugConfig()
        {
            // Путь к конфигу
            string path = Path.Combine(Application.streamingAssetsPath, CONFIG_NAME);
            
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<Config>(json);
            }
            throw new FileNotFoundException("Файл конфигурации не найден!");
        }
        
    }
}