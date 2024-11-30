using System.Collections;
using UnityEngine;

public class clicksHandler : MonoBehaviour
{
    public float sensitivity;
    public bool blockMove;
    private bool isDragging;
    private Vector3 lastMousePosition, delta;
    [SerializeField] private LayerMask layerMaskInteract;
    [SerializeField] private allScripts scripts;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMaskInteract))
            {
                if (hit.collider.gameObject.tag == "bear")
                {
                    bear selectedBear = scripts.colonyManager.GetBear(hit.collider.gameObject.name);
                    scripts.dialogManager.ActivateBearInteractionDialog(selectedBear);
                }
                else if (hit.collider.gameObject.tag == "materialStack")
                    hit.collider.gameObject.GetComponent<materialStack>().ActivateInteraction();
            }
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
