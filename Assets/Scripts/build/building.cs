using UnityEngine;

public class Building : MonoBehaviour
{
    public string buildingName;
    public enum TypesOfBuilding {building, resource}
    public TypesOfBuilding typeOfBuilding;
    public ColonyManager.typeOfResource typeResource; // Тип ресурса, если выбран TypesOfBuilding.resource
    [SerializeField] private Renderer MainRenderer;
    public float materialsNeed, specMaterialsNeed;
    public Vector2Int Size = Vector2Int.one;

    // Смена цвета по возможности расстановки
    public void SetTransparent(bool available) => MainRenderer.material.color = available ? Color.green : Color.red;

    // Смена цвета на нормальный
    public void SetNormal() => MainRenderer.material.color = Color.white;

    // Это ебать не должно
    private void OnDrawGizmosSelected()
    {
        Vector3 offset = new Vector3(-Size.x * 0.5f + 0.5f, 0, -Size.y * 0.5f + 0.5f);

        for (int x = 0; x < Size.x; x++)
        {
            for (int y = 0; y < Size.y; y++)
            {
                Gizmos.color = (x + y) % 2 == 0 ? new Color(0.88f, 0f, 1f, 0.3f) : new Color(1f, 0.68f, 0f, 0.3f);
                Gizmos.DrawCube(transform.position + offset + new Vector3(x, 0, y), new Vector3(1, .1f, 1));
            }
        }
    }
}