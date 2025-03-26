using TMPro;
using UnityEngine.UI;

namespace UGC.UGCFields
{
    public class StringUGCFieldAttribute : BaseUGCFieldAttribute
    {
        public StringUGCFieldAttribute(string name, string description) : base(name, description)
        {
            InputField input = Prefab.GetComponent<InputField>();
            TextMeshProUGUI text = Prefab.GetComponent<TextMeshProUGUI>();
            
            input.placeholder.GetComponent<TextMeshPro>().text = description;
            text.text = name;
        }
    }
}