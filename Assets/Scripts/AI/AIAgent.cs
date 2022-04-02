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

    private int chosenTargetIndex = -1;
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
        if (Stations.Count == 0)
            return;

        if (chosenTargetIndex < 0 || ReachedTarget())
        {
            if (CurrentWaitTime <= 0.0f)
            {
                SetNewTarget();
            }
        }
    }

    private bool ReachedTarget()
    {
        bool reached = false;
        switch (targetType)
        {
            case TargetType.Station:
                reached = Vector3.Distance(transform.position, Stations[chosenTargetIndex].position) < 1.0f;
                if (reached && CurrentWaitTime <= 0.0f)
                {
                    CurrentWaitTime = SecondsAtStation;
                    return true;
                }
                break;
            case TargetType.Desk:
                reached = Vector3.Distance(transform.position, Desks[chosenTargetIndex].position) < 1.0f;
                if (reached && CurrentWaitTime <= 0.0f)
                    CurrentWaitTime = SecondsAtDesk;
                break;
        }

        return reached;
    }

    private void SetNewTarget()
    {
        float rand = Random.value;

        if(rand <= TargetDeskChance)
        {
            //For now just pick a random desk
            int chosenTargetIndex = Random.Range(0, Desks.Count);
            Agent.destination = Desks[chosenTargetIndex].position;
            targetType = TargetType.Desk;
        }
        else
        {
            chosenTargetIndex = Random.Range(0, Stations.Count);
            Agent.destination = Stations[chosenTargetIndex].position;
            targetType = TargetType.Station;
        }

    }
}
