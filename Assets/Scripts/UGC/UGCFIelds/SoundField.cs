
using System.IO;
using SFB;
using TMPro;
using UnityEngine;

namespace UGC.UGCFIelds
{
    
    
    /// <summary>
    /// Поле ввода музыки
    /// </summary>
    public class SoundField : BaseField
    {   
        /// <summary>
        /// Значение для JSON
        /// </summary>
        private string _value;
        
        
        /// <summary>
        /// Getter for _value, value have only getter
        /// </summary>
        public object Value => _value;

        /// <summary>
        /// Название трека
        /// </summary>
        [SerializeField] private TextMeshPro trackName;

        /// <summary>
        /// название мода 
        /// </summary>
        private string modName;
        
        /// <summary>
        /// Конструктор
        /// </summary>
        public SoundField(string modName)
        {
            this.modName = modName;
        }
        
        /// <summary>
        /// Клик по кнопке
        /// </summary>
        private void ButtonClick()
        {
            ImportFile();
        }

        /// <summary>
        /// Открывается меню выбора файла, чел выбирает новый файл
        /// </summary>
        private void ImportFile()
        {
            ExtensionFilter[] filters = { new("Sound files", "mp3", "wav") };
            string path = StandaloneFileBrowser.OpenFilePanel("Open Sound File", "", filters, false)[0];
            _value = Path.GetFileName(path);
            File.Copy(path, Path.Combine(Application.streamingAssetsPath, "Mods", modName, _value), true);
        }

    }
}