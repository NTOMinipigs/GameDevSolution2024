using UnityEngine;

public class BearMovement : MonoBehaviour
{// Новое
    public Bear totalBear;
    public float speed;
    private float waitTime;
    public float startWaitTime;
    private bool wait = true;
    [SerializeField] private Vector3 moveTarget;
    [SerializeField] private GameObject downPlane;

    private void Start()
    {
        waitTime = startWaitTime;
        transform.eulerAngles = new Vector3(0, 0, 0);
    }

    public void SetChoiced() => downPlane.gameObject.SetActive(true);

    public void SetNormal() => downPlane.gameObject.SetActive(false);

    private void Update()
    {
        if (wait)
        {
            waitTime -= Time.deltaTime;
            if (waitTime < 0)
            {
                wait = false;
                if (totalBear.activity == ActivityManager.Activities.Сhill)
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
        if (totalBear.activity == ActivityManager.Activities.Work)
        {
            totalBear.tired += 0.0015f;
            totalBear.hungry += 0.00065f;
        }
        else if (totalBear.activity == ActivityManager.Activities.Сhill)
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
