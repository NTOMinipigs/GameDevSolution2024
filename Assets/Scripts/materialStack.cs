using UnityEngine;

public class materialStack : MonoBehaviour
{
    [SerializeField] private GameObject canvas;

    public void ActivateInteraction()
    {
        canvas.gameObject.SetActive(true);
    }

    public void StartGettingMaterial()
    {
        // Реализовать чтобы мишка шел к стэку, начинал добывать, через время добылось и он обратно. Еще учитывать коэфф усталости и голода.
    }
}
