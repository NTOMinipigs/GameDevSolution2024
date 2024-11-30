using UnityEngine;

public class bearMovement : MonoBehaviour
{// Новое
    public bear totalBear;
    public float speed;
    private float waitTime;
    public float startWaitTime;
    private bool wait = true;
    [SerializeField] private Vector3 moveTarget;

    private void Start()
    {
        waitTime = startWaitTime;
        transform.eulerAngles = new Vector3(0, 0, 0);
    }

    private void Update()
    {
        if (wait)
        {
            waitTime -= Time.deltaTime;
            if (waitTime < 0)
            {
                wait = false;
                if (totalBear.activity == bear.activities.chill)
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
        // Говно реализация, переделать
        if (totalBear.activity == bear.activities.work)
        {
            totalBear.tired += 0.0015f;
            totalBear.hungry += 0.00065f;
        }
        else if (totalBear.activity == bear.activities.chill)
        {
            if (totalBear.hungry >= 5)
            {
                // Сделать чтобы он шел кушать, время прошло, и он снова отдыхает/работает
            }
            if (totalBear.tired >= 0)
                totalBear.tired -= 0.002f;
        }
    
    }
}
