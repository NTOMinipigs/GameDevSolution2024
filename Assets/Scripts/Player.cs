using UnityEngine;

/// <summary>
/// Я не придумал как это в общем назвать, но нам где-то нужно хранить логин игрока, так что да
/// мне лень заворачивать это в синглтон, дохуя хочешь, храни в статик
/// </summary>
public class Player : MonoBehaviour
{
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Уничтожаем дубликат
        }
    }
    
    
    public static Player Instance { get; private set; }   
    public string playerName;
}
