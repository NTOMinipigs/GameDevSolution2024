using UnityEngine;

public class BearMovement : MonoBehaviour
{// Новое
    public Bear totalBear;
    public float speed;
    private float waitTime;
    public float startWaitTime;
    private bool wait = true;
    private bool doingTask;
    [SerializeField] private Vector3 moveTarget;
    public allScripts scripts;

    private void Start()
    {
        waitTime = startWaitTime;
        transform.eulerAngles = new Vector3(0, 0, 0);
        scripts = GameObject.Find("scripts").GetComponent<allScripts>();
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
            waitTime -= Time.deltaTime;
            if (waitTime < 0)
            {
                wait = false;
                if (totalBear.activity == Bear.Activities.chill)
                    moveTarget = new Vector3(transform.position.x + Random.Range(-100f, 100f), transform.position.y, transform.position.z + Random.Range(-100f, 100f));
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, moveTarget, speed * Time.deltaTime);
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
                doingTask = false;
            }
        }

        // Говно реализация, переделать
        if (totalBear.activity == Bear.Activities.work)
        {
            totalBear.tired += 0.0005f;
            totalBear.hungry += 0.00007f;
        }
        else if (totalBear.activity == Bear.Activities.chill)
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
