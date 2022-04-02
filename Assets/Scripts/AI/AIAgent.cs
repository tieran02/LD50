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

    private List<BoxCollider> walkVolumes;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        workManager = FindObjectOfType<WorkManager>();

        walkVolumes = new List<BoxCollider>();
        foreach (var zone in GameObject.FindGameObjectsWithTag("WalkZone"))
        {
            var collider = zone.GetComponent<BoxCollider>();
            if (collider) walkVolumes.Add(collider);
        }

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
            agent.avoidancePriority = 75;

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
                agent.avoidancePriority = 100;

                if(agent.remainingDistance <= 0.1f)
                {
                    transform.position = workManager.DeskTargets[deskIndex].transform.position;
                    transform.rotation = workManager.DeskTargets[deskIndex].transform.rotation;
                }
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
        if (walkVolumes.Count == 0)
            return;

        BoxCollider volume = walkVolumes[Random.Range(0, walkVolumes.Count - 1)];
        Vector3 point = new Vector3( Random.Range(volume.bounds.min.x, volume.bounds.max.x), 0, Random.Range(volume.bounds.min.z, volume.bounds.max.z));

        NavMeshHit hit;
        NavMesh.SamplePosition(point, out hit, 10.0f, 1);
        Vector3 finalPosition = hit.position;
        agent.destination = finalPosition;
    }
}
