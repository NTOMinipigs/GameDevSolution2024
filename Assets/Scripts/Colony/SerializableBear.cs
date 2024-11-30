using UnityEngine;


/// <summary>
/// Этот класс нужен для удобной работы с сериализованными спрайтами в юнити, в формате спрайт-гендер
/// </summary>
[System.Serializable]
class SerializableBear : MonoBehaviour
{
    
    /// <summary>
    /// Этот enum сделан чтобы не плодить магические true и false (буквально заменяет их)
    /// </summary>
    public enum Gender
    {
        Men,
        Women
    }
    
    // Соответственно филды которые нужно сериализовать
    public Gender gender;
    public Sprite sprite;
    public GameObject prefab;
}