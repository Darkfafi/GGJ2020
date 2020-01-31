using System.Collections;
using System.Collections.Generic;
using EventObjects;
using UnityEngine;

public class CameraAlert : MonoBehaviour
{
   public TransformWithEvent AlertLocation;


   void Update()
   {
      if (Input.GetKeyDown(KeyCode.X))
      {
         AlertLocation.SetValue(transform);
      }
   }
}
