using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshWander : MonoBehaviour
{
    private NavMeshAgent agent;
    public NavMeshWandererArea navMeshWandererArea;

    public float SetlocationDelay;
    float WanderDistance;
    public LayerMask EntityLayer;
    Vector3 targetLocation;
    Transform focusRotationTransform;

    public enum DogState
    {
        Wander,
        Playing,
    };

    public DogState dogState;
        


    float timer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if(navMeshWandererArea == null)
        {
            WanderDistance = 1f;
        }
        else
        {
            WanderDistance = navMeshWandererArea.WanderRadius;
        }
            

    }

    private void Start()
    {
        SetRandomLocation();
        timer = 0f;
    }

    private void Update()
    {
        switch (dogState)
        {
            case DogState.Wander:

                if (timer < SetlocationDelay)
                {
                    timer += Time.deltaTime;
                }
                else
                {
                    timer = 0;
                    SetRandomLocation();
                }
                break;

            case DogState.Playing:

                Quaternion lookOnLook = Quaternion.LookRotation(focusRotationTransform.position - transform.position);

                transform.rotation = Quaternion.Slerp(transform.rotation, lookOnLook, 4f * Time.deltaTime);

                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z);

                timer += Time.deltaTime;
                break;


        }

        

        


    }

    void SetRandomLocation()
    {
        targetLocation = RandomNavSphere(transform.position, WanderDistance, EntityLayer);
        agent.SetDestination(targetLocation);
    }




    public Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask)
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * distance;

        randomDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randomDirection, out navHit, distance, layermask);

        return navHit.position;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            timer = 0;
            dogState = DogState.Playing;
            focusRotationTransform = collision.gameObject.transform;
            targetLocation = transform.position;
            agent.SetDestination(targetLocation);
            print("Play time!");
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            timer = 0;
            dogState = DogState.Wander;
            SetRandomLocation();
        }
    }
}
