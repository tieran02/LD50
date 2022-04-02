using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AIAgent : MonoBehaviour
{
    public float CurrentWaitTime = 0.0f;

    private NavMeshAgent agent;
    private WorkManager workManager;

    [SerializeField]
    private float CheckForWorkTimer;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        workManager = FindObjectOfType<WorkManager>();

        CheckForWorkTimer = 0.0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        agent.avoidancePriority = 50;
        CheckForWorkTimer -= Time.deltaTime;

        WorkStation station = null;
        if(workManager.HasWork(this, ref station))
        {
            //We have a station, go to it
            agent.destination = station.transform.position;
            agent.avoidancePriority = 100;

            if (ReachedStation(station))
            {
                station.WorkerUseStation();
                CheckForWorkTimer = Random.Range(10, 30); //look for a new job between 10-30 seconds if we didn't find a job
            }
        }
        else if(CheckForWorkTimer <= 0.0f)
        {
            //Request a new job
            if(!workManager.RequestWork(this));
                CheckForWorkTimer = Random.Range(5, 15); //look for a new job between 5-15 seconds if we didn't find a job
        }
        else
        {
            //No work, just go to desk or wonder
            if(workManager.DeskOwnerLookup.ContainsKey(gameObject))
            {
                //we got a desk so just go work at desk
                int deskIndex = workManager.DeskOwnerLookup[gameObject];
                agent.destination = workManager.DeskTargets[deskIndex].transform.position;
                agent.avoidancePriority = 150;
            }
            else
            {
                // just wonder around
                if(agent.remainingDistance <= 0.8f)
                {
                    Wonder();
                }
            }
        }
    }

    private bool ReachedStation(WorkStation station)
    {
        return Vector3.Distance(transform.position, station.transform.position) < 1.0f;
    }

    void Wonder()
    {
        Vector3 randomDirection = Random.insideUnitSphere * 10.0f;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, 10.0f, 1);
        Vector3 finalPosition = hit.position;
        agent.destination = finalPosition;
    }
}
