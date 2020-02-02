using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMController : MonoBehaviour
{
    public float MaxVolume = 0.3f;
    public float FadeSpeed = 0.05f;

    public AudioSource LowVolume;
    public AudioSource IntenseVolume;

    public bool Alerted;

    void Awake()
    {
        LowVolume.volume = 0f;
        IntenseVolume.volume = 0f;
    }

    void Update()
    {
        if (LowVolume.volume < MaxVolume)
        {
            LowVolume.volume +=  FadeSpeed;
        }

        if (Alerted)
        {
            if (IntenseVolume.volume < MaxVolume)
            {
                IntenseVolume.volume +=  FadeSpeed;
            }
        }
        else
        {
            if (IntenseVolume.volume > 0)
            {
                IntenseVolume.volume -=  FadeSpeed;
            }
        }
    }
}
