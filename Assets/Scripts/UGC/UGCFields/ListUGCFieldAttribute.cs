using System.Collections.Generic;
using UnityEngine.UI;

namespace UGC.UGCFields
{
    public class ListUGCFieldAttribute : BaseUGCFieldAttribute
    {
        /// <summary>
        /// Поля ввода
        /// </summary>
        private List<InputField> _inputFields = new();
        
        /// <summary>
        /// Стандартный конструктор
        /// </summary>
        /// <param name="name">Название</param>
        /// <param name="description">Описание для каждого из полей</param>
        public ListUGCFieldAttribute(string name, string description) : base(name, description)
        {
        }

        /// <summary>
        /// Создайте новый input field 
        /// </summary>
        private void CreateNewInputField()
        {
            
        }
    }
}