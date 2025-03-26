namespace UGC.UGCFIelds
{
    public class StringField : BaseField
    {
        /// <summary>
        /// Значение для этой хуйни
        /// </summary>
        private string _value;
        
        /// <summary>
        /// Getter for value, set private
        /// </summary>
        public object Value => _value;
        
        /// <summary>
        /// Метод вызываемый при измеенении 
        /// </summary>
        /// <param name="value"></param>
        private void OnValueChanged(string value)
        {
            _value = value;
        }
    }
}