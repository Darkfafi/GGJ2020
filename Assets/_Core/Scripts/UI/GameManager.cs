using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public Image[] exclamationMarks;
    public Image[] wrenches;

    private int currentExclamation;
    private int currentWrench;
    // Start is called before the first frame update
    private void Awake()
    {
        NPCCommunicator.Instance.NPCSeenBrokenBreakableEvent += OnShock;
        currentExclamation = 0;
        currentWrench = 0;
    }

    private void OnDestroy()
    {
        NPCCommunicator.Instance.NPCSeenBrokenBreakableEvent -= OnShock;
    }

    private void OnShock(NPC npc, Breakable breakableSeen)
    {
        ExclamationActivate();
    }

    private void ExclamationActivate()
    {
        exclamationMarks[currentExclamation].gameObject.SetActive(true);
        currentExclamation++;
        Debug.Log("EXCLAMATION");
        if (currentExclamation >= exclamationMarks.Length)
        {
            //stop?
            Debug.Log("GameOver?");
        }
    }


    public void WrenchActivate()
    {
        wrenches[currentWrench].gameObject.SetActive(true);
        currentWrench++;
        if (currentWrench >= wrenches.Length)
        {
            Debug.Log("You won?");
        }
    }
}
