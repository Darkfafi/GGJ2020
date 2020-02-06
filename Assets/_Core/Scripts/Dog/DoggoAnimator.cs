using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshWander), typeof(NavMeshAgent))]
public class DoggoAnimator : MonoBehaviour
{
    NavMeshAgent agent;
    Animator anim;
    NavMeshWander wanderer;
    AudioSource _sound;

    public AudioClip Bark;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        wanderer = GetComponent<NavMeshWander>();
        _sound = GetComponent<AudioSource>();
    }

    private void Update()
    {
        switch (wanderer.dogState)
        {
            case NavMeshWander.DogState.Wander:

                anim.SetBool("Playing", false);
                float velocity = agent.velocity.magnitude / agent.speed;
                if (velocity > 0.1f)
                {
                    anim.SetBool("Walking", true);
                }
                else
                {
                    anim.SetBool("Walking", false);
                }
                break;
        }


        
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            anim.SetBool("Playing", true);
            _sound.PlayOneShot(Bark);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            anim.SetBool("Playing", false);
        }
    }
}
