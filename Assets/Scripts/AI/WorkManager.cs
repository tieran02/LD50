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

    public AnimationCurve DifficultyCurve;
    public float TaskSpacing = 60.0f; //1 hour

    public Dictionary<GameObject, int> DeskOwnerLookup;

    [SerializeField]
    private List<WorkTask> workTasks;
    private Clock clock;

    public int currentShift = 0;

    private void Awake()
    {
        clock = FindObjectOfType<Clock>();
        TaskSpacing *= clock.acceleration;

        DeskOwnerLookup = new Dictionary<GameObject, int>();
        workTasks = new List<WorkTask>();
        //currentShift = PlayerPrefs.GetInt("CurrentShift");

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

        CreateShiftTasks(8);
    }

    void CreateShiftTasks(float shiftLengthHours)
    {
        if (ShiftStations.Count == 0)
        {
            Debug.LogError("Can't create shift tasks because ShiftStations is empty");
            return;
        }

        const int startTimeInSeconds = 32400;
        int endTimeInSeconds = startTimeInSeconds + (int)(3600 * shiftLengthHours);
        float difficultySpacing = TaskSpacing * DifficultyCurve.Evaluate(currentShift);

        int maxTasks = Mathf.CeilToInt((endTimeInSeconds - startTimeInSeconds) / difficultySpacing);

        for (int i = 0; i < maxTasks; i++)
        {
            if (i > 0)
            {
                WorkTask previousTask = workTasks[i - 1];
                List<WorkStation> validStations = new List<WorkStation>();
                foreach(var station in ShiftStations)
                {
                    if (previousTask.assignedStaion != station)
                        validStations.Add(station);
                }

                //pick a random shift task
                int shiftTask = Random.Range(0, validStations.Count);
                float shiftTime = previousTask.assignedTime + previousTask.assignedStaion.Cooldown + difficultySpacing;
                shiftTime += Random.Range(5, 25);

                WorkTask task = new WorkTask
                {
                    assignedStaion = validStations[shiftTask],
                    assignedTime = shiftTime
                };

                workTasks.Add(task);
            }
            else
            {
                //Create tasks but don't assign an agent yet
                float taskTime = startTimeInSeconds;
                float timeOffset = Random.Range(5, 20);

                //pick a random shift task
                int shiftTask = Random.Range(0, ShiftStations.Count);
                WorkTask task = new WorkTask
                {
                    assignedStaion = ShiftStations[shiftTask],
                    assignedTime = taskTime + timeOffset
                };

                workTasks.Add(task);
            }
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
