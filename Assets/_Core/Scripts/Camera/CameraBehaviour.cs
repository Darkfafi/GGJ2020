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
    
    
    public float CameraSpeed = 2f;
    public float CameraPlayerSpeed = 5f;
    
    [Header("Follow Player Options")]
    [Range(0, 20)]
    public float PlayerFollowOffsetZ = 5f;
    
    [Range(0, 20)]
    public float PlayerFollowOffsetY = 5f;
    
    [Header("Alert Options")]
    public float AlertDuration = 2f;
    [Range(0, 20)]
    public float AlertFollowOffsetZ = 5f;
    
    [Range(0, 20)]
    public float AlertFollowOffsetY = 5f;
    
    [Header("Follow Entity Options")]
    [Range(0, 20)]
    public float EntityFollowOffsetStartOfZoomZ = 2f;
    [Range(0, 20)]
    public float EntityFollowOffsetEndOfZoomZ = 2f;
    [Range(0, 20)]
    public float EntityFollowOffsetStartOfZoomY = 2f;
    [Range(0, 20)]
    public float EntityFollowOffsetEndOfZoomY = 2f;

    [Range(0, 60)] public float ZoomTimeInSeconds = 10;

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
        prevState = cameraState;
        cameraState = CameraState.FollowEntity;
        StartCoroutine(FollowEntity(FollowingEntityLocation.Value));
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

    private Transform AlertPlayerTransform;


    void Update()
    {
        switch (cameraState)
        {
            case CameraState.FollowPlayer:
                FollowPlayer();
                break;
            
            case CameraState.FollowEntity:
                
                break;
        }
    }


    void FollowPlayer()
    {
        Vector3 TargetLocation = PlayerTransform.position + new Vector3(0, PlayerFollowOffsetY, PlayerFollowOffsetZ);
        CameraTransform.position = Vector3.Lerp(CameraTransform.position, TargetLocation, CameraPlayerSpeed * Time.deltaTime);
    }

    void FollowAlert(Transform Entity)
    {
        Vector3 TargetLocation = Entity.position + new Vector3(0, AlertFollowOffsetY, AlertFollowOffsetZ);
        CameraTransform.position = Vector3.Lerp(CameraTransform.position, TargetLocation, CameraSpeed * Time.deltaTime);
    }

    IEnumerator AlertPlayer(Transform actionLocation)
    {
        timer = 0;
        while (timer < AlertDuration)
        {
            FollowAlert(actionLocation);
            timer += Time.deltaTime;
            yield return null;
        }
        AlertLocation.Reset();
        cameraState = prevState;
    }

    IEnumerator FollowEntity(Transform entityLocation)
    {
        timer = 0;
        while (cameraState == CameraState.FollowEntity)
        {
            if (timer < ZoomTimeInSeconds)
            {
                timer += Time.deltaTime;
            }
            
            Vector3 zoom = new Vector3(
                0,
                EntityFollowOffsetStartOfZoomY + (EntityFollowOffsetEndOfZoomY - EntityFollowOffsetStartOfZoomY) * (timer /ZoomTimeInSeconds),
                EntityFollowOffsetStartOfZoomZ + (EntityFollowOffsetEndOfZoomZ - EntityFollowOffsetStartOfZoomZ) * (timer / ZoomTimeInSeconds)
            );
            
            Vector3 targetLocation = entityLocation.position + zoom;
            CameraTransform.position = Vector3.Lerp(CameraTransform.position, targetLocation, CameraSpeed * Time.deltaTime);
            

            yield return null;
        }
        FollowingEntityLocation.Reset();
        cameraState = prevState;
    }
}
