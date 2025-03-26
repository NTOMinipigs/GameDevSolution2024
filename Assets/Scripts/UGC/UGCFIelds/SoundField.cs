
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
        /// Название трека
        /// </summary>
        [SerializeField] private TextMeshPro trackName;
        
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
            ExtensionFilter[] filters = { new ExtensionFilter("Sound files", "mp3", "wav") };
            string path = StandaloneFileBrowser.OpenFilePanel("Open Sound File", "", filters, false)[0];
            
            
        }

    }
}