using System.IO;
using System.Net.Mime;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UGC
{
    /// <summary>
    /// Manager для UGC, является высшим объектом в иерархии UCG
    /// </summary>
    public class UGCManager : MonoBehaviour
    {
        # region consts
        private const string IconName = "icon.png";
        private const string JsonName = "assets.json";
        private const string DefaultIcon = "default_icon.png";
        # endregion
        
        # region UI

        [SerializeField] private GameObject UGCPreviewPrefab;
        [SerializeField] public GameObject UGCViewsContent; // ВОообще он должен быть приватным, но я пидорас
        
        # endregion
        
        private readonly string _pathToMods = Path.Combine(Application.streamingAssetsPath, "Mods");

        /// <summary>
        /// Instance of UGCManager
        /// </summary>
        public static UGCManager Singleton {get; private set;}

        /// <summary>
        /// Awake
        /// </summary>
        void Awake()
        {
            Singleton = this; 
        }
        
        /// <summary>
        /// Создай новый UGC
        /// </summary>
        /// <param name="name">Название UGC</param>
        private void CreateNewUGC(string name)
        {
            string pathToNewMod = Path.Combine(_pathToMods, name);
            Directory.CreateDirectory(pathToNewMod);
            
            // Копируем дефолтную иконку набора в набор
            File.WriteAllBytes(Path.Combine(pathToNewMod, IconName), UnityEngine.Resources.Load<TextAsset>(DefaultIcon).bytes);

            // Создаем JSON
            UGC ugc = new();
            string json = JsonConvert.SerializeObject(ugc, Formatting.Indented);
            File.WriteAllText(Path.Combine(Application.streamingAssetsPath, name, JsonName), json);
        }

        /// <summary>
        /// Загрузите новый UGC
        /// </summary>
        /// <param name="name">Название набора</param>
        private UGC LoadUGC(string name)
        {
            string json = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, name, JsonName));
            return JsonConvert.DeserializeObject<UGC>(json);
        }

        /// <summary>
        /// Получите все UGC
        /// </summary>
        /// <returns></returns>
        private UGCPreview[] GetAllUGC()
        {
            string[] files = Directory.GetFiles(_pathToMods);
            UGCPreview[] results = new UGCPreview[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];
                
                // Подгружаем весь ugc для получения инфо для превью
                string json = File.ReadAllText(Path.Combine(file, JsonName));
                UGC ugc = JsonConvert.DeserializeObject<UGC>(json);
                
                
                // Получаем все значения для превью мода
                Sprite icon = LoadSpriteFromPath(Path.Combine(file, IconName));
                string fileName = ugc.Name;
                string description = ugc.Description; // TODO: Сделать подгрузку описания из JSON
                
                UGCPreview ugcPreview = new UGCPreview(icon, fileName, description);
                results[i] = ugcPreview;
            }

            return results;
        }


        /// <summary>
        /// Покажи все UGC в меню
        /// </summary>
        public void ShowAllUGC()
        {
            UGCPreview[] ugcs = GetAllUGC();
            foreach (UGCPreview ugc in ugcs)
            {
                GameObject preview = Instantiate(UGCPreviewPrefab, UGCViewsContent.transform);
                preview.GetComponentInChildren<Image>().sprite = ugc.Icon;
                
                TextMeshPro nameComponent =  preview.transform.Find("Name").GetComponent<TextMeshPro>();
                nameComponent.text = ugc.Name;
                
                TextMeshPro descriptionComponent = preview.transform.Find("Description").GetComponent<TextMeshPro>();
                descriptionComponent.text = ugc.Description;
            }
            
        }
         
        /// <summary>
        /// Загрузка спрайтов по пути к ним. В UnityEngine подобного нету
        /// </summary>
        /// <param name="filePath">Путь к файлу</param>
        /// <returns>Спрайт по пути</returns>
        private Sprite LoadSpriteFromPath(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Debug.LogError("File not found: " + filePath);
                return null;
            }

            // Читаем байты из а
            byte[] fileData = File.ReadAllBytes(filePath);
        
            // Создаем текстуру и загружаем в нее изображение
            Texture2D texture = new Texture2D(2, 2); // Размер не важен, он перезапишется
            if (!texture.LoadImage(fileData)) // Автоматически определяет формат (PNG, JPG)
            {
                Debug.LogError("Failed to load texture from bytes!");
                return null;
            }

            // Конвертируем Texture2D в Sprite
            Sprite sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height), // Область текстуры
                new Vector2(0.5f, 0.5f), // Pivot (центр по умолчанию)
                100 // Pixels per Unit (можно настроить)
            );

            return sprite;
        }
    }
}
