using System.Collections;
using System.Collections.Generic;
using EventObjects;
using UnityEngine;

public class FollowEntity : MonoBehaviour
{
    public TransformWithEvent EntityLocation;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            EntityLocation.SetValue(transform);
        }
    }
}
