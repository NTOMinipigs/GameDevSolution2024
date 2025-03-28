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
    private Animator _animator;
    private GameObject _bearModel;

    private void Start()
    {
        waitTime = startWaitTime;
        transform.eulerAngles = new Vector3(0, 0, 0);
        _animator = GetComponent<Animator>();
        _bearModel = transform.Find("bear").gameObject;
    }

    private void Update()
    {
        BearTask newTask = BearTaskManager.Singleton.GetBearTask(totalBear);

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

        if (_wait && newTask == null || !totalBear.canMove)
        {
            _animator.SetBool("walk", false);
            // Если не может двигаться - таймер не двигаем
            if (!totalBear.canMove) return;
            waitTime -= Time.deltaTime;
            if (waitTime < 0)
            {
                _wait = false;
                if (totalBear.activity == Activities.Chill)
                    moveTarget =
                        new Vector3(ColonyManager.Singleton.spawnBears.transform.position.x + Random.Range(-50f, 50f),
                            transform.position.y,
                            ColonyManager.Singleton.spawnBears.transform.position.z + Random.Range(-50f, 50f));
            }
        }
        else
        {
            if (moveTarget == Vector3.zero) // Ну вдруг
            {
                _wait = true;
                waitTime = startWaitTime;
            }

            transform.position = Vector3.MoveTowards(transform.position, moveTarget,
                speed * totalBear.modifySpeed * Time.deltaTime);
            _animator.SetBool("walk", true);

            // Поворачиваем объект в сторону moveTarget
            Vector3 direction = (moveTarget - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                targetRotation *=
                    Quaternion.Euler(0, 90, 0); // они почему-то хуярят боком, я просто доворачиваю на 90 градусов
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
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
        BearTask newTask = BearTaskManager.Singleton.GetBearTask(totalBear);
        if (_doingTask && newTask != null)
        {
            totalBear.canMove = false;
            newTask.totalSteps += 0.001f * totalBear.modifyWork;
            if (newTask.objectOfTask.CompareTag("building"))
            {
                BuildingController bc = newTask.objectOfTask.GetComponent<BuildingController>();
                bc.stepsReady = newTask.totalSteps;
                if (!bc.isBuild) // Если строится
                    bc.reveal.progress = newTask.totalSteps / newTask.needSteps;
            }

            if (newTask.needSteps != -1f && newTask.totalSteps >= newTask.needSteps)
            {
                BearTaskManager.Singleton.EndTask(newTask);
                _doingTask = false;
            }
        }

        if (!_doingTask)
            totalBear.canMove = true;

        _bearModel.gameObject.SetActive(!_doingTask);

        // Говно реализация, переделать
        if (totalBear.activity == Activities.Work)
        {
            totalBear.tired += 0.0005f * totalBear.modifyTired;
            totalBear.hungry += 0.00007f * totalBear.modifyHungry;
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