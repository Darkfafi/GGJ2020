using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavMeshWandererArea : MonoBehaviour
{
    public float WanderRadius;


    private void OnDrawGizmosSelected()
    {
       // Gizmos.color = Color.cyan;
        //Gizmos.DrawWireSphere(transform.position, WanderRadius);

        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.DrawWireDisc(transform.position, transform.up, WanderRadius);
    }
}
