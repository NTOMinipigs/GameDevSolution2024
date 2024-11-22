using System.Collections;
using UnityEngine;

public class clicksHandler : MonoBehaviour
{
    public float sensitivity;
    public bool blockMove;
    private bool isDragging;
    private Vector3 lastMousePosition;
    private Vector3 delta;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
            isDragging = false;

        if (isDragging && !blockMove)
        {
            delta = (Input.mousePosition - lastMousePosition) * sensitivity * Time.deltaTime;

            transform.position += new Vector3(-delta.x, 0, -delta.y);
            lastMousePosition = Input.mousePosition;
        }
    }
}
