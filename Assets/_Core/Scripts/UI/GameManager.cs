using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public Image[] exclamationMarks;
    public Image[] wrenches;
    public Image endScreen;
    public Image winScreen;

    private int currentExclamation;
    private int currentWrench;
    // Start is called before the first frame update
    private void Awake()
    {
        NPCCommunicator.Instance.NPCSeenBrokenBreakableEvent += OnShock;
        currentExclamation = 0;
        currentWrench = 0;
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            WrenchActivate();
        }
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
        if (exclamationMarks != null)
        {
            exclamationMarks[currentExclamation].gameObject.SetActive(true);
            currentExclamation++;
            Debug.Log("EXCLAMATION");
            if (currentExclamation > exclamationMarks.Length - 1)
            {
                exclamationMarks = null;
                GameOverScreen();
            }
        }
    }


    public void WrenchActivate()
    {
        if (wrenches != null)
        {
            wrenches[currentWrench].gameObject.SetActive(true);
            currentWrench++;
            if (currentWrench > wrenches.Length - 1)
            {
                wrenches = null;
                WinScreen();
            }
        }
    }

    public void GameOverScreen()
    {
        endScreen.gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public void WinScreen()
    {
        winScreen.gameObject.SetActive(true);
        Time.timeScale = 0f;
    }
}
