using UnityEngine;

public class Zoom : MonoBehaviour
{
    /// <summary>
    /// Минимальное поле зрения (или близость)
    /// </summary>
    [SerializeField] public float minZoom = 20f;
    
    /// <summary>
    /// Максимальное поле зрения (или дальность)
    /// </summary>
    [SerializeField] public float maxZoom = 60f;
    
    /// <summary>
    /// Чувствительность
    /// </summary>
    [SerializeField] public float sensitivity = 10f;

    
    private Camera _camera;
    
    /// <summary>
    /// Инициализация
    /// </summary>
    public void Start()
    {
        _camera = GetComponent<Camera>();   
        Time.timeScale = 1f;
    }
    
    /// <summary>
    /// Обновлять каждый раз
    /// </summary>
    public void Update()
    {
        if (_camera.orthographic)
        {
            // Для ортографической камеры изменяем размер
            _camera.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * sensitivity * 5;
            _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize, minZoom, maxZoom);
        }
        else
        {
            // изменяем поле зрения
            _camera.fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * sensitivity * 5;
            _camera.fieldOfView = Mathf.Clamp(_camera.fieldOfView, minZoom, maxZoom);
        }
    }
}
