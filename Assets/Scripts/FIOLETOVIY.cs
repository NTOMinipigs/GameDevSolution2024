
using System.Threading.Tasks;
using UnityEngine;

public class FIOLETOVIY : MonoBehaviour
{
    [SerializeField] public allScripts allScripts;
    public int segments = 10; // Количество точек линии (сегментов)
    public float radius = 1f; // Радиус вокруг сферы
    public float randomness = 0.5f; // Величина случайного отклонения
    private GameObject red;
    private GameObject blue;
    private GameObject purple;
    private float upTicks = 0;
    private float mergeTicks = 0;
    private string status = "up";
    public Material lineMaterial; // Материал для LineRenderer
    
    void SpawnSpheres(GameObject bear)
    {
        red = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        red.transform.localScale = new Vector3(50, 50, 50);
        red.transform.position = new Vector3(bear.transform.position.x - 100, bear.transform.position.y + 100, bear.transform.position.z);
        red.GetComponent<Renderer>().material.color = Color.red;
        
        blue = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        blue.transform.localScale = new Vector3(50, 50, 50);
        blue.transform.position = new Vector3(bear.transform.position.x + 100 , bear.transform.position.y + 100, bear.transform.position.z);
        blue.GetComponent<Renderer>().material.color = Color.blue;
    }

    async Task UpSpheres()
    {
        while (upTicks < 50)
        {
            upTicks += 1;

            red.transform.position = new Vector3(red.transform.position.x, red.transform.position.y + 1,
                red.transform.position.z);
            blue.transform.position = new Vector3(blue.transform.position.x, blue.transform.position.y + 1,
                blue.transform.position.z);
            await Task.Delay(50);
        }
    }

    async Task MergeSpheres()
    {
        while (mergeTicks < 100)
        {
            mergeTicks += 1;
            red.transform.position = new Vector3(red.transform.position.x + 1, red.transform.position.y,
                red.transform.position.z);
            blue.transform.position = new Vector3(blue.transform.position.x - 1, blue.transform.position.y,
                blue.transform.position.z);
            await Task.Delay(50);
        }
    }

    void CreatePurpleSphere()
    {
        purple = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        purple.transform.localScale = new Vector3(70, 70, 70);
        purple.transform.position = new Vector3(red.transform.position.x, red.transform.position.y, red.transform.position.z);
        purple.GetComponent<Renderer>().material.color = Color.magenta;
        
        Destroy(blue);
        Destroy(red);
    }
    
    void GenerateLightning()
    
    {
        // Создаем объект для LineRenderer
        GameObject lineObject = new GameObject("ElectricEffect");
        lineObject.transform.parent = purple.transform; // Делаем дочерним объектом сферы
        lineObject.transform.localPosition = purple.transform.position;
        
        // Добавляем компонент LineRenderer
        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();

        // Настройка LineRenderer
        lineRenderer.material = lineMaterial; // Применяем материал
        lineRenderer.startWidth = 0.05f; // Толщина линии в начале
        lineRenderer.endWidth = 0.05f;   // Толщина линии в конце
        lineRenderer.loop = true;       // Линия замыкается в кольцо
        lineRenderer.positionCount = segments + 1; // Устанавливаем количество точек
        lineRenderer.transform.position = new Vector3(purple.transform.position.x, purple.transform.position.y, purple.transform.position.z);
        // Генерация позиций для сегментов линии
        for (int i = 0; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2 / segments; // Угол для текущего сегмента
            float x = Mathf.Cos(angle) * radius + Random.Range(-randomness, randomness);
            float y = Random.Range(-randomness, randomness); // Отклонение по вертикали
            float z = Mathf.Sin(angle) * radius + Random.Range(-randomness, randomness);

            // Устанавливаем позицию сегмента
            lineRenderer.SetPosition(i, new Vector3(x, y, z));
        }
    }

    void SendPurple()
    {
        purple.transform.position = new Vector3(purple.transform.position.x + 1, purple.transform.position.y, purple.transform.position.z + 1);
    }


    public async void Start()
    {
        await Task.Delay(1000);
        gameObject.GetComponent<MusicManager>().Audios["FIOLETOVIY"].Play();
        SpawnSpheres(GameObject.Find(allScripts.colonyManager.bearsInColony[2].gameName));
        await UpSpheres();
        await MergeSpheres();
        CreatePurpleSphere();
        await Task.Delay(10500);
        status = "pizda";
    }

    // void Update()
    // {
    //     if (purple != null)
    //     GenerateLightning();
    // } 
    void Update()
    {
        if (status == "pizda")
            SendPurple();
    }

}

