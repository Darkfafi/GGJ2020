using System;
using System.Collections;
using System.Collections.Generic;
using EventObjects;
using UnityEngine;
using UnityEngine.EventSystems;

public enum CameraState
{
    FollowPlayer,
    FollowEntity,
    AlertPlayer
};
public class CameraBehaviour : MonoBehaviour
{
    private CameraState cameraState;
    private CameraState prevState;
    private float timer;

    [Header("Required")] 
    public Transform CameraTransform;

    public Transform PlayerTransform;
    public TransformWithEvent AlertLocation;
    public TransformWithEvent FollowingEntityLocation;
    
    [Header("Options")]
    public float CameraSpeed = 2f;
    public float AlertDuration = 2f;
    [Range(0, 20)]
    public float FollowOffsetZ = 5f;
    
    [Range(0, 20)]
    public float FollowOffsetY = 5f;

    private void Awake()
    {
        if (!CameraTransform)
        {
            CameraTransform = transform;
        }
        AlertLocation.GetValueAndAddListener(SetAlertLocation);
        FollowingEntityLocation.GetValueAndAddListener(SetEntityToFollow);
    }
    
    public void SetEntityToFollow(Transform entity)
    {
        FollowEntityTransform = entity;
        cameraState = CameraState.FollowEntity;
        FollowingEntityLocation.SetValue(entity);
    }

    public void SetFollowPlayer()
    {
        cameraState = CameraState.FollowPlayer;
    }
        
    
    
    public void SetAlertLocation(Transform location)
    {
        if (location != null)
        {
            AlertPlayerTransform = location;
            prevState = cameraState;
            cameraState = CameraState.AlertPlayer;
            StartCoroutine(AlertPlayer(AlertPlayerTransform));
        }
    }
    
    
    public Transform FollowEntityTransform
    {
        get
        {
            return FollowEntityTransform;
        }
        set
        {
            if (value != null)
            {
                FollowEntityTransform = value;
                cameraState = CameraState.FollowEntity;
            }
        }
    }

    private Transform AlertPlayerTransform;


    void Update()
    {
        switch (cameraState)
        {
            case CameraState.FollowPlayer:
                FollowPlayer();
                break;
            
            case CameraState.FollowEntity:
                FollowEntity(FollowingEntityLocation.Value);
                break;
        }
    }


    void FollowPlayer()
    {
        Vector3 TargetLocation = PlayerTransform.position + new Vector3(0, FollowOffsetY, FollowOffsetZ);
        CameraTransform.position = Vector3.Lerp(CameraTransform.position, TargetLocation, CameraSpeed * Time.deltaTime);
    }

    void FollowEntity(Transform Entity)
    {
        Vector3 TargetLocation = Entity.position + new Vector3(0, FollowOffsetY, FollowOffsetZ);
        CameraTransform.position = Vector3.Lerp(CameraTransform.position, TargetLocation, CameraSpeed * Time.deltaTime);
    }

    IEnumerator AlertPlayer(Transform actionLocation)
    {
        timer = 0;
        while (timer < AlertDuration)
        {
            FollowEntity(actionLocation);
            timer += Time.deltaTime;
            yield return null;
        }
        AlertLocation.Reset();
        cameraState = prevState;
    }
}
