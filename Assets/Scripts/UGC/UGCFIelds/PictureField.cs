using System.IO;
using SFB;
using UnityEngine;

namespace UGC.UGCFIelds
{
    public class PictureField : BaseField
    {
    
        /// <summary>
        /// 
        /// </summary>
        private string _value;

        /// <summary>
        /// Getter for value private set
        /// </summary>
        public object Value => _value;

        
        /// <summary>
        /// Название мода
        /// </summary>
        private string modName;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="name">Название мода</param>
        public PictureField(string name)
        {
            modName = name;
        }
        
        /// <summary>
        /// Вызовите это при нажатие на кнопку
        /// </summary>
        private void OnButtonClick()
        {
            ImportImage();
        }


        private void ImportImage()
        {
            ExtensionFilter[] filters = { new("Image Files", "png", "jpg") };
            string path = StandaloneFileBrowser.OpenFilePanel("Open Image File", "", filters, false)[0];
            _value = Path.GetFileName(path);
            File.Copy(path, Path.Combine(Application.streamingAssetsPath, "Mods", modName, _value), true);
        }
    }
}