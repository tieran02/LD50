using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AIAgent : MonoBehaviour
{
    enum TargetType
    {
        Station,
        Desk
    }

    public List<Transform> Stations;
    public List<Transform> Desks;

    public float SecondsAtDesk = 5.0f;
    public float SecondsAtStation = 0.2f; //This will be replaced with the station task duration
    public float TargetDeskChance = 0.25f;

    public float CurrentWaitTime = 0.0f;

    private NavMeshAgent Agent;

    public int chosenTargetIndex = -1;
    private TargetType targetType;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();

        //Get all targets
        foreach(var station in GameObject.FindGameObjectsWithTag("Station"))
        {
            Stations.Add(station.transform);
        }

        foreach (var desk in GameObject.FindGameObjectsWithTag("Desk"))
        {
            Desks.Add(desk.transform);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Stations.Count == 0 || Desks.Count == 0)
            return;

        if (chosenTargetIndex < 0)
        {
            SetNewTarget();
        }

        bool reachedTarget = ReachedTarget();
        if(reachedTarget)
        {
            if(WaitingAtTarget())
                CurrentWaitTime -= Time.deltaTime;
            else
            {
                SetNewTarget();
            }
        }


        WalkToTarget();
    }

    private bool ReachedTarget()
    {
        bool reached = false;
        switch (targetType)
        {
            case TargetType.Station:
                reached = Vector3.Distance(transform.position, Stations[chosenTargetIndex].position) < 1.0f;
                break;
            case TargetType.Desk:
                reached = Vector3.Distance(transform.position, Desks[chosenTargetIndex].position) < 1.5f;
                break;
        }

        return reached;
    }

    private bool WaitingAtTarget()
    {
        return CurrentWaitTime > 0.0f;
    }

    private void SetNewTarget()
    {
        float rand = Random.value;

        if(rand <= TargetDeskChance)
        {
            //For now just pick a random desk
            chosenTargetIndex = Random.Range(0, Desks.Count-1);
            CurrentWaitTime = SecondsAtDesk;
            targetType = TargetType.Desk;
        }
        else
        {
            chosenTargetIndex = Random.Range(0, Stations.Count-1);
            CurrentWaitTime = SecondsAtStation;
            targetType = TargetType.Station;
        }

    }

    private void WalkToTarget()
    {
        switch (targetType)
        {
            case TargetType.Station:
                Agent.destination = Stations[chosenTargetIndex].position;
                break;
            case TargetType.Desk:
                Agent.destination = Desks[chosenTargetIndex].position;
                break;
        }
    }
}
