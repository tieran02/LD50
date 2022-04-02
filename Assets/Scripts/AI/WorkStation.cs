using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WorkStation : MonoBehaviour
{
    public float Cooldown = 10.0f;
    public float TimeToFail = 30.0f;
    public float StressToAdd = 10.0f;
    public float WorkerBreakChance = 0.5f;

    private float currentTimeLeft = 0.0f;
    [SerializeField]
    private bool active = false;
    [SerializeField]
    private AIAgent currentWorker;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!active)
            return;

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
        //TODO add stress to player and other stuff
    }

    public void Success()
    {
        currentTimeLeft = 0.0f;
        active = false;
    }
}
