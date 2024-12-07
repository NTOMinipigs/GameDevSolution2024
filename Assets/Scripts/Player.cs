using UnityEngine;

/// <summary>
/// Я не придумал как это в общем назвать, но нам где-то нужно хранить логин игрока, так что да
/// мне лень заворачивать это в синглтон, дохуя хочешь, храни в статик
/// </summary>
public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    public string playerName;

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
