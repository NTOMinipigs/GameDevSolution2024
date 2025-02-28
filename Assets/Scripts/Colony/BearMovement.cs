using UnityEngine;

public class BearMovement : MonoBehaviour
{
    // Новое
    public Bear totalBear;
    public float speed;
    public float rotationSpeed = 5f;
    private float waitTime;
    public float startWaitTime;
    private bool _wait = true;
    private bool _doingTask;
    [SerializeField] private Vector3 moveTarget;
    [SerializeField] private LayerMask rayInteractLayerMask;
    public AllScripts scripts;
    private Animator _animator;

    private void Start()
    {
        waitTime = startWaitTime;
        transform.eulerAngles = new Vector3(0, 0, 0);
        scripts = GameObject.Find("scripts").GetComponent<AllScripts>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        BearTask newTask = scripts.colonyManager.GetBearTask(totalBear);

        var taskColliders = Physics.OverlapSphere(transform.position, 3f, rayInteractLayerMask);
        if (taskColliders.Length > 0)
        {
            if (newTask != null)
                _doingTask = taskColliders[0].gameObject == newTask.objectOfTask;
            else
                _doingTask = false;
        }
        else
            _doingTask = false;

        if (newTask != null && !_doingTask)
            moveTarget = new Vector3(newTask.objectOfTask.transform.position.x, transform.position.y,
                newTask.objectOfTask.transform.position.z);

        if (_wait && newTask == null)
        {
            _animator.SetBool("walk", false);
            waitTime -= Time.deltaTime;
            if (waitTime < 0)
            {
                _wait = false;
                if (totalBear.activity == Activities.Chill)
                    moveTarget =
                        new Vector3(scripts.colonyManager.spawnBears.transform.position.x + Random.Range(-50f, 50f),
                            transform.position.y,
                            scripts.colonyManager.spawnBears.transform.position.z + Random.Range(-50f, 50f));
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, moveTarget, speed * Time.deltaTime);
            _animator.SetBool("walk", true);

            // Поворачиваем объект в сторону moveTarget
            Vector3 direction = (moveTarget - transform.position).normalized; // Вычисляем направление
            if (direction != Vector3.zero) // Проверяем, что направление не нулевое
            {
                Quaternion lookRotation = Quaternion.LookRotation(-direction); // Создаем вращение
                transform.rotation =
                    Quaternion.Slerp(transform.rotation, lookRotation,
                        rotationSpeed * Time.deltaTime); // Плавно поворачиваем
            }

            if (Vector3.Distance(transform.position, moveTarget) < 0.5f)
            {
                _wait = true;
                waitTime = startWaitTime;
            }
        }
    }

    private void FixedUpdate()
    {
        BearTask newTask = scripts.colonyManager.GetBearTask(totalBear);
        if (_doingTask)
        {
            newTask.totalSteps += 0.001f;
            if (newTask.objectOfTask.tag == "building")
                newTask.objectOfTask.GetComponent<BuildingController>().reveal.progress = newTask.totalSteps;

            if (newTask.totalSteps >= newTask.needSteps)
            {
                scripts.colonyManager.EndTask(newTask);
                Debug.Log(newTask.objectOfTask);
                _doingTask = false;
            }
        }

        // Говно реализация, переделать
        if (totalBear.activity == Activities.Work)
        {
            totalBear.tired += 0.0005f;
            totalBear.hungry += 0.00007f;
        }
        else if (totalBear.activity == Activities.Chill)
        {
            if (totalBear.hungry >= 5)
            {
                // Сделать чтобы он шел кушать, время прошло, и он снова отдыхает/работает
            }

            if (totalBear.tired >= 0)
                totalBear.tired -= 0.005f;
        }
    }
}