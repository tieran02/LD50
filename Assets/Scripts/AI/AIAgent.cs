using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AIAgent : MonoBehaviour
{
    public List<Transform> Targets;
    private NavMeshAgent Agent;

    private int chosenTargetIndex = -1;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();

        //Get all targets
        foreach(var target in GameObject.FindGameObjectsWithTag("Station"))
        {
            Targets.Add(target.transform);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Targets.Count == 0)
            return;

        if (chosenTargetIndex < 0 || ReachedTarget())
            chosenTargetIndex = Random.Range(0, Targets.Count);

        Agent.destination = Targets[chosenTargetIndex].position;
        Debug.Log(chosenTargetIndex);
    }

    private bool ReachedTarget()
    {
        return Vector3.Distance(transform.position, Targets[chosenTargetIndex].position) < 1.0f;
    }
}
