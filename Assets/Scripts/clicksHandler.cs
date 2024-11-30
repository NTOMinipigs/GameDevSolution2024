using System.Collections;
using UnityEngine;

public class clicksHandler : MonoBehaviour
{
    public float sensitivity;
    public bool blockMove;
    private bool isDragging;
    private Vector3 lastMousePosition, delta;
    private GameObject choicedBear;
    [SerializeField] private LayerMask layerMaskInteract;
    [SerializeField] private allScripts scripts;

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Выделение медведя
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMaskInteract))
        {
            if (hit.collider.gameObject.CompareTag("bear"))
            {
                if (choicedBear != hit.collider.gameObject)
                {
                    if (choicedBear != null)
                        choicedBear.GetComponent<bearMovement>().SetNormal();
                    choicedBear = hit.collider.gameObject;
                    choicedBear.GetComponent<bearMovement>().SetChoiced();
                }
            }
            else
            {
                if (choicedBear != null)
                {
                    choicedBear.GetComponent<bearMovement>().SetNormal();
                    choicedBear = null;
                }
            }
        }
        else
        {
            if (choicedBear != null)
            {
                choicedBear.GetComponent<bearMovement>().SetNormal();
                choicedBear = null;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
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
