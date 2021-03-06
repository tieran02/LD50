using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WorkStation : MonoBehaviour
{
    public Transform[] WorkPoints;

    public float Cooldown = 10.0f;
    public float TimeToFail = 30.0f;
    public int StressToAdd = 10;
    public int StressToRemove = 0;
    public float WorkerBreakChance = 0.5f;

    public float PassiveBreakChance = 0.0f;
    public float PassiveBreakFrequency = 10.0f;

    public bool ShiftTask = false;
    public bool PlayerOnly = false;
    public bool PassiveOnly = false;
    public bool WaitForAllWorkers = false;
    public bool WaitWhileActive = false;

    public float RepairTime = 2.0f;
    private Collider PlayerDetection;
    private bool PlayerWithinRange = false;
    private float currentRepairStatus = 0.0f;

    public GameObject ActiveParticles;
    public Image BrokenImage;
    public Image RepairImage;

    private float LastBreakTime = 0.0f;
    private float LastPassiveTime = 0.0f;
    private float currentTimeLeft = 0.0f;
    [SerializeField]
    private bool active = false;
    [SerializeField]
    private List<AIAgent> currentWorkers;
    private WorkManager workManager;

    private void Awake()
    {
        workManager = FindObjectOfType<WorkManager>();
        PlayerDetection = GetComponent<Collider>();
        PlayerDetection.isTrigger = true;
        currentWorkers = new List<AIAgent>();

        LastBreakTime = -9999.0f;
        LastPassiveTime = Time.time + Random.Range(PassiveBreakFrequency * 0.5f, 10.0f);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (RepairImage)
            RepairImage.fillAmount = 0.0f;

        if (PlayerWithinRange && Input.GetButton("Use") && active)
        {
            Debug.Log(currentRepairStatus);

            if (RepairImage)
            {
                float value = currentRepairStatus / RepairTime;
                RepairImage.fillAmount = value;
            }

            if (BrokenImage != null)
                BrokenImage.enabled = false;

            currentRepairStatus += Time.deltaTime;
            if(currentRepairStatus >= RepairTime)
            {
                Success();
            }
            return;
        }
        currentRepairStatus = 0.0f;

        //Stations can also break passively
        if (!active && PassiveBreakChance > 0.0f && !OnPassiveCooldown())
        {
            LastPassiveTime = Time.time;
            if (Random.value <= PassiveBreakChance)
                TriggerStation();
        }

        if (!active)
        {
            if(ActiveParticles != null)
                ActiveParticles.SetActive(false);
            if (BrokenImage != null)
                BrokenImage.enabled = false;
            return;
        }
        if (ActiveParticles != null)
            ActiveParticles.SetActive(true);
        if (BrokenImage != null)
            BrokenImage.enabled = true;

        currentTimeLeft -= Time.deltaTime;
        if(currentTimeLeft <= 0.0f)
        {
            Failure();
        }
    }

    public void NotifyWorker(AIAgent agent)
    {
        currentWorkers.Add(agent);
    }

    public void ReleaseWorker(AIAgent agent)
    {
        currentWorkers.Remove(agent);
    }

    public void ReleaseAllWorkers()
    {
        for (int i = 0; i < currentWorkers.Count; i++)
        {
            ReleaseWorker(currentWorkers[i]);
            --i;
        }
    }

    public bool HasWorker(AIAgent agent)
    {
        return currentWorkers.Contains(agent);
    }

    public bool HasAnyWorker()
    {
        return currentWorkers.Count > 0;
    }

    public bool HasFreeWorkSpace()
    {
        return currentWorkers.Count < WorkPoints.Length;
    }

    public bool GetWorkTransform(AIAgent agent, out Vector3 position, out Quaternion rotation)
    {
        int index = currentWorkers.IndexOf(agent);

        if(index >= 0 && index < WorkPoints.Length)
        {
            position = WorkPoints[index].position;
            rotation = WorkPoints[index].rotation;
            return true;
        }
        position = Vector3.zero;
        rotation = Quaternion.identity;

        return false;
    }

    public bool IsActive()
    {
        return active;
    }

    public bool OnCooldown()
    {
        float scaledCooldown = Cooldown;
        workManager.DecreaseValueDifficulty(ref scaledCooldown);
        return Time.time < LastBreakTime + Mathf.Max(Cooldown * 0.5f, scaledCooldown);
    }

    public bool OnPassiveCooldown()
    {
        float scaledCooldown = PassiveBreakFrequency;
        workManager.DecreaseValueDifficulty(ref scaledCooldown);
        return Time.time < LastPassiveTime + Mathf.Max(PassiveBreakFrequency * 0.5f, scaledCooldown);
    }

    public void WorkerUseStation(AIAgent agent)
    {
        if (Random.value <= WorkerBreakChance)
            TriggerStation();
        ReleaseWorker(agent);
    }

    public void AllWorkersUseStation()
    {
        if (!active && Random.value <= WorkerBreakChance)
            TriggerStation();

        if (!WaitWhileActive || !active)
        {
            ReleaseAllWorkers();
        }
    }

    public bool AllWorkerInPosition()
    {
        if (currentWorkers.Count != WorkPoints.Length)
            return false;

        for (int i = 0; i < currentWorkers.Count; i++)
        {
            if (Vector3.Distance(currentWorkers[i].transform.position, WorkPoints[i].position) > 1.5f)
                return false;
        }
        return true;
    }

    public void TriggerStation()
    {
        currentTimeLeft = TimeToFail;
        active = true;
    }

    public void Failure()
    {
        FindObjectOfType<CharacterMovement>()?.addStress(StressToAdd);

        currentRepairStatus = 0.0f;
        currentTimeLeft = 0.0f;
        LastBreakTime = Time.time;
        active = false;

        ReleaseAllWorkers();
        workManager.RemoveTask(this);
    }

    public void Success()
    {
        FindObjectOfType<CharacterMovement>()?.addStress(-StressToRemove);

        currentRepairStatus = 0.0f;
        currentTimeLeft = 0.0f;
        LastBreakTime = Time.time;
        active = false;

        ReleaseAllWorkers();
        workManager.RemoveTask(this);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayerWithinRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayerWithinRange = false;
        }
    }
}
