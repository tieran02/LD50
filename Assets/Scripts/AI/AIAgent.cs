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
        agent.enabled = true;
        agent.avoidancePriority = 50;
        CheckForWorkTimer -= Time.deltaTime;

        WorkStation station = null;
        if(workManager.HasWork(this, ref station))
        {
            //We have a station, go to it
            Vector3 pos;
            Quaternion rot;
            station.GetWorkTransform(this, out pos, out rot);

            agent.destination = pos;
            agent.avoidancePriority = 75;

            if (ReachedStation(pos))
            {
                //snap to work pos
                transform.position = new Vector3(pos.x, transform.position.y, pos.z);
                transform.rotation = rot;
                agent.enabled = false;


                if (station.WaitForAllWorkers && station.AllWorkerInPosition())
                {
                    station.AllWorkersUseStation();
                }
                else if(!station.WaitForAllWorkers)
                {
                    station.WorkerUseStation(this);
                    float baseMaxWait = 30;
                    float baseMinWait = 10;
                    workManager.DecreaseValueDifficulty(ref baseMaxWait);
                    workManager.DecreaseValueDifficulty(ref baseMinWait);

                    CheckForWorkTimer = Random.Range(baseMinWait, baseMaxWait);
                    workManager.RemoveTask(station);
                }
            }
        }
        else if(CheckForWorkTimer <= 0.0f)
        {
            //Request a new job
            float baseMaxWait = 30;
            float baseMinWait = 10;
            workManager.DecreaseValueDifficulty(ref baseMaxWait);
            workManager.DecreaseValueDifficulty(ref baseMinWait);
            if (!workManager.RequestWork(this));
                CheckForWorkTimer = Random.Range(baseMinWait, baseMaxWait);
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

                if(ReachedStation(workManager.DeskTargets[deskIndex].transform.position))
                {
                    transform.position = workManager.DeskTargets[deskIndex].transform.position;
                    transform.rotation = workManager.DeskTargets[deskIndex].transform.rotation;
                    agent.enabled = false;
                }
            }
            else
            {
                // just wonder around
                if(ReachedStation(agent.destination))
                {
                    Wonder();
                }
            }
        }
    }

    private bool ReachedStation(Vector3 pos)
    {
        return Vector3.Distance(transform.position, pos) < 1.25f;
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
