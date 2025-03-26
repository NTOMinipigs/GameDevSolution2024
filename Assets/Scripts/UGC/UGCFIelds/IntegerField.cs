namespace UGC.UGCFIelds
{
    /// <summary>
    /// Поле числа=
    /// </summary>
    public class IntegerField : BaseField
    {
        /// <summary>
        /// Значение для этой хуйни
        /// </summary>
        private int _value;
        
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
            _value = int.Parse(value);
        }
    }
}