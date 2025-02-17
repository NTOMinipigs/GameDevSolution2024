using UnityEngine;

public class BearMovement : MonoBehaviour
{// Новое
    public Bear totalBear;
    public float speed;
    public float rotationSpeed = 5f;
    private float waitTime;
    public float startWaitTime;
    private bool wait = true;
    private bool doingTask;
    [SerializeField] private Vector3 moveTarget;
    public AllScripts scripts;
    private Animator animator;

    private void Start()
    {
        waitTime = startWaitTime;
        transform.eulerAngles = new Vector3(0, 0, 0);
        scripts = GameObject.Find("scripts").GetComponent<AllScripts>();
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (scripts.colonyManager.GetBearTask(totalBear) != null)
        {
            if (collider.gameObject == scripts.colonyManager.GetBearTask(totalBear).objectOfTask)
                doingTask = true;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (scripts.colonyManager.GetBearTask(totalBear) != null)
        {
            if (collider.gameObject == scripts.colonyManager.GetBearTask(totalBear).objectOfTask)
                doingTask = false;
        }
    }

    private void Update()
    {
        BearTask newTask = scripts.colonyManager.GetBearTask(totalBear);
        if (newTask != null)
            moveTarget = new Vector3(newTask.objectOfTask.transform.position.x, transform.position.y, newTask.objectOfTask.transform.position.z);

        if (wait && newTask == null)
        {
            animator.SetBool("walk", false);
            waitTime -= Time.deltaTime;
            if (waitTime < 0)
            {
                wait = false;
                if (totalBear.activity == Activities.Chill)
                    moveTarget = new Vector3(transform.position.x + Random.Range(-100f, 100f), transform.position.y, transform.position.z + Random.Range(-100f, 100f));
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, moveTarget, speed * Time.deltaTime);
            animator.SetBool("walk", true);

            // Поворачиваем объект в сторону moveTarget
            Vector3 direction = (moveTarget - transform.position).normalized; // Вычисляем направление
            if (direction != Vector3.zero) // Проверяем, что направление не нулевое
            {
                Quaternion lookRotation = Quaternion.LookRotation(-direction); // Создаем вращение
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime); // Плавно поворачиваем
            }

            if (Vector3.Distance(transform.position, moveTarget) < 0.5f)
            {
                wait = true;
                waitTime = startWaitTime;
            }
        }
    }

    private void FixedUpdate()
    {
        BearTask newTask = scripts.colonyManager.GetBearTask(totalBear);
        if (doingTask)
        {
            newTask.totalSteps += 0.01f;
            if (newTask.totalSteps >= newTask.needSteps)
            {
                scripts.colonyManager.EndTask(newTask);
                Debug.Log(newTask.objectOfTask);
                doingTask = false;
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
