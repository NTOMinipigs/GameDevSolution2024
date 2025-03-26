using UnityEngine;

namespace UGC
{
    /// <summary>
    /// Превью для мода
    /// </summary>
    public class UGCPreview
    {
        /// <summary>
        /// Иконка набора
        /// </summary>
        public Sprite Icon;
        
        /// <summary>
        /// название набора
        /// </summary>
        public string Name;
        
        /// <summary>
        /// Описание набора
        /// </summary>
        public string Description;

        /// <summary>
        /// Конструктор, принимает все филды и назначает их
        /// </summary>
        /// <param name="icon">Иконка набора</param>
        /// <param name="name">Название набора</param>
        /// <param name="description">Описание</param>
        public UGCPreview(Sprite icon, string name, string description)
        {
            Icon = icon;
            Name = name;
            Description = description;
        }
    }
}