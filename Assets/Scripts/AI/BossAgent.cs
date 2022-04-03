using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BossAgent : MonoBehaviour
{
    public float CurrentWaitTime = 0.0f;

    public AnimationCurve StressCurve;
    public float StressAmount = 1.0f;

    private NavMeshAgent agent;
    public List<BoxCollider> walkVolumes;
    private SphereCollider collider;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        collider = GetComponent<SphereCollider>();

        if(walkVolumes.Count == 0)
            walkVolumes = new List<BoxCollider>();

        foreach (var zone in GameObject.FindGameObjectsWithTag("WalkZone"))
        {
            var collider = zone.GetComponent<BoxCollider>();
            if (collider) walkVolumes.Add(collider);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ReachedStation(agent.destination))
        {
            Wonder();
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
        Vector3 point = new Vector3(Random.Range(volume.bounds.min.x, volume.bounds.max.x), 0, Random.Range(volume.bounds.min.z, volume.bounds.max.z));

        NavMeshHit hit;
        NavMesh.SamplePosition(point, out hit, 10.0f, 1);
        Vector3 finalPosition = hit.position;
        agent.destination = finalPosition;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            float value = Vector3.Distance(other.transform.position, transform.position);
            value /= collider.radius;
            Debug.Log(value);

            float stressAmount = StressCurve.Evaluate(value);
            FindObjectOfType<CharacterMovement>().addStress(stressAmount);
        }
    }
}
