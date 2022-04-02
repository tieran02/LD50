using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WorkType
{
    Station,
    Desk
}

public struct WorkTask
{
    public WorkStation assignedStaion;
    public AIAgent assignedWorker;
}

public class WorkManager : MonoBehaviour
{
    public List<Transform> DeskTargets;
    public List<WorkStation> Stations;

    public Dictionary<GameObject, int> DeskOwnerLookup;

    private List<WorkTask> workTasks;

    private void Awake()
    {
        DeskOwnerLookup = new Dictionary<GameObject, int>();
        workTasks = new List<WorkTask>();

        //Get all targets
        foreach (var station in FindObjectsOfType<WorkStation>())
        {
            Stations.Add(station);
        }

        foreach (var desk in GameObject.FindGameObjectsWithTag("Desk"))
        {
            DeskTargets.Add(desk.transform);
        }

        //now assign workers to a desk
        if (DeskTargets.Count > 0)
        {
            var workers = FindObjectsOfType<AIAgent>();
            for (int i = 0; i < workers.Length; i++)
            {
                if (i >= DeskTargets.Count)
                    break;

                //Start at desk
                workers[i].gameObject.transform.position = DeskTargets[i].position;
                DeskOwnerLookup.Add(workers[i].gameObject, i);
            }
        }
    }

    private void AssignWork(AIAgent agent, WorkStation station)
    {
        WorkTask task = new WorkTask
        {
            assignedWorker = agent,
            assignedStaion = station
        };

        workTasks.Add(task);
    }

    public bool RequestWork(AIAgent agent)
    {
        //Loop through all stations and find one that is not occupied
        //TODO have a cool down period for a station
        // but for now just assign out task as soon as ready
        foreach(var station in Stations)
        {
            if(!station.HasWorker())
            {
                AssignWork(agent, station);
                station.NotifyWorker(agent);
                break;
            }
        }

        return false;
    }

    public bool HasWork(AIAgent agent, ref WorkStation station)
    {
        foreach(var workTask in workTasks)
        {
            if(workTask.assignedWorker == agent)
            {
                station = workTask.assignedStaion;
                return true;
            }
        }
        return false;
    }

    private void Update()
    {
        for (int i = 0; i < workTasks.Count; i++)
        {
            if (!workTasks[i].assignedStaion.HasWorker())
            {
                workTasks.RemoveAt(i);
                --i;
            }
        }
    }
}
