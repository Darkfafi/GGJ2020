using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
    public float speed;

    public Transform P1;
    public Transform P2;

    private float timer;

    private bool m;

    void Update()
    {
        if (!m)
        {
            transform.position = Vector3.Slerp(transform.position, P2.position, speed * Time.deltaTime);
            timer += Time.deltaTime;
            if (timer > 5)
            {
                m = true;
            }
        }
        else
        {
            transform.position = Vector3.Slerp(transform.position, P1.position, speed * Time.deltaTime);
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                m = false;
            }
        }
    }
}
