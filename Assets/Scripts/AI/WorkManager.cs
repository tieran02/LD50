using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WorkType
{
    Station,
    Desk
}

[System.Serializable]
public class WorkTask
{
    public WorkStation assignedStaion;
    public float assignedTime;
}

public class WorkManager : MonoBehaviour
{
    public List<Transform> DeskTargets;
    public List<WorkStation> Stations;
    public List<WorkStation> ShiftStations;

    public Dictionary<GameObject, int> DeskOwnerLookup;

    [SerializeField]
    private List<WorkTask> workTasks;
    private Clock clock;


    private void Awake()
    {
        clock = FindObjectOfType<Clock>();
        DeskOwnerLookup = new Dictionary<GameObject, int>();
        workTasks = new List<WorkTask>();

        //Get all targets
        foreach (var station in FindObjectsOfType<WorkStation>())
        {
            if (!station.ShiftTask && !station.PlayerOnly)
                Stations.Add(station);
            else
                ShiftStations.Add(station);
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

        CreateShiftTasks(5, 8);
    }

    void CreateShiftTasks(int numberOfTasks, float shiftLengthHours)
    {
        if (ShiftStations.Count == 0)
        {
            Debug.LogError("Can't create shift tasks because ShiftStations is empty");
            return;
        }

        float spacing = (shiftLengthHours * 3600) / numberOfTasks;
        const int startTimeInSeconds = 32400;
        int endTimeInSeconds = startTimeInSeconds + (int)(3600 * shiftLengthHours);

        for (int i = 0; i < numberOfTasks; i++)
        {
            //Create tasks but don't assign an agent yet
            float taskTime = startTimeInSeconds + (spacing * i);

            //pick a random shift task
            int shiftTask = Random.Range(0, ShiftStations.Count);
            WorkTask task = new WorkTask
            {
                assignedStaion = ShiftStations[shiftTask],
                assignedTime = taskTime
            };

            workTasks.Add(task);
        }
    }

    private void AssignWork(AIAgent agent, WorkStation station, float scheduleTime)
    {
        WorkTask task = new WorkTask
        {
            assignedStaion = station,
            assignedTime = scheduleTime
        };

        workTasks.Add(task);
    }

    public bool RequestWork(AIAgent agent)
    {
        //Loop through all stations and find one that is not occupied
        // Priorities shift stations
        foreach (var workTask in workTasks)
        {
            if (clock.timer >= workTask.assignedTime && !workTask.assignedStaion.PlayerOnly && workTask.assignedStaion.HasFreeWorkSpace() && !workTask.assignedStaion.HasWorker(agent))
            {
                workTask.assignedStaion.NotifyWorker(agent);
                return true;
            }
        }


        // but for now just assign out task as soon as ready
        foreach (var station in Stations)
        {
            if(!station.PassiveOnly && !station.IsActive() && !station.OnCooldown() && station.HasFreeWorkSpace())
            {
                AssignWork(agent, station, -1);
                station.NotifyWorker(agent);
                return true;
            }
        }

        return false;
    }

    public bool HasWork(AIAgent agent, ref WorkStation station)
    {
        foreach(var workTask in workTasks)
        {
            if(workTask.assignedStaion.HasWorker(agent))
            {
                station = workTask.assignedStaion;
                return true;
            }
        }
        return false;
    }

    public void RemoveTask(WorkStation station)
    {
        for (int i = 0; i < workTasks.Count; i++)
        {
            if (workTasks[i].assignedStaion == station)
            {
                workTasks.RemoveAt(i);
                break;
            }
        }
    }

    private void Update()
    {
        //Trigger player only shift tasks
        foreach (var workTask in workTasks)
        {
            if (clock.timer >= workTask.assignedTime && workTask.assignedStaion.PlayerOnly && !workTask.assignedStaion.IsActive())
            {
                workTask.assignedStaion.TriggerStation();
            }
        }
    }
}
