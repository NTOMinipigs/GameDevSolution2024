using UnityEngine;

public class BuildingSystem : MonoBehaviour
{
    public Vector2Int GridSize = new Vector2Int(10, 10);
    private Building[,] grid;
    private Building flyingBuilding;
    private Camera mainCamera;

    private void Awake()
    {
        grid = new Building[GridSize.x, GridSize.y];
        mainCamera = Camera.main;
    }

    public void StartPlacingBuilding(Building buildingPrefab)
    {
        if (flyingBuilding != null)
            Destroy(flyingBuilding.gameObject);

        flyingBuilding = Instantiate(buildingPrefab);
    }

    private void Update()
    {
        if (flyingBuilding != null)
        {
            var groundPlane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (groundPlane.Raycast(ray, out float position))
            {
                Vector3 worldPosition = ray.GetPoint(position);

                int x = Mathf.RoundToInt(worldPosition.x);
                int y = Mathf.RoundToInt(worldPosition.z);

                x -= flyingBuilding.Size.x / 2;
                y -= flyingBuilding.Size.y / 2;

                bool available = true;

                if (x < -GridSize.x / 2 || x > GridSize.x / 2 - flyingBuilding.Size.x) available = false;
                if (y < -GridSize.y / 2 || y > GridSize.y / 2 - flyingBuilding.Size.y) available = false;

                if (available && IsPlaceTaken(x, y)) available = false;

                flyingBuilding.transform.position = new Vector3(x + flyingBuilding.Size.x / 2f, 0, y + flyingBuilding.Size.y / 2f);
                flyingBuilding.SetTransparent(available);

                if (available && Input.GetMouseButtonDown(0))
                    PlaceFlyingBuilding(x, y);
            }
        }
    }

    /*
    private void OnDrawGizmosSelected()
    {
        // Визуализируем сетку
        Gizmos.color = Color.green;
        for (int x = -GridSize.x / 2; x < GridSize.x / 2; x++)
        {
            for (int y = -GridSize.y / 2; y < GridSize.y / 2; y++)
            {
                Vector3 position = new Vector3(x, 0, y);
                Gizmos.DrawWireCube(position + new Vector3(0.5f, 0, 0.5f), new Vector3(1f, 0, 1f));
            }
        }
    }
    */

    private bool IsPlaceTaken(int placeX, int placeY)
    {
        for (int x = 0; x < flyingBuilding.Size.x; x++)
        {
            for (int y = 0; y < flyingBuilding.Size.y; y++)
                if (grid[placeX + x, placeY + y] != null) return true;
        }

        return false;
    }

    private void PlaceFlyingBuilding(int placeX, int placeY)
    {
        for (int x = 0; x < flyingBuilding.Size.x; x++)
        {
            for (int y = 0; y < flyingBuilding.Size.y; y++)
                grid[placeX + x, placeY + y] = flyingBuilding;
        }

        flyingBuilding.SetNormal();
        flyingBuilding = null;
    }
}