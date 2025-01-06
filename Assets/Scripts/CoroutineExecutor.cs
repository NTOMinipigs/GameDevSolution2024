
using UnityEngine;

/// <summary>
/// БОЛЬШУЩИЙ КОСТЫЛЬ!
/// Не все классы в проекте наследуются от MonoBehaviour
/// Однако некоторым нужно использовать методы MonoBehaviour, например StartCoroutine
/// Этот класс представляет интерфейс MonoBehaviour через Instance
/// Singleton pattern!
/// </summary>
public class CoroutineExecutor : MonoBehaviour
{
    public static CoroutineExecutor Instance;
    
    private void Awake()
    {
        Instance = this;
    }
}
