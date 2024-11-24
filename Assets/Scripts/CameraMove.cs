using UnityEngine;

public class CameraMove : MonoBehaviour
{

    // Слой стены, нужен для рейкаста
    public LayerMask wallLayer;
    Vector2 direction = new Vector2(1, 0.48f);
    float speed = 1f;

    void Update() {
        Vector2 moveDelta = direction.normalized * speed * Time.deltaTime;

        // Проверяем столкновение с границами коллайдера
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, moveDelta.magnitude + 0.1f, wallLayer);
        if (hit.collider != null)
        {
            direction = Vector2.Reflect(direction, hit.normal);
        }
        else
        {
            // Если столкновений нет, перемещаем камеру
            transform.position += (Vector3)moveDelta;
        }
    }
}
