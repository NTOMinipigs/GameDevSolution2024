using UnityEngine;

/// <summary>
/// Движение камеры в главном меню
/// </summary>
public class CameraMenu : MonoBehaviour
{
    public float speed = 100f; // Скорость по X и Y

    public float leftBoundary;
    public float rightBoundary;  
    public float bottomBoundary;
    public float topBoundary;      
    
    
    private Vector3 direction; 

    void Start()
    {
        // Случайное начальное направление
        direction = new Vector3(
            Random.value > 0.5f ? 1f : -1f,
            Random.value > 0.5f ? 1f : -1f,
            0
        );
    }

    void Update()
    {
        Vector3 newPosition = transform.position + new Vector3(speed * direction.x, speed * direction.y, 0) * Time.deltaTime;

        // Разворот по X
        if (newPosition.x < leftBoundary || newPosition.x > rightBoundary)
        {
            direction.x *= -1; 
            newPosition.x = Mathf.Clamp(newPosition.x, leftBoundary, rightBoundary);
        }

        // Разворот по Y
        if (newPosition.y < bottomBoundary || newPosition.y > topBoundary)
        {
            direction.y *= -1; 
            newPosition.y = Mathf.Clamp(newPosition.y, bottomBoundary, topBoundary);
        }

        transform.position = newPosition;
    }
}
