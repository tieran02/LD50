using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SphereCollider))]
public class WorkStation : MonoBehaviour
{
    public float Cooldown = 10.0f;
    public float TimeToFail = 30.0f;
    public int StressToAdd = 10;
    public float WorkerBreakChance = 0.5f;

    public float PassiveBreakChance = 0.0f;
    public float PassiveBreakFrequency = 10.0f;

    public float RepairTime = 2.0f;
    private SphereCollider PlayerDetection;
    private bool PlayerWithinRange = false;
    private float currentRepairStatus = 0.0f;

    public GameObject ActiveParticles;

    private float LastBreakTime = 0.0f;
    private float LastPassiveTime = 0.0f;
    private float currentTimeLeft = 0.0f;
    [SerializeField]
    private bool active = false;
    [SerializeField]
    private AIAgent currentWorker;

    private void Awake()
    {
        PlayerDetection = GetComponent<SphereCollider>();
        PlayerDetection.isTrigger = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerWithinRange && Input.GetKey(KeyCode.F) && active)
        {
            Debug.Log(currentRepairStatus);
            currentRepairStatus += Time.deltaTime;
            if(currentRepairStatus >= RepairTime)
            {
                Success();
            }
        }

        //Stations can also break passively
        if(!active && PassiveBreakChance > 0.0f && !OnPassiveCooldown())
        {
            LastPassiveTime = Time.time;
            if (Random.value <= PassiveBreakChance)
                TriggerStation();
        }

        if (!active)
        {
            ActiveParticles?.SetActive(false);
            return;
        }
        ActiveParticles?.SetActive(true);


        currentTimeLeft -= Time.deltaTime;
        if(currentTimeLeft <= 0.0f)
        {
            Failure();
        }
    }

    public void NotifyWorker(AIAgent agent)
    {
        currentWorker = agent;
    }

    public void ReleaseWorker()
    {
        currentWorker = null;
    }

    public bool HasWorker()
    {
        return currentWorker != null;
    }

    public bool IsActive()
    {
        return active;
    }

    public bool OnCooldown()
    {
        return Time.time < LastBreakTime + Cooldown;
    }

    public bool OnPassiveCooldown()
    {
        return Time.time < LastPassiveTime + PassiveBreakFrequency;
    }

    public void WorkerUseStation()
    {
        if (Random.value <= WorkerBreakChance)
            TriggerStation();
        ReleaseWorker();
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
    }

    public void Success()
    {
        currentRepairStatus = 0.0f;
        currentTimeLeft = 0.0f;
        LastBreakTime = Time.time;
        active = false;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayerWithinRange = true;
        }
    }
}
