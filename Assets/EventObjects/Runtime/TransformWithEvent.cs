using System;
using UnityEngine;

namespace EventObjects
{
    [CreateAssetMenu(menuName = "EventObjects/Transform", fileName = "New TransformWithEvent")]
    public class TransformWithEvent : ValueWithEvent<Transform, TransformEvent> { }
}